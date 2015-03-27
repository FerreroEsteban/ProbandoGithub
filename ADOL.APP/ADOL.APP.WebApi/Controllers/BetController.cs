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
            var bets = mgr.GetUserBets(id);
            dynamic view = new List<ExpandoObject>();
            foreach (var bet in bets)
            {
                dynamic viewBet = new ExpandoObject();
                viewBet.ID = bet.ID;
                viewBet.BetType = bet.BetType;
                viewBet.BetPrice = bet.BetPrice;
                viewBet.Amount = bet.Amount;
                viewBet.BetTitle = bet.ApuestasDeportiva.OddProvider.GetOddName(bet.BetType);
                view.Add(viewBet);
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
