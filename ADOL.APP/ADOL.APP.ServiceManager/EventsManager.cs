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

            List<BE.Sport> dep = seax.GetActiveSports();

            List<BE.SportEvent> eventosNuevos = bmax.PullEvents(dep);

            List<BE.SportEvent> eventosGuardados = seax.GetCurrentEvents();

            foreach (var newEvent in eventosNuevos)
            {
                if (eventosGuardados.Any(p => p.Code.Equals(newEvent.Code)))
                {
                    var evento = eventosGuardados.Where(p => p.Code.Equals(newEvent.Code)).First();
                    evento.Init = newEvent.Init;
                    evento.SportBets = newEvent.SportBets.ToList();
                }
                else
                {
                    eventosGuardados.Add(newEvent);
                }
            }

            seax.StoreEvents(eventosGuardados);
        }

        public List<BE.SportEvent> GetSportEvents(string sportCode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetSportEvents(sportCode);
        }

        public BE.SportEvent GetSportEvent(string matchCode)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetSportEvent(matchCode);
        }

        public List<BE.SportEvent> GetLeagueEvents(string leagueId)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEvents(leagueId);
        }

        public List<BE.SportEvent> GetTournamentEvents(string tournamentId)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEventsByTournament(tournamentId);
        }

        public List<BE.SportBet> GetEventOdds(string matchID)
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetEventOdd(matchID);
        }

        public List<BE.Sport> GetActiveSports()
        {
            SportEventsAccess seax = new SportEventsAccess();
            return seax.GetActiveSports();
        }

        public List<BE.Sport> GetActiveSportLeagues(string sportcode)
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
                        foreach (var userbet in userbets.Where(p => p.Item1.Equals(result.MatchID)))
                        {
                            var oddProvider = userbet.Item2.GetOddProvider();
                            var betStatus = oddProvider.ValidateUserBet(result, userbet.Item2);
                            uba.UpdateUserBetStatus(userbet.Item2.ID, betStatus);
                        }
                    }
                }
            }
        }
    }
}
