using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class BaseWalletResponseData
    {
        public string SessionToken { get; set; }
        public string UserUID { get; set; }
        public string NickName { get; set; }
        public string TransactionID { get; set; }
        public decimal Balance { get; set; }
        public WalletErrorCode errorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }
}
