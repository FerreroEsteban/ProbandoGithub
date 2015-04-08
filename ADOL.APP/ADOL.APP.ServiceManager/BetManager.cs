using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Xml.Linq;
using System.Threading;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class BetManager
    {
        public bool AddUserBet(string userToken, int BetType, List<Tuple<int, decimal, string>> bets)
        {
            UserAccess ua = new UserAccess();
            var user = ua.GetUser(userToken);
            decimal amountToValidte = bets.Sum(p => p.Item2);

            try
            {
                if (UserWalletFacade.ValidateFundsAvailable(user, amountToValidte))
                {
                    if (BetType > 0)
                    {
                        return ProcessCombinedBets(amountToValidte, bets, user);
                    }
                    else
                    {
                        return ProcessSingleBets(bets, user);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ProcessSingleBets(List<Tuple<int, decimal, string>> bets, BE.User user)
        {
            Dictionary<BE.UserBet,BE.BaseWalletResponseData> userBets = new Dictionary<BE.UserBet,BE.BaseWalletResponseData>();
            SportEventsAccess sea = new SportEventsAccess();
            UserBetAccess uba = new UserBetAccess();
            foreach (var bet in bets)
            {
                var sportbet = sea.GetSportBet(bet.Item1);
                var userbet = new BE.UserBet();
                userbet.User = user;
                userbet.SportBet = sportbet;
                userbet.MatchCode = sportbet.SportEvent.Code;
                userbet.MatchName = sportbet.SportEvent.Name;
                userbet.PaymentStatus = null;
                userbet.Hit = null;
                userbet.TransactionID = Guid.NewGuid().ToString();
                userbet.LinkedCode = null;
                var oddProvider = userbet.GetOddProvider();
                userbet.BetType = bet.Item3;
                userbet.Amount = bet.Item2;
                userbet.BetPrice = oddProvider.GetOddValue(bet.Item3, sportbet);

                string detail = GetDebitDetail(userbet);

                BE.DebitRequest req = new BE.DebitRequest();
                req.Amount = userbet.Amount;
                req.BetDetail = detail;
                req.EventID = int.Parse(userbet.MatchCode);
                req.EventName = userbet.MatchName;
                req.SessionToken = user.SessionToken;
                req.UserUID = user.UID;
                req.TransactionID = userbet.TransactionID;

                BE.BaseResponse<BE.BaseWalletResponseData> response = UserWalletFacade.ProcessBetDebit(req);
                if (response.Status.Equals(BE.ResponseStatus.OK))
                {
                    userBets.Add(userbet, response.GetData());
                }
                else
                { 
                    //Todo: log defect
                    new Thread(delegate()
                    {
                        DoRollBack(userBets);
                    }).Start();
                    return false;
                }
            }

            if (!uba.StoreUserBet(userBets.Keys.ToList()))
            {
                new Thread(delegate()
                {
                    DoRollBack(userBets);
                }).Start();
                return false;
            }
            return true;
        }

        private void DoRollBack(Dictionary<BE.UserBet, BE.BaseWalletResponseData> userBets)
        {
            List<BE.BaseRequest> operations = new List<BE.BaseRequest>();
            BE.BaseRequest req = null;
            foreach (var userbet in userBets)
            {
                if (userbet.Key.LinkedCode == null || (userbet.Key.LinkedCode != null && userbet.Key.Amount > 0))
                {
                    req.SessionToken = userbet.Value.SessionToken;
                    req.UserUID = userbet.Value.UserUID;
                    req.TransactionID = userbet.Value.TransactionID;
                    req.Amount = userbet.Key.Amount;

                    bool responseOK = false;
                    int i = 1;
                    while (responseOK || i > 3)
                    {
                        var resp = UserWalletFacade.ProcessRollback(req);
                        if (resp.Status.Equals(BE.ResponseStatus.OK))
                        {
                            responseOK = true;
                        }
                        if (!responseOK)
                        {
                            Thread.Sleep(30000);
                        }
                    }
                }
            }
        }

        private bool ProcessCombinedBets(decimal betAmount, List<Tuple<int, decimal, string>> bets, BE.User user)
        {
            List<BE.UserBet> userBets = new List<BE.UserBet>();
            SportEventsAccess sea = new SportEventsAccess();
            string transaction = Guid.NewGuid().ToString();
            decimal totalBetPrice = 1M;
            foreach (var bet in bets)
            {
                var sportbet = sea.GetSportBet(bet.Item1);
                var userbet = new BE.UserBet();
                userbet.User = user;
                userbet.SportBet = sportbet;
                userbet.MatchCode = sportbet.SportEvent.Code;
                userbet.MatchName = sportbet.SportEvent.Name;
                userbet.PaymentStatus = null;
                userbet.Hit = null;
                userbet.TransactionID = transaction;
                userbet.LinkedCode = transaction;
                var oddProvider = userbet.GetOddProvider();
                userbet.BetType = bet.Item3;
                userbet.Amount = userBets.Count > 0 ? 0M : betAmount;

                totalBetPrice *= oddProvider.GetOddValue(bet.Item3, sportbet);
            }

            userBets.ForEach(p => p.BetPrice = totalBetPrice);

            string detail = GetDebitDetail(userBets);

            BE.DebitRequest req = new BE.DebitRequest();
            req.Amount = userBets[0].Amount;
            req.BetDetail = detail;
            //req.EventID = int.Parse(userbet.MatchCode);
            //req.EventName = userbet.MatchName;
            req.SessionToken = user.SessionToken;
            req.UserUID = user.UID;
            req.TransactionID = userBets[0].TransactionID;

            Dictionary<BE.UserBet, BE.BaseWalletResponseData> combinedBet = new Dictionary<BE.UserBet, BE.BaseWalletResponseData>();

            BE.BaseResponse<BE.BaseWalletResponseData> response = UserWalletFacade.ProcessBetDebit(req);
            if (response.Status.Equals(BE.ResponseStatus.Fail))
            {
                //userBets.Add(userbet, response.GetData());
                
                userBets.ForEach(ub => combinedBet.Add(ub, response.GetData()));
                new Thread(delegate()
                {
                    DoRollBack(combinedBet);
                }).Start();
                return false;
            }
            UserBetAccess uba = new UserBetAccess();
            if (!uba.StoreUserBet(userBets))
            {
                new Thread(delegate()
                {
                    DoRollBack(combinedBet);
                }).Start();
                return false;
            }
            return true;
        }

        private decimal GetBetAmount(int betType, int userBetsCount, decimal amountToValidate, decimal singlBetAmount)
        {
            if (betType > 0 && userBetsCount < 1)
            {
                return amountToValidate;
            }

            if (betType > 0 && userBetsCount > 0)
            {
                return 0M;
            }

            return singlBetAmount;
        }

        private string GetDebitDetail(BE.UserBet userbet)
        {
            XElement detail = new XElement("BetDetail",
                new XElement("BetType", "single"),
                new XElement("UserUID", userbet.User.UID),
                new XElement("TransactionID", userbet.TransactionID),
                new XElement("MatchCode", userbet.MatchCode),
                new XElement("MatchName", userbet.MatchName),
                new XElement("BetName", userbet.BetType),
                new XElement("BetPrice", userbet.BetPrice),
                new XElement("BetAmount", userbet.Amount),
                new XElement("Date", DateTime.UtcNow)
                );

            return detail.ToString();
        }

        private string GetDebitDetail(List<BE.UserBet> userbets)
        {
            XElement detail = new XElement("BetDetail",
                    new XElement("BetType", "combined"),
                    new XElement("UserUID", userbets[0].User.UID),
                    new XElement("TransactionID", userbets[0].TransactionID),
                    new XElement("BetAmount", userbets[0].Amount),
                    new XElement("BetPrice", userbets[0].BetPrice),
                    new XElement("Date", DateTime.UtcNow));

            XElement eventsDetail = new XElement("EventsDetails");

            foreach (var userbet in userbets)
            {
               XElement matchDetails = new XElement("MatchDetail", 
                    new XElement("MatchCode", userbet.MatchCode),
                    new XElement("MatchName", userbet.MatchName),
                    new XElement("BetName", userbet.BetType)
                    );

               eventsDetail.Add(matchDetails);
            }

            detail.Add(eventsDetail);
            return detail.ToString();
        }

        public List<BE.UserBet> GetUserBets(string userToken)
        {
            UserBetAccess uba = new UserBetAccess();
            return uba.GetUserBets(userToken);
        }
    }
}
