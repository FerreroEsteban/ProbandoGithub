using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public abstract class BaseOddProvider
    {
        internal Dictionary<string, string> oddTypes { get; set; }
    }
}
