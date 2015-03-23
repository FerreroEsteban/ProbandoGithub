using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Globalization;
using System.Transactions;
using System.Data.Entity;
using System.Data.Objects;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class UserBetAccess
    {
        public void AddUserBet(string userToken, bool isLinked,List<Tuple<int,decimal,string>> bets)
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var linkID = Guid.NewGuid();
                    foreach (var bet in bets)
                    {
                        
                        var apuestasDeportivas = db.ApuestasDeportivas.Where(p => p.ID.Equals(bet.Item1)).First();
                        var betPrice = apuestasDeportivas.GetOddPrice(bet.Item3);

                        BE.ApuestasDeUsuario au = new BE.ApuestasDeUsuario();
                        au.ApuestaDeportivaID = bet.Item1;
                        au.Token = userToken;
                        au.Amount = bet.Item2;
                        au.BetType = bet.Item3;
                        au.BetPrice = betPrice;
                        au.Linked = isLinked ? linkID.ToString() : null;

                        db.ApuestasDeUsuarios.Add(au);
                        db.SaveChanges();
                    }
                    scope.Complete();
                }
            }
        }

        public List<BE.ApuestasDeUsuario> GetUserBets(string userToken)
        {
            List<BE.ApuestasDeUsuario> returnValue = new List<BE.ApuestasDeUsuario>();
            using (var db = new BE.ADOLAPPDBEntities())
            {
                var apuestas = db.ApuestasDeUsuarios.Where(p => p.Token.Equals(userToken)).ToList();
                foreach (var apuesta in apuestas)
                {
                    BE.ApuestasDeUsuario item = new BE.ApuestasDeUsuario();
                    var sportOdd = apuesta.ApuestasDeportiva;

                    item = apuesta;
                    item.ApuestasDeportiva = sportOdd;
                    returnValue.Add(item);
                }
            }
            return returnValue;
        }
    }
}
