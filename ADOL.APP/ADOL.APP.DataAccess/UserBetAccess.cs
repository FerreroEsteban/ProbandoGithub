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
            using (var db = new BE.ADOLDBEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var linkID = Guid.NewGuid();
                    foreach (var bet in bets)
                    {
                        
                        var sportBet = db.SportBets.Where(p => p.ID.Equals(bet.Item1)).First();
                        var betPrice = sportBet.GetOddPrice(bet.Item3);

                        BE.UserBet au = new BE.UserBet();
                        au.SportBetID = bet.Item1;
                        au.Token = userToken;
                        au.Amount = bet.Item2;
                        au.BetType = bet.Item3;
                        au.BetPrice = betPrice;
                        au.LinkedCode = isLinked ? linkID.ToString() : null;
                        au.MatchCode = sportBet.SportEvent.Code;

                        db.UserBets.Add(au);
                        db.SaveChanges();
                    }
                    scope.Complete();
                }
            }
        }

        public List<BE.UserBet> GetUserBets(string userToken)
        {
            List<BE.UserBet> returnValue = new List<BE.UserBet>();
            using (var db = new BE.ADOLDBEntities())
            {
                var userBets = db.UserBets.Where(p => p.Token.Equals(userToken)).ToList();
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
