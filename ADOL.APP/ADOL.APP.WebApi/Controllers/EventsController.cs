using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Dynamic;

namespace ADOL.APP.WebApi.Controllers
{
    public class EventsController : ApiController
    {
        public dynamic GetActiveEvents(string id)
        {
            EventsManager mgr = new EventsManager();
            var leagueEvents = mgr.GetLeagueEvents(id);
            dynamic view = new List<ExpandoObject>();
            foreach (var singleEvent in leagueEvents)
            {
                dynamic thisEvent = new ExpandoObject();
                thisEvent.ID = singleEvent.ID;
                thisEvent.Code = singleEvent.Code;
                thisEvent.Nombre = singleEvent.Name;
                thisEvent.Local = singleEvent.Home;
                thisEvent.Visitante = singleEvent.Away;
                thisEvent.Date = singleEvent.Init.ToString("dd MMM");
                thisEvent.Time = singleEvent.Init.ToString("hh:mm");
                thisEvent.ApuestasDisponibles = GetEventOdds(singleEvent.SportBets);
                view.Add(thisEvent);
            }
            return view;
        }

        public dynamic GetEventOdds(string id)
        {
            EventsManager mgr = new EventsManager();
            var odds = mgr.GetEventOdds(id);
            return GetEventOdds(odds);
        }

        private dynamic GetEventOdds(ICollection<SportBet> collection)
        {
            dynamic view = new List<ExpandoObject>();
            foreach (var bet in collection)
            {
                dynamic betAvailable = new ExpandoObject();
                betAvailable.ID = bet.ID;
                betAvailable.oddtype = bet.Code;
                dynamic oddCollection = new List<ExpandoObject>();
                foreach (var oddtype in bet.OddProvider.GetAvailables())
                {
                    dynamic singleOdd = new ExpandoObject();
                    singleOdd.Code = oddtype.Key;
                    singleOdd.Name = oddtype.Value;
                    singleOdd.Price = bet.GetOddPrice(oddtype.Key);
                    oddCollection.Add(singleOdd);
                }

                betAvailable.OddCollection = oddCollection;

                view.Add(betAvailable);
            }
            return view;
        }
    }
}