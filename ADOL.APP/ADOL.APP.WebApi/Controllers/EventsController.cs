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
            var eventos = mgr.GetSportEvent(id);
            dynamic view = new List<ExpandoObject>();
            foreach (var singleEvent in eventos)
            {
                dynamic thisEvent = new ExpandoObject();
                thisEvent.ID = singleEvent.ID;
                thisEvent.Code = singleEvent.Codigo;
                thisEvent.Nombre = singleEvent.Nombre;
                thisEvent.Local = singleEvent.Local;
                thisEvent.Visitante = singleEvent.Visitante;
                thisEvent.Date = singleEvent.Inicio.ToString("dd MMM");
                thisEvent.Time = singleEvent.Inicio.ToString("hh:mm");
                thisEvent.ApuestasDisponibles = GetEventOdds(singleEvent.ApuestasDeportivas);
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

        private dynamic GetEventOdds(ICollection<ApuestasDeportiva> collection)
        {
            dynamic view = new List<ExpandoObject>();
            foreach (var apuesta in collection)
            {
                dynamic apuestaDisponible = new ExpandoObject();
                apuestaDisponible.ID = apuesta.ID;

                dynamic oddCollection = new List<ExpandoObject>();
                foreach (var oddtype in apuesta.OddProvider.GetAvailables())
                {
                    dynamic singleOdd = new ExpandoObject();
                    singleOdd.Code = oddtype.Key;
                    singleOdd.Name = oddtype.Value;
                    singleOdd.Price = apuesta.GetOddPrice(oddtype.Key);
                    oddCollection.Add(singleOdd);
                }

                apuestaDisponible.OddCollection = oddCollection;

                view.Add(apuestaDisponible);
            }
            return view;
        }
    }
}