using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class MatchResults
    {
        public string MatchID { get; set; }

        public Dictionary<string, string> Results { get; set; }
    }
}
