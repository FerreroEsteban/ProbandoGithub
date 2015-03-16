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

        float GetOddValue(string oddType, ApuestasDeportiva odd);
    }
}
