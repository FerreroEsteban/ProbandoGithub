using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ADOL.APP.CurrentAccountService.Helpers
{
    public static class RequestContextHelper
    {
        private static IDictionary GetContext()
        {
            return HttpContext.Current.Items;
        }

        public static string GetCurrentToken()
        {
            return GetContext()["sessionToken"].ToString() ?? string.Empty;
        }

        public static void SetCurrentToken(string sessionToken)
        {
            if(string.IsNullOrEmpty(sessionToken) || sessionToken.Length < 30)
                throw new ArgumentOutOfRangeException("sessionToken", "A valid sessionToken must be not null and needs more than 30 characters");
            GetContext()["sessionToken"] = sessionToken;
        }

        public static decimal GetCurrentBalance()
        {
            return (decimal)GetContext()["userBalance"];
        }

        public static void SetCurrentBalance(decimal userBalance)
        {
            GetContext()["userBalance"] = userBalance;
        }

        public static bool IsLogin()
        {
            return (bool)GetContext()["withLogin"];
        }

        public static void SetIfLogin(bool withLogin)
        {
            GetContext()["withLogin"] = withLogin;
        }

        public static string GetLastError()
        {
            return GetContext()["LastError"].ToString();
        }

        public static void SetLastError(string value)
        {
            GetContext()["LastError"] = GetContext()["LastError"] == null ? value : string.Format("{0} - {1}", GetContext()["LastError"].ToString(), value);
        }

        public static string GetUserName()
        {
            return GetContext()["UserName"].ToString() ?? string.Empty;
        }

        public static void SetUserName(string userName)
        {
            GetContext()["UserName"] = userName;
        }
    }
}
