using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Linq.Expressions;
using System.Data.Entity;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class EventsManager
    {
        public void UpdateEvents()
        { 
            BookmakerAccess bmax = new BookmakerAccess();
            SportEventsAccess seax = new SportEventsAccess();

            List<BE.Deporte> dep = seax.GetActiveSports();

            List<BE.EventosDeportivo> eventosNuevos = bmax.PullEvents(dep);

            List<BE.EventosDeportivo> eventosGuardados = seax.GetCurrentEvents();

            foreach (var newEvent in eventosNuevos)
            {
                if (eventosGuardados.Any(p => p.Codigo.Equals(newEvent.Codigo)))
                {
                    var evento = eventosGuardados.Where(p => p.Codigo.Equals(newEvent.Codigo)).First();
                    evento.Inicio = newEvent.Inicio;
                    evento.ApuestasDeportivas = newEvent.ApuestasDeportivas;
                }
                else
                {
                    eventosGuardados.Add(newEvent);
                }
            }

            seax.StoreEvents(eventosGuardados);
        }

        public List<BE.EventosDeportivo> GetSportEvent(string sportCode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetSportEvent(sportCode);
        }
    }
}
