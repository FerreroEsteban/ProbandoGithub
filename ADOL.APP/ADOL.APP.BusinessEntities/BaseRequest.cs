using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class BaseRequest
    {
        public string LaunchToken { get; set; }
        public string SessionToken { get; set; }
        public string UserUID { get; set; }
        public string TransactionID { get; set; }
        public int EventID { get; set; }
        public string EventName { get; set; }
        public decimal Amount { get; set; }
    }
}
