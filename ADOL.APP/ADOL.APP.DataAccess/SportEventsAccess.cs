using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Data.Entity;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class SportEventsAccess
    {
        public List<BE.SportEvent> GetCurrentEvents()
        {
            using (var dbcontext = new BE.ADOLDBEntities())
            {
                return dbcontext.SportEvents.Include("SportBets").Where(p => p.Active && p.Init > DateTime.UtcNow).ToList();
            }
        }

        public void StoreEvents(List<BE.SportEvent> updatedEvents)
        {
            using (var dbcontext = new BE.ADOLDBEntities())
            {
                try
                {
                    foreach (var updatedEvent in updatedEvents)
                    {
                        if (dbcontext.SportEvents.Any(p => p.Code.Equals(updatedEvent.Code)))
                        {
                            var storedEvent = dbcontext.SportEvents.Where(p => p.Code.Equals(updatedEvent.Code)).First();
                            foreach (var bet in updatedEvent.SportBets)
                            {
                                var storedOdd = storedEvent.SportBets.Where(p => p.Code.Equals(bet.Code)).FirstOrDefault();
                                if (storedOdd != null)
                                {
                                    storedOdd.Odd1 = bet.Odd1;
                                    storedOdd.Odd2 = bet.Odd2;
                                    storedOdd.Odd3 = bet.Odd3;
                                }
                                else
                                {
                                    storedEvent.SportBets.Add(bet);
                                }
                            }

                            storedEvent = updatedEvent;
                            //storedEvent.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
                        }
                        else
                        {
                            foreach (var singleBet in updatedEvent.SportBets)
                            {
                                dbcontext.SportBets.Add(singleBet);
                            }

                            //evento.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
                            updatedEvent.Active = true;
                            dbcontext.SportEvents.Add(updatedEvent);
                        }
                    }

                    dbcontext.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                    //this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    //return View();
                }
                catch (Exception exn)
                {
                    StringBuilder sb = new StringBuilder();
                    var innerException = AddInnerException(exn, sb);
                    while (innerException != null)
                    {
                        innerException = AddInnerException(innerException, sb);
                    }
                    //var errors = sb.ToString();
                    throw new Exception(sb.ToString());
                }
            }
        }

        public List<BE.Sport> GetActiveSports()
        {
            using (var db = new BE.ADOLDBEntities())
            {
                return db.Sports.Where(p => p.Active).ToList();
            }
        }

        public List<BE.Sport> GetActiveSports(string code)
        {
            return this.GetActiveSports().Where(p => p.Code.Equals(code)).ToList();
        }

        public List<BE.SportEvent> GetSportEvents(string sportCode)
        {
            List<BE.SportEvent> returnValue = new List<BE.SportEvent>();
            using (var db = new BE.ADOLDBEntities())
            {
                BE.SportEvent sportEvent = new BE.SportEvent();
                var events = db.Sports.Where(p => p.Code.Equals(sportCode)).First().SportEvents.ToList();
                foreach (var singleEvent in events)
                {
                    sportEvent = singleEvent;
                    sportEvent.SportBets = singleEvent.SportBets;
                    returnValue.Add(sportEvent);
                }
            }
            return returnValue;
        }

        public BE.SportBet GetSportEvent(int sportBetID)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                return db.SportBets.Include(s => s.SportEvent).Where(p => p.ID.Equals(sportBetID)).First();
            }
        }

        public BE.SportEvent GetSportEvent(string eventCode)
        {
            BE.SportEvent returnValue = new BE.SportEvent();

            using (var db = new BE.ADOLDBEntities())
            {
                returnValue = db.SportEvents.First(e => e.Code == eventCode);
            }
            return returnValue;
        }

        public List<BE.SportEvent> GetEvents(string leagueId)
        {
            List<BE.SportEvent> returnValue = new List<BE.SportEvent>();
            using (var db = new BE.ADOLDBEntities())
            {
                BE.SportEvent sportEvent = new BE.SportEvent();
                var sports = db.Sports.ToList();
                var events = db.Sports.Where(p => p.League.Equals(leagueId)).First().SportEvents.ToList();
                foreach (var singleEvent in events)
                {
                    sportEvent = singleEvent;
                    sportEvent.SportBets = singleEvent.SportBets;
                    returnValue.Add(sportEvent);
                }
            }
            return returnValue;
        }

        public List<BE.SportEvent> GetEventsByTournament(string tournamentId)
        {
            List<BE.SportEvent> returnValue = new List<BE.SportEvent>();
            using (var db = new BE.ADOLDBEntities())
            {
                BE.SportEvent sportEvent = new BE.SportEvent();
                var sports = db.Sports.ToList();
                var events = db.Sports.Where(p => p.TournamentID.Equals(tournamentId)).First().SportEvents.ToList();
                foreach (var singleEvent in events)
                {
                    sportEvent = singleEvent;
                    sportEvent.SportBets = singleEvent.SportBets;
                    returnValue.Add(sportEvent);
                }
            }
            return returnValue;
        }

        public List<BE.SportBet> GetEventOdd(string matchID)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                return db.SportBets.Where(p => p.SportEvent.Code.Equals(matchID)).ToList();
            }
        }

        public BE.SportBet GetSportBet(int sportBetID)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                return db.SportBets.Include(s => s.SportEvent).Where(p => p.ID.Equals(sportBetID)).First();
            }
        }

        private Exception AddInnerException(Exception ex, StringBuilder sb)
        {
            sb.AppendLine(ex.Message + "\n");
            return ex.InnerException;
        }
    }
}
