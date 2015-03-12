using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.DataAccess;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class BetManager
    {
        public bool AddUserBet(string userToken, int sportBetID, float amount)
        {
            try
            {
                if (UserWalletFacade.ValidateFundsAvailable(userToken, amount))
                {
                    UserBetAccess uba = new UserBetAccess();

                    uba.AddUserBet(userToken, sportBetID, amount);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ApuestasDeUsuario> GetUserBets(string userToken)
        {
            UserBetAccess uba = new UserBetAccess();
            return uba.GetUserBets(userToken);
        }
    }
}
