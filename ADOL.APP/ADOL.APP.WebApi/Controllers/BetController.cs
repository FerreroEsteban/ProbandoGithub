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

namespace ADOL.APP.WebApi.Controllers
{
    public class BetController : ApiController
    {

        public dynamic GetUserBet(string id)
        {
            BetManager mgr = new BetManager();
            EventsManager mgrEvent = new EventsManager();
            var bets = mgr.GetUserBets(id);
            dynamic view = new List<ExpandoObject>();

            string lastProcessedLinkedCode = null;

            foreach (var bet in bets.OrderBy(b => b.LinkedCode))
            {
                if (string.IsNullOrWhiteSpace(bet.LinkedCode) || (bet.LinkedCode != lastProcessedLinkedCode))
                {

                    dynamic viewBet = new ExpandoObject();
                    viewBet.betId = bet.ID;
                    viewBet.amount = bet.Amount;
                    viewBet.simple = string.IsNullOrWhiteSpace(bet.LinkedCode);
                    viewBet.composed = !viewBet.simple;
                    if (viewBet.simple)
                    {
                        viewBet.oddType = bet.SportBet.Code;
                        viewBet.oddCode = bet.BetType;
                        viewBet.price = bet.BetPrice;

                        var match = mgrEvent.GetSportEvent(bet.MatchCode);
                        dynamic thisEvent = new ExpandoObject();
                        thisEvent.ID = match.ID;
                        thisEvent.code = match.Code;
                        thisEvent.nombre = match.Name;
                        thisEvent.local = match.Home;
                        thisEvent.visitante = match.Away;
                        thisEvent.date = match.Init.ToString("dd MMM");
                        thisEvent.time = match.Init.ToString("hh:mm");
                        viewBet.match = thisEvent;
                    }
                    else
                    {
                        viewBet.price = 1;
                        lastProcessedLinkedCode = bet.LinkedCode;
                        viewBet.betInfo = new List<ExpandoObject>();
                        foreach (var linkedBet in bets.Where(b => b.LinkedCode == bet.LinkedCode))
                        {
                            viewBet.price = viewBet.price * linkedBet.BetPrice;
                            var match = mgrEvent.GetSportEvent(bet.MatchCode);
                            dynamic betDetail = new ExpandoObject();
                            betDetail.oddType = linkedBet.SportBet.Code;
                            betDetail.oddCode = linkedBet.BetType;

                            dynamic matchDetail = new ExpandoObject();
                            matchDetail.ID = match.ID;
                            matchDetail.code = match.Code;
                            matchDetail.nombre = match.Name;
                            matchDetail.local = match.Home;
                            matchDetail.visitante = match.Away;
                            matchDetail.date = match.Init.ToString("dd MMM");
                            matchDetail.time = match.Init.ToString("hh:mm");

                            dynamic betInfoItem = new ExpandoObject();
                            betInfoItem.betDetail = betDetail;
                            betInfoItem.match = matchDetail;

                            viewBet.betInfo.Add(betInfoItem);
                        }

                    }
                    view.Add(viewBet);
                }
            }
            return view;
        }

        [WebInvoke(Method = "POST", UriTemplate = "AddUserBet")]
        public bool AddUserBet(RequestData data)
        {
            var userToken = data.token;
            var betType = (int)Enum.Parse(typeof(BetType), data.betsType);

            BetManager mgr = new BetManager();

            List<Tuple<int, decimal, string>> bets = new List<Tuple<int, decimal, string>>();
            foreach (var uibet in data.uibets)
            {
                Tuple<int, decimal, string> bet = new Tuple<int, decimal, string>(uibet.ID, uibet.Amount, uibet.BetType);
                bets.Add(bet);
            }

            return mgr.AddUserBet(userToken, (int)betType, bets);
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
