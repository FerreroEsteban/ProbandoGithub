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
        public dynamic GetActiveEvents(string sportCode)
        {
            EventsManager mgr = new EventsManager();
            var eventos = mgr.GetSportEvent(sportCode);
            dynamic view = new List<ExpandoObject>();
            foreach (var singleEvent in eventos)
            {
                dynamic thisEvent = new ExpandoObject();
                thisEvent.ID = singleEvent.ID;
                thisEvent.Nombre = singleEvent.Nombre;
                thisEvent.Local = singleEvent.Local;
                thisEvent.Visitante = singleEvent.Visitante;
                thisEvent.Starting = singleEvent.Inicio;
                thisEvent.ApuestasDisponibles = GetEventOdds(singleEvent.ApuestasDeportivas);
                view.Add(thisEvent);
            }
            return view;
        }

        private dynamic GetEventOdds(ICollection<ApuestasDeportiva> collection)
        {
            dynamic view = new List<ExpandoObject>();
            foreach (var apuesta in collection)
            {
                dynamic apuestaDisponible = new ExpandoObject();
                apuestaDisponible.ID = apuesta.ID;
                apuestaDisponible.Codigo = apuesta.Codigo;
                apuestaDisponible.Local = apuesta.Odd1;
                apuestaDisponible.Empate = apuesta.Odd2;
                apuestaDisponible.Visitante = apuesta.Odd3;
                view.Add(apuestaDisponible);
            }
            return view;
        }
    }
}