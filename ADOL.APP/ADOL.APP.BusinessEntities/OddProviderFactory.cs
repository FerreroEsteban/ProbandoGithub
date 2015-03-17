using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public static class OddProviderFactory
    {
        internal static IOddProvider GetOddProvider(string oddType)
        {
            switch (oddType)
            {
                case "three way":
                    return new ThreeWayOddProvider();
                default:
                    throw new ArgumentOutOfRangeException("oddType", "You must provide a valid odd type");
            }
        }
    }
}
