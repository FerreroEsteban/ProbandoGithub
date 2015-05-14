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
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class BetManager : BaseManager
    {
        private SportEventsAccess sea = new SportEventsAccess();
        
        public BetManager()
            : base()
        { 
            //do nothing
        }
        
        public BE.BaseResponse<bool> AddUserBet(string userToken, int BetType, List<Tuple<int, decimal, string>> bets)
        {
            var user = this.GetSessionUser();

            decimal amountToValidte = BetType > 0 ? bets[0].Item2 : bets.Sum(p => p.Item2);
            BE.BaseResponse<bool> returnData;

            try
            {
                if (BetType > 0)
                {
                    returnData = ProcessCombinedBets(amountToValidte, bets, user);
                }
                else
                {
                    returnData = ProcessSingleBets(bets, user);
                }
                returnData = new BE.BaseResponse<bool>(false, BE.ResponseStatus.Fail, "No hay fondos suficientes para la operacion");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Format("error adding user bet. Type: {0}", BetType > 0 ? "Combined" : "Single"), ex);
                RequestContextHelper.LastError = ex.Message;
                returnData = new BE.BaseResponse<bool>(false, BE.ResponseStatus.Fail, ex.Message);
            }
            
            return returnData;
        }

        private BE.BaseResponse<bool> ProcessSingleBets(List<Tuple<int, decimal, string>> bets, BE.User user)
        {
            Dictionary<BE.UserBet, BE.BaseWalletResponseData> userBets = new Dictionary<BE.UserBet, BE.BaseWalletResponseData>();
            //SportEventsAccess sea = new SportEventsAccess();
            UserBetAccess uba = new UserBetAccess();
            var withErrors = false;
            foreach (var bet in bets)
            {
                var sportbet = sea.GetSportBet(bet.Item1);
                var userbet = new BE.UserBet();
                userbet.UserID = user.ID;
                userbet.SportBetID = sportbet.ID;
                userbet.MatchCode = sportbet.SportEvent.Code;
                userbet.MatchName = sportbet.SportEvent.Name;
                userbet.PaymentStatus = null;
                userbet.Hit = null;
                userbet.TransactionID = Guid.NewGuid().ToString();
                userbet.LinkedCode = null;
                var oddProvider = userbet.GetOddProvider(sportbet.Code);
                userbet.BetType = bet.Item3;
                userbet.Amount = bet.Item2;
                userbet.BetPrice = oddProvider.GetOddValue(bet.Item3, sportbet);

                string detail = GetDebitDetail(userbet, user);

                BE.DebitRequest req = new BE.DebitRequest();
                req.Amount = userbet.Amount;
                req.BetDetail = detail;
                req.EventID = userbet.MatchCode;
                req.EventName = userbet.MatchName;
                req.SessionToken = user.SessionToken;
                req.UserUID = user.UID;
                req.TransactionID = userbet.TransactionID;

                LogHelper.LogActivity("single bet request created", req);
                
                var betValidated = BetValidatorExtension.ValidateBet(sportbet.SportEvent.Sport.Code, userbet.Amount, userbet.BetPrice, user.Balance);
                LogHelper.LogActivity("Is validated: {0}", (betValidated.Status.Equals(ResponseStatus.OK) && betValidated.GetData()).ToString());
                if (betValidated.Status.Equals(ResponseStatus.OK) && betValidated.GetData())
                {
                    BE.BaseResponse<BE.BaseWalletResponseData> response = UserWalletFacade.ProcessBetDebit(req);
                    LogHelper.LogActivity(string.Format("Debit response status: {0}", response.Status.ToString()), response.GetData());
                    userBets.Add(userbet, response.GetData());
                    if (response.Status.Equals(BE.ResponseStatus.Fail))
                    {
                        RequestContextHelper.LastError = response.Message;
                        withErrors = true;
                        break;
                    }

                    RequestContextHelper.SessionToken = response.GetData().SessionToken;
                    RequestContextHelper.UserBalance = response.GetData().Balance;
                    RequestContextHelper.UserName = response.GetData().NickName;

                    user.SessionToken = response.GetData().SessionToken;
                    user.Balance = response.GetData().Balance;
                }
                else
                {
                    return new BaseResponse<bool>(false, ResponseStatus.Fail, betValidated.Message);
                }
            }

            if (withErrors)
            {
                LogHelper.LogActivity("Rollback withErrors", userBets);
                new Thread(delegate()
                {
                    DoRollBack(userBets);
                }).Start();
            }
            else
            {
                if (!uba.StoreUserBet(userBets.Keys.ToList()))
                {
                    LogHelper.LogActivity("Rollback StoreUserBet", userBets);
                    new Thread(delegate()
                    {
                        DoRollBack(userBets);
                    }).Start();
                    return new BE.BaseResponse<bool>(false, BE.ResponseStatus.Fail, "No se pudo guardar la apuesta por un error interno");
                }
            }

            UserAccess ua = new UserAccess();
            user = ua.UpdateUser(user);

            return new BE.BaseResponse<bool>(true, BE.ResponseStatus.OK);
        }

        private void DoRollBack(Dictionary<BE.UserBet, BE.BaseWalletResponseData> userBets)
        {
            List<BE.BaseRequest> operations = new List<BE.BaseRequest>();
            BE.BaseRequest req = null;
            foreach (var userbet in userBets)
            {
                if (userbet.Key.LinkedCode == null || (userbet.Key.LinkedCode != null && userbet.Key.Amount > 0))
                {
                    req = new BE.BaseRequest();
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

        private BE.BaseResponse<bool> ProcessCombinedBets(decimal betAmount, List<Tuple<int, decimal, string>> bets, BE.User user)
        {
            List<BE.UserBet> userBets = new List<BE.UserBet>();
            //SportEventsAccess sea = new SportEventsAccess();
            string transaction = Guid.NewGuid().ToString();
            decimal totalBetPrice = 1M;
            List<string> sportCodes = new List<string>();
            foreach (var bet in bets)
            {
                var sportbet = sea.GetSportBet(bet.Item1);
                var userbet = new BE.UserBet();
                userbet.UserID = user.ID;
                userbet.SportBetID = sportbet.ID;
                userbet.MatchCode = sportbet.SportEvent.Code;
                userbet.MatchName = sportbet.SportEvent.Name;
                userbet.PaymentStatus = null;
                userbet.Hit = null;
                userbet.TransactionID = transaction;
                userbet.LinkedCode = transaction;
                var oddProvider = userbet.GetOddProvider(sportbet.Code);
                userbet.BetType = bet.Item3;
                userbet.Amount = userBets.Count > 0 ? 0M : betAmount;

                totalBetPrice *= oddProvider.GetOddValue(bet.Item3, sportbet);

                userBets.Add(userbet);

                sportCodes.Add(sportbet.SportEvent.Sport.Code);
            }

            userBets.ForEach(p => p.BetPrice = totalBetPrice);

            string detail = GetDebitDetail(userBets, user);

            BE.DebitRequest req = new BE.DebitRequest();
            req.Amount = userBets[0].Amount;
            req.BetDetail = detail;
            req.EventID = string.Join(",", userBets.Select(p => p.MatchCode).ToArray());
            req.EventName = string.Join(",", userBets.Select(p => p.MatchName).ToArray());
            req.SessionToken = user.SessionToken;
            req.UserUID = user.UID;
            req.TransactionID = userBets[0].TransactionID;
            LogHelper.LogActivity("Request created", req);
            Dictionary<BE.UserBet, BE.BaseWalletResponseData> combinedBet = new Dictionary<BE.UserBet, BE.BaseWalletResponseData>();

            var validatedBet = BetValidatorExtension.ValidateBet(sportCodes.ToArray(), totalBetPrice, betAmount, user.Balance);
            LogHelper.LogActivity(string.Format("Is bet validated: {0}", (validatedBet.Status.Equals(ResponseStatus.OK) && validatedBet.GetData()).ToString()));
            if (validatedBet.Status.Equals(ResponseStatus.OK) && validatedBet.GetData())
            {

                BE.BaseResponse<BE.BaseWalletResponseData> response = UserWalletFacade.ProcessBetDebit(req);
                LogHelper.LogActivity(string.Format("debit response status: {0}", response.Status.ToString()), response.GetData());
                if (response.Status.Equals(BE.ResponseStatus.Fail))
                {
                    RequestContextHelper.LastError = response.Message;

                    userBets.ForEach(ub => combinedBet.Add(ub, response.GetData()));
                    new Thread(delegate()
                    {
                        LogHelper.LogActivity("Rollback wallet");
                        DoRollBack(combinedBet);
                    }).Start();
                    return new BE.BaseResponse<bool>(false, BE.ResponseStatus.Fail, response.Message);
                }
                UserBetAccess uba = new UserBetAccess();
                if (!uba.StoreUserBet(userBets))
                {
                    LogHelper.LogActivity("rollback storing bets", userBets);
                    new Thread(delegate()
                    {
                        DoRollBack(combinedBet);
                    }).Start();
                    return new BE.BaseResponse<bool>(false, BE.ResponseStatus.Fail, "No se pudo guardar la puesta por un error interno");
                }

                LogHelper.LogActivity("bets processed", response.GetData());

                RequestContextHelper.SessionToken = response.GetData().SessionToken;
                RequestContextHelper.UserBalance = response.GetData().Balance;
                RequestContextHelper.UserName = response.GetData().NickName;


                user.SessionToken = response.GetData().SessionToken;
                user.Balance = response.GetData().Balance;

                UserAccess ua = new UserAccess();
                user = ua.UpdateUser(user);

                return new BE.BaseResponse<bool>(true, BE.ResponseStatus.OK);
            }
            else
            {
                return new BaseResponse<bool>(false, ResponseStatus.Fail, validatedBet.Message);
            }
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

        private string GetDebitDetail(BE.UserBet userbet, BE.User user)
        {
            XElement detail = new XElement("BetDetail",
                new XElement("BetType", "single"),
                new XElement("UserUID", user.UID),
                new XElement("TransactionID", userbet.TransactionID),
                new XElement("MatchCode", userbet.MatchCode),
                new XElement("MatchName", userbet.MatchName),
                new XElement("BetName", userbet.BetType),
                new XElement("BetPrice", userbet.BetPrice),
                new XElement("BetAmount", userbet.Amount),
                new XElement("Date", DateTime.UtcNow)
                );

            return detail.ToString(SaveOptions.DisableFormatting);
        }

        private string GetDebitDetail(List<BE.UserBet> userbets, BE.User user)
        {
            XElement detail = new XElement("BetDetail",
                    new XElement("BetType", "combined"),
                    new XElement("UserUID", user.UID),
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
            return detail.ToString(SaveOptions.DisableFormatting);
        }

        public BE.BaseResponse<List<BE.UserBet>> GetUserBets(string userToken)
        {
            UserBetAccess uba = new UserBetAccess();
            try
            {
                return new BE.BaseResponse<List<BE.UserBet>>(uba.GetUserBets(userToken), BE.ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                RequestContextHelper.LastError = ex.Message;
                return new BE.BaseResponse<List<BE.UserBet>>(new List<BE.UserBet>(), BE.ResponseStatus.Fail, ex.Message);
            }
        }
    }
}
