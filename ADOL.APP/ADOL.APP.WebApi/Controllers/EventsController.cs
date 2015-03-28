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
            var eventos = mgr.GetLeagueEvents(id);
            dynamic view = new List<ExpandoObject>();
            foreach (var singleEvent in eventos)
            {
                dynamic thisEvent = new ExpandoObject();
                thisEvent.ID = singleEvent.ID;
                thisEvent.code = singleEvent.Codigo;
                thisEvent.nombre = singleEvent.Nombre;
                thisEvent.local = singleEvent.Local;
                thisEvent.visitante = singleEvent.Visitante;
                thisEvent.date = singleEvent.Inicio.ToString("dd MMM");
                thisEvent.time = singleEvent.Inicio.ToString("hh:mm");
                thisEvent.apuestasDisponibles = GetEventOdds(singleEvent.ApuestasDeportivas);
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
                apuestaDisponible.oddtype = apuesta.Codigo;
                dynamic oddCollection = new List<ExpandoObject>();
                foreach (var oddtype in apuesta.OddProvider.GetAvailables())
                {
                    dynamic singleOdd = new ExpandoObject();
                    singleOdd.ID = string.Format("{0}_{1}", apuesta.ID, oddtype.Key);
                    singleOdd.code = oddtype.Key;
                    singleOdd.name = oddtype.Value;
                    singleOdd.price = apuesta.GetOddPrice(oddtype.Key);
                    oddCollection.Add(singleOdd);
                }

                apuestaDisponible.oddCollection = oddCollection;

                view.Add(apuestaDisponible);
            }
            return view;
        }
    }
}