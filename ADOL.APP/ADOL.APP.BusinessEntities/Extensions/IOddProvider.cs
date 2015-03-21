using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public interface IOddProvider
    {
        IDictionary<string, string> GetAvailables();

        string GetOddName(string oddType);

        decimal GetOddValue(string oddType, ApuestasDeportiva odd);
    }
}
