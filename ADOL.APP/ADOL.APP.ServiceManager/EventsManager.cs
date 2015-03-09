using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using System.Linq.Expressions;
using System.Data.Entity;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class EventsManager
    {
        public void UpdateEvents(string eventTypeCodes)
        { 
            BookmakerAccess bmax = new BookmakerAccess();
            List<EventosDeportivo> eventosNuevos = bmax.PullEvents(eventTypeCodes.Split(','));

            List<EventosDeportivo> eventosGuardados = bmax.GetCurrentEvents();

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

            bmax.StoreEvents(eventosGuardados);
        }
    }
}
