using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Dynamic;
using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;

namespace ADOL.APP.WebApi.Controllers
{
    public class EventsController : ApiBaseController
    {
        public dynamic GetActiveEvents(string id)
        {
            EventsManager mgr = new EventsManager();
            var leagueEvents = mgr.GetTournamentEvents(id);
            List<EventDTO> view = new List<EventDTO>();
            foreach (var singleEvent in leagueEvents)
            {
                EventDTO thisEvent = new EventDTO();
                thisEvent.ID = singleEvent.ID;
                thisEvent.Code = singleEvent.Code;
                thisEvent.Name = singleEvent.Name;
                thisEvent.Local = singleEvent.Home;
                thisEvent.Visitante = singleEvent.Away;
                thisEvent.Date = singleEvent.Init.ToString("dd MMM");
                thisEvent.Time = singleEvent.Init.ToString("hh:mm");
                thisEvent.AvailableBets = GetEventOdds(singleEvent.SportBets);
                view.Add(thisEvent);
            }
            return this.GetView(view);
        }

        public dynamic GetEventOdds(string id)
        {
            EventsManager mgr = new EventsManager();
            var odds = mgr.GetEventOdds(id);
            return GetEventOdds(odds);
        }

        private List<BetDTO> GetEventOdds(ICollection<SportBet> collection)
        {
            List<BetDTO> bets = new List<BetDTO>();
            foreach (var bet in collection)
            {
                BetDTO betAvailable = new BetDTO();
                betAvailable.ID = bet.ID;
                betAvailable.OddType = bet.Code;
                List<OddDTO> oddCollection = new List<OddDTO>();
                foreach (var oddType in bet.OddProvider.GetAvailables())
                {
                    OddDTO singleOdd = new OddDTO();
                    singleOdd.ID = string.Format("{0}_{1}", bet.ID, oddType.Key);
                    singleOdd.Code = oddType.Key;
                    singleOdd.Price = bet.GetOddPrice(oddType.Key);
                    oddCollection.Add(singleOdd);
                }

                betAvailable.OddCollection = oddCollection;

                bets.Add(betAvailable);
            }
            return bets;
        }
    }
}