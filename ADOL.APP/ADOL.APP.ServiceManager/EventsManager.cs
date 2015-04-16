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

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class EventsManager
    {
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

        public List<SportDTO> GetActiveSports()
        {
            SportEventsAccess seax = new SportEventsAccess();
            if (!string.IsNullOrEmpty(req.LaunchToken))
            {
                var response = UserWalletFacade.ProcessLogin(req);
                if (response.Status.Equals(BE.ResponseStatus.OK))
                {
                    RequestContextHelper.SetCurrentToken(response.GetData().SessionToken);
                    RequestContextHelper.SetCurrentBalance(response.GetData().Balance);
                }
            }
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

        public List<BE.Sport> GetActiveSportLeagues(string sportcode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetActiveSports(sportcode);
        }

        public void CheckResults(string sportCode)
        {
            BookmakerAccess bma = new BookmakerAccess();
            List<BE.MatchResults> results = bma.PullResults(sportCode);
            if (results.Count > 0)
            {
                UserBetAccess uba = new UserBetAccess();
                var userbets = uba.GetPendings(results.Select(p => p.MatchID).ToArray());
                if (userbets.Count > 0)
                {
                    foreach (var result in results)
                    {
                        foreach (var userbet in userbets.Where(p => p.Item1.Equals(result.MatchID)))
                        {
                            var oddProvider = userbet.Item2.GetOddProvider();
                            var betStatus = oddProvider.ValidateUserBet(result, userbet.Item2);
                            uba.UpdateUserBetStatus(userbet.Item2.ID, betStatus);
                        }
                    }
                }
            }
        }
    }
}
