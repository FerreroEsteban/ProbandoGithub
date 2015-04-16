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
        public bool StoreUserBet(List<BE.UserBet> bets)
        {
            try
            {
                using (var db = new BE.ADOLDBEntities())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var linkID = Guid.NewGuid();
                        foreach (var bet in bets)
                        {
                            db.UserBets.Add(bet);
                            db.SaveChanges();
                        }
                        scope.Complete();
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                //do something with ex
                return false;
            }
        }

        public List<BE.UserBet> GetUserBets(string userToken)
        {
            List<BE.UserBet> returnValue = new List<BE.UserBet>();
            using (var db = new BE.ADOLDBEntities())
            {
                var userBets = db.UserBets.Where(p => p.User.SessionToken.Equals(userToken)).ToList();
                foreach (var userBet in userBets)
                {
                    BE.UserBet item = new BE.UserBet();
                    var sportOdd = userBet.SportBet;

                    item = userBet;
                    item.SportBet = sportOdd;
                    returnValue.Add(item);
                }
            }
            return returnValue;
        }

        public List<Tuple<string,BE.UserBet>> GetPendings(string[] events)
        {
            List<Tuple<string, BE.UserBet>> returnValue = new List<Tuple<string, BE.UserBet>>();
            using (var db = new BE.ADOLDBEntities())
            {
                var bet = db.UserBets.Where(p => events.Contains(p.SportBet.SportEvent.Code) && p.Hit == null).ToList();
                if (bet != null)
                {
                    bet.ForEach(p => returnValue.Add(new Tuple<string, BE.UserBet>(p.SportBet.SportEvent.Code, p)));
                }
            }
            return returnValue;
        }

        public void UpdateUserBetStatus(int id, bool status)
        {
            using (var db = new ADOLDBEntities())
            {
                var userBet = db.UserBets.Where(p => p.ID == id).First();
                userBet.Hit = status;
                db.SaveChanges();
            }
        }
    }
}
