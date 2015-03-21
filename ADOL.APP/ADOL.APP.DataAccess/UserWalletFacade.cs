using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess
{
    public static class UserWalletFacade
    {
        public static bool ValidateFundsAvailable(string userToken, decimal amount)
        {
            return true;
        }
    }
}
