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

        public List<BE.EventosDeportivo> GetLeagueEvents(string leagueId)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEvents(leagueId);
        }

        public List<BE.ApuestasDeportiva> GetEventOdds(string matchID)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEventOdd(matchID);
        }

        public List<BE.Deporte> GetActiveSports()
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetActiveSports();
        }

        public List<BE.Deporte> GetActiveSportLeagues(string sportcode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetActiveSports(sportcode);
        }

        public void CheckResults(string sportCode)
        {
            BookmakerAccess bma = new BookmakerAccess();
            List<BE.MatchResults> results = bma.PullResults(sportCode);
            if (results.Count > 0)
            {
                UserBetAccess uba = new UserBetAccess();
                var userbets = uba.GetPendings(results.Select(p => p.MatchID).ToArray());
                if (userbets.Count > 0)
                {
                    foreach (var result in results)
                    {
                        foreach (var userbet in userbets.Where(p => p.ApuestasDeportiva.Codigo.Equals(result.MatchID)))
                        {
                            var oddProvider = userbet.GetOddProvider();
                            var betStatus = oddProvider.ValidateUserBet(result, userbet);
                            uba.UpdateUserBetStatus(userbet.ID, betStatus);
                        }
                    }
                }
            }
        }
    }
}
