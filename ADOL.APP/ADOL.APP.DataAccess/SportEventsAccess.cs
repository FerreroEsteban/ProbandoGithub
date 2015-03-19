using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class SportEventsAccess
    {
        public List<BE.EventosDeportivo> GetCurrentEvents()
        {
            using (var dbcontext = new BE.ADOLAPPDBEntities())
            {
                return dbcontext.EventosDeportivos.Where(p => p.Activo && p.Inicio > DateTime.UtcNow).ToList();
            }
        }

        public void StoreEvents(List<BE.EventosDeportivo> eventosActualizados)
        {
            using (var dbcontext = new BE.ADOLAPPDBEntities())
            {
                try
                {
                    foreach (var evento in eventosActualizados)
                    {
                        if (dbcontext.EventosDeportivos.Any(p => p.Codigo.Equals(evento.Codigo)))
                        {
                            var storedEvent = dbcontext.EventosDeportivos.Where(p => p.Codigo.Equals(evento.Codigo)).First();
                            foreach (var apuesta in evento.ApuestasDeportivas)
                            {
                                var storedOdd = storedEvent.ApuestasDeportivas.Where(p => p.Codigo.Equals(apuesta.Codigo)).FirstOrDefault();
                                if (storedOdd != null)
                                {
                                    storedOdd.Odd1 = apuesta.Odd1;
                                    storedOdd.Odd2 = apuesta.Odd2;
                                    storedOdd.Odd3 = apuesta.Odd3;
                                }
                                else
                                {
                                    storedEvent.ApuestasDeportivas.Add(apuesta);
                                }
                            }
                            
                            storedEvent = evento;
                            //storedEvent.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
                        }
                        else
                        {
                            foreach (var apuesta in evento.ApuestasDeportivas)
                            {
                                dbcontext.ApuestasDeportivas.Add(apuesta);
                            }

                            //evento.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
                            evento.Activo = true;
                            dbcontext.EventosDeportivos.Add(evento);
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
                    var pepe = AddInnerException(exn, sb);
                    while (pepe != null)
                    {
                        pepe = AddInnerException(pepe, sb);
                    }
                    var errroes = sb.ToString();
                }
            }
        }

        public List<BE.Deporte> GetActiveSports()
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                return db.Deportes.Where(p => p.Activo).ToList();
            }
        }

        public List<BE.EventosDeportivo> GetSportEvent(string sportCode)
        {
            List<BE.EventosDeportivo> returnValue = new List<BE.EventosDeportivo>();
            using (var db = new BE.ADOLAPPDBEntities())
            {
                BE.EventosDeportivo sportEvent = new BE.EventosDeportivo();
                var deportes = db.Deportes.ToList();
                var eventos = db.Deportes.Where(p => p.Codigo.Equals(sportCode)).First().EventosDeportivos.ToList();
                foreach (var evento in eventos)
                {
                    sportEvent = evento;
                    sportEvent.ApuestasDeportivas = evento.ApuestasDeportivas;
                    returnValue.Add(sportEvent);
                }
            }
            return returnValue;
        }

        public List<BE.ApuestasDeportiva> GetEventOdd(string matchID)
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                return db.ApuestasDeportivas.Where(p => p.EventosDeportivo.Codigo.Equals(matchID)).ToList();
            }
        }

        private Exception AddInnerException(Exception ex, StringBuilder sb)
        {
            sb.AppendLine(ex.Message + "\n");
            return ex.InnerException;
        }
    }
}
