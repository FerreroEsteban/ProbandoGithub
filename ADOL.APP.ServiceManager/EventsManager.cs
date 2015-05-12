using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Linq.Expressions;
using System.Data.Entity;
using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;
using ADOL.APP.CurrentAccountService.Helpers;
using System.Xml.Linq;
using System.Collections;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class EventsManager : BaseManager
    {
        public EventsManager() :base()
        { 
        
        }

        public void UpdateEvents()
        { 
            BookmakerAccess bmax = new BookmakerAccess();
            SportEventsAccess seax = new SportEventsAccess();

            List<BE.Sport> dep = seax.GetActiveSports();

            List<BE.SportEvent> eventosNuevos = bmax.PullEvents(dep);

            List<BE.SportEvent> eventosGuardados = seax.GetCurrentEvents();

            foreach (var newEvent in eventosNuevos)
            {
                if (eventosGuardados.Any(p => p.Code.Equals(newEvent.Code)))
                {
                    var evento = eventosGuardados.Where(p => p.Code.Equals(newEvent.Code)).First();
                    evento.Init = newEvent.Init;
                    evento.SportBets = newEvent.SportBets.ToList();
                }
                else
                {
                    eventosGuardados.Add(newEvent);
                }
            }

            seax.StoreEvents(eventosGuardados);
        }

        public List<BE.SportEvent> GetSportEvents(string sportCode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetSportEvents(sportCode);
        }

        public BE.SportEvent GetSportEvent(string matchCode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetSportEvent(matchCode);
        }

        public List<BE.SportEvent> GetLeagueEvents(string leagueId)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEvents(leagueId);
        }

        public List<BE.SportEvent> GetTournamentEvents(string tournamentId)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEventsByTournament(tournamentId);
        }

        public List<BE.SportBet> GetEventOdds(string matchID)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEventOdd(matchID);
        }

        public List<SportDTO> GetActiveSports(BE.BaseRequest req)
        {
            SportEventsAccess seax = new SportEventsAccess();
            var successLogin = true;
            if (!string.IsNullOrEmpty(req.LaunchToken) && string.IsNullOrEmpty(RequestContextHelper.SessionToken))
            {
                var response = UserWalletFacade.ProcessLogin(req);
                if (response.Status.Equals(BE.ResponseStatus.OK))
                {
                    //RequestContextHelper.SessionToken = response.GetData().SessionToken;
                    //RequestContextHelper.UserBalance = response.GetData().Balance;
                    //RequestContextHelper.UserName = response.GetData().NickName;

                    UserAccess ua = new UserAccess();
                    BE.User user = new BE.User();
                    user.LaunchToken = req.LaunchToken;
                    user.SessionToken = response.GetData().SessionToken;
                    user.UID = response.GetData().UserUID;
                    user.Balance = response.GetData().Balance;
                    user.NickName = response.GetData().NickName;

                    ua.LoginUser(user);
                    this.UpdateCurrentUserData(user);
                }
                else
                {
                    successLogin = false;
                    RequestContextHelper.LastError = response.Message;
                }
            }

            if (successLogin)
            {
                List<BE.Sport> sports = seax.GetActiveSports();

                List<SportDTO> sportsDto = new List<SportDTO>();

                string lastCode = null, lastRegion = null, lastCountry = null;

                foreach (var sport in sports.OrderBy(s => s.Code).ThenBy(s => s.RegionName).ThenBy(s => s.CountryName).ThenBy(s => s.Name))
                {
                    if (lastCode != sport.Code)
                    {
                        lastCode = sport.Code;
                        SportDTO viewSport = new SportDTO();
                        List<RegionDTO> regions = new List<RegionDTO>();

                        viewSport.Code = lastCode;
                        viewSport.Name = sport.Name;

                        foreach (var region in sports.Where(s => s.Code == lastCode).OrderBy(s => s.RegionName).ThenBy(s => s.CountryName).ThenBy(s => s.Name))
                        {
                            if (lastRegion != region.RegionID)
                            {
                                lastRegion = region.RegionID;
                                RegionDTO newRegion = new RegionDTO();
                                List<CountryDTO> countries = new List<CountryDTO>();

                                newRegion.Name = region.RegionName;
                                newRegion.Code = region.RegionID;

                                #region paises
                                foreach (var pais in sports.Where(s => s.Code == lastCode && s.RegionID == lastRegion).OrderBy(s => s.CountryName).ThenBy(s => s.TournamentName))
                                {
                                    if (lastCountry != pais.Country)
                                    {
                                        CountryDTO country = new CountryDTO();
                                        List<LeagueDTO> leagues = new List<LeagueDTO>();

                                        lastCountry = pais.Country;
                                        country.Code = pais.Country;
                                        country.Flag = pais.MenuFlagKey;
                                        country.Name = pais.CountryName;
                                        #region Ligas
                                        foreach (var liga in sports.Where(s => s.Code == lastCode && s.RegionID == lastRegion && s.Country == lastCountry).OrderBy(s => s.TournamentName))
                                        {
                                            LeagueDTO league = new LeagueDTO();
                                            league.Code = liga.TournamentID;
                                            league.Name = liga.TournamentName == "" ? liga.InternalName : liga.TournamentName;
                                            leagues.Add(league);
                                        }
                                        #endregion
                                        country.Leagues = leagues;
                                        countries.Add(country);
                                    }
                                }
                                #endregion
                                newRegion.Countries = countries;
                                regions.Add(newRegion);
                            }
                        }
                        viewSport.Regions = regions;

                        sportsDto.Add(viewSport);
                    }
                }
                return sportsDto;
            }
            return new List<SportDTO>();
        }

        public List<BE.Sport> GetActiveSportLeagues(string sportcode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetActiveSports(sportcode);
        }

        public void CheckResults()
        {
            BookmakerAccess bma = new BookmakerAccess();
            UserBetAccess uba = new UserBetAccess();
            var leagues = uba.GetPendingLeagues();
            List<BE.MatchResults> results = bma.PullResults(leagues);
            if (results.Count > 0)
            {
                //UserBetAccess uba = new UserBetAccess();
                var userbets = uba.GetPendings(results.Select(p => p.MatchID).ToArray());
                if (userbets.Count > 0)
                {
                    foreach (var result in results)
                    {
                        foreach (var userbet in userbets.Where(p => p.Item1.Equals(result.MatchID)))
                        {
                            try
                            {
                                var oddProvider = userbet.Item2.GetOddProvider();
                                BE.PaymentStatus status = 0;
                                var hitStatus = oddProvider.ValidateUserBet(result, userbet.Item2, out status);
                                uba.UpdateUserBetStatus(userbet.Item2.ID, hitStatus, status);
                                
                            }
                            catch (Exception ex)
                            { 
                                //do something

                            }
                        }
                    }
                    PaySingleBets(uba);
                    PayCombinedBets(uba);
                }
            }
        }

        private void PaySingleBets(UserBetAccess uba)
        {
            UserAccess ua = new UserAccess();
            var pendings = uba.GetPaymentPendings(false);
            pendings.ToList().ForEach(p =>
            {
                BE.CreditRequest req = CreateCreditRequest(p, p.PaymentStatus.Value.Equals(3));
                BE.BaseResponse<BE.BaseWalletResponseData> resp = UserWalletFacade.ProcessBetCredit(req);
                if (resp.Status.Equals(BE.ResponseStatus.OK))
                {
                    uba.UpdateUserBetStatus(p.ID, p.Hit.Value, ((BE.PaymentStatus)(p.PaymentStatus.Value + 2)));
                    p.User.SessionToken = resp.GetData().SessionToken;
                    p.User.Balance = resp.GetData().Balance;
                    
                    var user = ua.UpdateUser(p.User);
                }
            });
        }

        private void PayCombinedBets(UserBetAccess uba)
        {
            var pendings = uba.GetPaymentPendings(true);
            UserAccess ua = new UserAccess();
            var linkeds = pendings.Select(p => p.LinkedCode).Distinct().ToList();
            linkeds.ForEach(l => {
                var combined = pendings.Where(c => c.LinkedCode.Equals(l)).ToList();
                if (combined.All(p => p.Hit.HasValue && p.Hit.Value))
                {
                    BE.CreditRequest req = CreateCreditRequest(combined);
                    BE.BaseResponse<BE.BaseWalletResponseData> resp = UserWalletFacade.ProcessBetCredit(req);
                    if (resp.Status.Equals(BE.ResponseStatus.OK))
                    {
                        foreach (var bet in combined)
                        {
                            uba.UpdateUserBetStatus(bet.ID, bet.Hit.Value, BE.PaymentStatus.PayedBack);
                        }
                        var user = combined[0].User;
                        user.SessionToken = resp.GetData().SessionToken;
                        user.Balance = resp.GetData().Balance;

                        user = ua.UpdateUser(user);
                    }
                }
                //else
                //{
                //    var returnBets = combined.Where(p => p.Hit.HasValue && !p.Hit.Value && p.PaymentStatus.Equals((int)BE.PaymentStatus.Return));
                //    if (returnBets != null && returnBets.Count() > 0)
                //    {
                //        returnBets.ToList().ForEach(p =>
                //        {
                //            BE.CreditRequest req = CreateCreditRequest(p, true);
                //            BE.BaseResponse<BE.BaseWalletResponseData> resp = UserWalletFacade.ProcessBetCredit(req);
                //            if (resp.Status.Equals(BE.ResponseStatus.OK))
                //            {
                //                uba.UpdateUserBetStatus(p.ID, p.Hit.Value, BE.PaymentStatus.Returned);
                //                p.User.SessionToken = resp.GetData().SessionToken;
                //                p.User.Balance = resp.GetData().Balance;

                //                var user = ua.UpdateUser(p.User);
                //            }
                //        });
                //    }
                //}
            });
            

                
                
        }

        private BE.CreditRequest CreateCreditRequest(BE.UserBet userbet, bool isReturn = false)
        {
            var payment = isReturn ? userbet.Amount : (userbet.Amount * userbet.BetPrice);
            BE.CreditRequest req = new BE.CreditRequest();
            req.Amount = payment;
            req.EventID = userbet.SportBet.SportEventID.ToString();
            req.EventName = userbet.SportBet.SportEvent.Name;
            req.SessionToken = userbet.User.SessionToken;
            req.TransactionID = userbet.TransactionID;
            req.UserUID = userbet.User.UID;
            req.BetDetail = GetCreditDetail(userbet, userbet.User);

            return req;
        }

        private BE.CreditRequest CreateCreditRequest(List<BE.UserBet> userbet)
        {
            var dataBet = userbet.Where(p => p.Amount > 0).First();
            BE.CreditRequest req = new BE.CreditRequest();
            req.Amount = dataBet.Amount;
            req.EventID = string.Join("|", userbet.Select(p => p.SportBet.SportEventID.ToString()).ToArray());
            req.EventName = string.Join("|", userbet.Select(p => p.SportBet.SportEvent.Name).ToArray());
            req.SessionToken = dataBet.User.SessionToken;
            req.TransactionID = dataBet.TransactionID;
            req.UserUID = dataBet.User.UID;
            req.BetDetail = GetCreditDetail(userbet, dataBet.User);

            return req;
        }

        private string GetCreditDetail(BE.UserBet userbet, BE.User user)
        {
            XElement detail = new XElement("CreditDetail",
                new XElement("BetType", "single"),
                new XElement("UserUID", user.UID),
                new XElement("TransactionID", userbet.TransactionID),
                new XElement("MatchCode", userbet.MatchCode),
                new XElement("MatchName", userbet.MatchName),
                new XElement("BetName", userbet.BetType),
                new XElement("BetPrice", userbet.BetPrice),
                new XElement("BetAmount", userbet.Amount),
                new XElement("Date", DateTime.UtcNow)
                );

            return detail.ToString(SaveOptions.DisableFormatting);
        }

        private string GetCreditDetail(List<BE.UserBet> userbets, BE.User user)
        {
            var dataBet = userbets.Where(p => p.Amount > 0).First();
            XElement detail = new XElement("CreditDetail",
                    new XElement("BetType", "combined"),
                    new XElement("UserUID", user.UID),
                    new XElement("TransactionID", dataBet.TransactionID),
                    new XElement("BetAmount", dataBet.Amount),
                    new XElement("BetPrice", dataBet.BetPrice),
                    new XElement("Date", DateTime.UtcNow));

            XElement eventsDetail = new XElement("EventsDetails");

            foreach (var userbet in userbets)
            {
                XElement matchDetails = new XElement("MatchDetail",
                     new XElement("MatchCode", userbet.MatchCode),
                     new XElement("MatchName", userbet.MatchName),
                     new XElement("BetName", userbet.BetType)
                     );

                eventsDetail.Add(matchDetails);
            }

            detail.Add(eventsDetail);
            return detail.ToString(SaveOptions.DisableFormatting);
        }
    }
}
