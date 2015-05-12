using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public partial class UserBet
    {
        public IOddProvider GetOddProvider()
        {
            return OddProviderFactory.GetOddProvider(this.SportBet.Code);
        }

        public IOddProvider GetOddProvider(string oddType)
        {
            return OddProviderFactory.GetOddProvider(oddType);
        }
    }
}
