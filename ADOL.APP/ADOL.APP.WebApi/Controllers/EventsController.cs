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
                thisEvent.code = singleEvent.Code;
                thisEvent.nombre = singleEvent.Name;
                thisEvent.local = singleEvent.Home;
                thisEvent.visitante = singleEvent.Away;
                thisEvent.date = singleEvent.Init.ToString("dd MMM");
                thisEvent.time = singleEvent.Init.ToString("hh:mm");
                thisEvent.apuestasDisponibles = GetEventOdds(singleEvent.SportBets);
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
                betAvailable.oddType = bet.Code;
                dynamic oddCollection = new List<ExpandoObject>();
                foreach (var oddType in bet.OddProvider.GetAvailables())
                {
                    dynamic singleOdd = new ExpandoObject();
                    singleOdd.ID = string.Format("{0}_{1}", bet.ID, oddType.Key);
                    singleOdd.code = oddType.Key;
                    singleOdd.price = bet.GetOddPrice(oddType.Key);
                    oddCollection.Add(singleOdd);
                }

                betAvailable.oddCollection = oddCollection;

                view.Add(betAvailable);
            }
            return view;
        }
    }
}