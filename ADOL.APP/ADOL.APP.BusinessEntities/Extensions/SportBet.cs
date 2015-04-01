using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public partial class SportBet
    {
        private IOddProvider oddProvider;
        public IOddProvider OddProvider
        {
            get 
            {
                if (this.oddProvider == null)
                {
                    this.oddProvider = OddProviderFactory.GetOddProvider(this.Code);
                }
                return this.oddProvider;
            }
        }

        public decimal GetOddPrice(string oddType)
        {
            return this.OddProvider.GetOddValue(oddType, this);
        }
    }
}
