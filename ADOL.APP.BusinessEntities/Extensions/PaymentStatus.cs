using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public enum PaymentStatus
    {
        Pending = 0,
        withoutInformation = 1,
        NoHit = 2,
        Return = 3,
        PayBack = 4,
        Returned = 5,
        PayedBack = 6
    }
}
