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
        public void AddUserBet(string userToken, int sportBetID, float amount)
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                BE.ApuestasDeUsuario au = new BE.ApuestasDeUsuario();
                au.ApuestaDeportivaID = sportBetID;
                au.Token = userToken;
                au.Amount = amount;
                    
                db.ApuestasDeUsuarios.Add(au);
                db.SaveChanges();
            }
        }

        public List<BE.ApuestasDeUsuario> GetUserBets(string userToken)
        {
            using (var db = new BE.ADOLAPPDBEntities())
            {
                return db.ApuestasDeUsuarios.Where(p => p.Token.Equals(userToken)).ToList();
            }
        }
    }
}
