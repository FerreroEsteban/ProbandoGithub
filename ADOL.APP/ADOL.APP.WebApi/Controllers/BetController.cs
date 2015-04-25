using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;
using ADOL.APP.CurrentAccountService.Helpers;

namespace ADOL.APP.WebApi.Controllers
{
    public class BetController : ApiBaseController
    {

        public dynamic GetUserBet(string id)
        {
            BetManager mgr = new BetManager();
            EventsManager mgrEvent = new EventsManager();
            //var bets = mgr.GetUserBets(id);
            var bets = mgr.GetUserBets(RequestContextHelper.SessionToken);
               
            if (bets.Status.Equals(ResponseStatus.OK))
            {
                List<UserBetDTO> betList = new List<UserBetDTO>();

                string lastProcessedLinkedCode = null;

                foreach (var bet in bets.GetData().OrderBy(b => b.LinkedCode))
                {
                    if (string.IsNullOrWhiteSpace(bet.LinkedCode) || (bet.LinkedCode != lastProcessedLinkedCode))
                    {

                        UserBetDTO viewBet = new UserBetDTO();
                        viewBet.ID = bet.ID;
                        viewBet.Amount = bet.Amount;
                        viewBet.Simple = string.IsNullOrWhiteSpace(bet.LinkedCode);
                        viewBet.Composed = !viewBet.Simple;
                        if (viewBet.Simple)
                        {
                            viewBet.OddType = bet.SportBet.Code;
                            viewBet.OddCode = bet.BetType;
                            viewBet.Price = bet.BetPrice;

                            var match = mgrEvent.GetSportEvent(bet.MatchCode);
                            MatchDTO thisEvent = new MatchDTO();
                            thisEvent.ID = match.ID;
                            thisEvent.Code = match.Code;
                            thisEvent.Name = match.Name;
                            thisEvent.Local = match.Home;
                            thisEvent.Visitante = match.Away;
                            thisEvent.Date = match.Init.ToString("dd MMM");
                            thisEvent.Time = match.Init.ToString("hh:mm");
                            viewBet.Match = thisEvent;
                        }
                        else
                        {
                            viewBet.Price = 1;
                            lastProcessedLinkedCode = bet.LinkedCode;
                            viewBet.BetInfo = new List<BetInfoItemDTO>();
                            foreach (var linkedBet in bets.GetData().Where(b => b.LinkedCode == bet.LinkedCode))
                            {
                                viewBet.Price = viewBet.Price * linkedBet.BetPrice;
                                var match = mgrEvent.GetSportEvent(bet.MatchCode);
                                BetDetailDTO betDetail = new BetDetailDTO();
                                betDetail.OddType = linkedBet.SportBet.Code;
                                betDetail.OddCode = linkedBet.BetType;

                                MatchDTO matchDetail = new MatchDTO();
                                matchDetail.ID = match.ID;
                                matchDetail.Code = match.Code;
                                matchDetail.Name  = match.Name;
                                matchDetail.Local = match.Home;
                                matchDetail.Visitante = match.Away;
                                matchDetail.Date = match.Init.ToString("dd MMM");
                                matchDetail.Time = match.Init.ToString("hh:mm");

                                BetInfoItemDTO betInfoItem = new BetInfoItemDTO();
                                betInfoItem.BetDetail = betDetail;
                                betInfoItem.Match = matchDetail;

                                viewBet.BetInfo.Add(betInfoItem);
                            }

                        }
                        betList.Add(viewBet);
                    }

                }
                
                return this.GetView(betList);
            }
            
            return this.GetView("OperationFails");
        }

        [WebInvoke(Method = "POST", UriTemplate = "AddUserBet")]
        public dynamic AddUserBet(RequestData data)
        {
            var userToken = RequestContextHelper.SessionToken; //data.token;
            var betType = (int)Enum.Parse(typeof(BetType), data.betsType);

            BetManager mgr = new BetManager();

            List<Tuple<int, decimal, string>> bets = new List<Tuple<int, decimal, string>>();
            foreach (var uibet in data.uibets)
            {
                Tuple<int, decimal, string> bet = new Tuple<int, decimal, string>(uibet.ID, uibet.Amount, uibet.BetType);
                bets.Add(bet);
            }

            var success = mgr.AddUserBet(userToken, (int)betType, bets);
            return this.GetUserBet(RequestContextHelper.SessionToken);
        }
    }

    public class RequestData
    {
        public string token { get; set; }
        public string betsType { get; set; }
        public List<UIBet> uibets { get; set; }
    }

    public class UIBet
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public string BetType { get; set; }
    }
}
