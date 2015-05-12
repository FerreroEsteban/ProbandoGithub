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
                case "three way - ht":
                    return new ThreeWayHalfTimeOddProvider();
                case "three way - 2nd hf":
                    return new ThreeWaySecondHalfOddProvider();
                case "odd/even":
                    return new OddEvenOddProvider();
                case "draw no bet":
                    return new DrawNoBetOddProvider();
                case "double chance (1x/x2/12)":
                    return new DobleChanceOddProvider();
                default:
                    throw new ArgumentOutOfRangeException("oddType", "You must provide a valid odd type");
            }
        }
    }
}
