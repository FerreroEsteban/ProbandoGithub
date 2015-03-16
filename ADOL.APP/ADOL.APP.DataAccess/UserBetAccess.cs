using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Globalization;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class UserBetAccess
    {
        public void AddUserBet(string userToken, int sportBetID, float amount, string betType)
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                var apuestasDeportivas = db.ApuestasDeportivas.Where(p => p.ID.Equals(sportBetID)).First();
                var betPrice = apuestasDeportivas.GetOddPrice(betType);

                BE.ApuestasDeUsuario au = new BE.ApuestasDeUsuario();
                au.ApuestaDeportivaID = sportBetID;
                au.Token = userToken;
                au.Amount = amount;
                au.BetType = betType;
                au.BetPrice = betPrice;
                    
                db.ApuestasDeUsuarios.Add(au);
                db.SaveChanges();
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
