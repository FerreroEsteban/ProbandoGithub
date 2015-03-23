using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class BetManager
    {
        public bool AddUserBet(string userToken, int BetType, List<Tuple<int, decimal, string>> bets)
        {
            decimal amountToValidte = 0;
            bets.ForEach(p => amountToValidte += p.Item2);
            try
            {
                if (UserWalletFacade.ValidateFundsAvailable(userToken, amountToValidte))
                {
                    UserBetAccess uba = new UserBetAccess();
                    bets.ForEach(p => uba.AddUserBet(userToken, BetType > 0, bets));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<BE.ApuestasDeUsuario> GetUserBets(string userToken)
        {
            UserBetAccess uba = new UserBetAccess();
            return uba.GetUserBets(userToken);
        }
    }
}
