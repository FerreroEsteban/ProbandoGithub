using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using System.Globalization;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class UserBetAccess
    {
        public void AddUserBet(string userToken, int sportBetID, float amount)
        { 
            using(var db = new ADOLDBEntities())
            {
                ApuestasDeUsuario au = new ApuestasDeUsuario();
                au.ApuestaDeportivaID = sportBetID;
                au.Token = userToken;
                au.Amount = decimal.Parse(amount.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    
                db.ApuestasDeUsuarios.Add(au);
                db.SaveChanges();
            }
        }

        public List<ApuestasDeUsuario> GetUserBets(string userToken)
        {
            using (var db = new ADOLDBEntities())
            {
                return db.ApuestasDeUsuarios.Where(p => p.Token.Equals(userToken)).ToList();
            }
        }
    }
}
