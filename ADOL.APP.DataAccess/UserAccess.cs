using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.CurrentAccountService.DataAccess.DBAccess
{
    public class UserAccess
    {
        public BE.User GetUser(string sessionToken)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                return db.Users.Where(p => p.SessionToken.Equals(sessionToken)).FirstOrDefault();
            }
        }

        public BE.User LoginUser(BE.User userData)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                if (db.Users.Any(p => p.UID.Equals(userData.UID)))
                {
                    var user = db.Users.Where(p => p.UID.Equals(userData.UID)).First();
                    user.LaunchToken = userData.LaunchToken;
                    user.SessionToken = userData.SessionToken;
                    user.Balance = userData.Balance;
                    db.Users.Attach(user);
                    db.Entry(user).State = System.Data.EntityState.Modified;
                }
                else
                {
                    db.Users.Add(userData);
                }
                db.SaveChanges();
            }
            return userData;
        }

        public BE.User UpdateUser(BE.User userData)
        {
            using (var db = new BE.ADOLDBEntities())
            {
                var user = db.Users.Where(p => p.UID.Equals(userData.UID)).First();
                user.LaunchToken = userData.LaunchToken;
                user.SessionToken = userData.SessionToken;
                user.Balance = userData.Balance;
                db.Users.Attach(user);
                db.Entry(user).State = System.Data.EntityState.Modified;

                db.SaveChanges();
            }
            return userData;
        }
    }
}

