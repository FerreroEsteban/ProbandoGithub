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
        public bool AddUserBet(dynamic betData)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(betData);

            BetManager mgr = new BetManager();

            List<Tuple<int, decimal, string>> bets = new List<Tuple<int, decimal, string>>();
            foreach (var uibet in betData.uibets)
            {
                Tuple<int, decimal, string> bet = new Tuple<int, decimal, string>((int)uibet.ID.Value, (decimal)uibet.Amount.Value, (string)uibet.BetType.Value);
                bets.Add(bet);                 
            }

            return mgr.AddUserBet((string)betData.token.Value, (int)betData.ID.Value, bets);
        }
    }
}
