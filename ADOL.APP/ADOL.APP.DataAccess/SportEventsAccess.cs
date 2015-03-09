using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class SportEventsAccess
    {
        public List<EventosDeportivo> GetCurrentEvents()
        {
            using (var dbcontext = new ADOLDBEntities())
            {
                return dbcontext.EventosDeportivos.Where(p => p.Activo && p.Inicio > DateTime.UtcNow).ToList();
            }
        }

        public void StoreEvents(List<EventosDeportivo> eventosActualizados)
        {
            using (var dbcontext = new ADOLDBEntities())
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
                                var storedOdd = storedEvent.ApuestasDeportivas.Where(p => p.TipoApuesta.Equals(apuesta.TipoApuesta)).FirstOrDefault();
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
                            storedEvent.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
                        }
                        else
                        {
                            foreach (var apuesta in evento.ApuestasDeportivas)
                            {
                                dbcontext.ApuestasDeportivas.Add(apuesta);
                            }

                            evento.Deporte = dbcontext.Deportes.Where(p => p.Codigo.Equals("1")).First();
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

        public List<Deporte> GetActiveSports()
        {
            using (var db = new ADOLDBEntities())
            {
                return db.Deportes.Where(p => p.Activo).ToList();
            }
        }

        public List<EventosDeportivo> GetSportEvent(string sportCode)
        {
            List<EventosDeportivo> returnValue = new List<EventosDeportivo>();
            using (var db = new ADOLDBEntities())
            {
                EventosDeportivo sportEvent = new EventosDeportivo();
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

        private Exception AddInnerException(Exception ex, StringBuilder sb)
        {
            sb.AppendLine(ex.Message + "\n");
            return ex.InnerException;
        }
    }
}
