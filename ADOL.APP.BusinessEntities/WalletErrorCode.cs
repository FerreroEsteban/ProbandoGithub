using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public enum WalletErrorCode
    {
        Success = 0,
        Unexpected = 1,
        NoFunds = 2,
        BetLimit = 3,
        UnknownToken = 4,
        InvalidUser = 5,
        LockedUser = 6,
        MissingTransaction = 7,
        Timeout = 8,
        InvalidAmount = 9
    }
}
