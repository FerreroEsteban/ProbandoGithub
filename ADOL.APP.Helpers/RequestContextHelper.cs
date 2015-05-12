using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace ADOL.APP.CurrentAccountService.Helpers
{
    public static class RequestContextHelper
    {
        private const string SESSION_TOKEN_KEY = "SessionToken";
        private const string LAST_ERROR_KEY = "LastError";
        private const string USER_NAME_KEY = "UserName";
        private const string BALANCE_KEY = "Balance";

        private static string requestToken = string.Empty;
        private static string requestNickName = string.Empty;
        private static decimal requestBalance = 0M;
        private static string requestError = string.Empty;

        //public static string RequestToken
        //{
        //    get
        //    {
        //        return requestToken;
        //    }
        //    set
        //    {
        //        requestToken = value;
        //    }
        //}

        //public static string RequestNickName
        //{
        //    get
        //    {
        //        return requestNickName;
        //    }
        //    set
        //    {
        //        requestNickName = value;
        //    }
        //}

        //public static decimal RequestBalance
        //{
        //    get
        //    {
        //        return requestBalance;
        //    }
        //    set
        //    {
        //        requestBalance = value;
        //    }
        //}

        //public static string RequestError
        //{
        //    get
        //    {
        //        return requestError;
        //    }
        //    set
        //    {
        //        requestError = value;
        //    }
        //}

        private static bool IsSessionEnabled
        {
            get
            {
                return GetSessionObjects() != null;
            }
            set
            { 
                //DO NOTHING
            }
        }

        private static HttpSessionState GetSessionObjects()
        {
            return HttpContext.Current.Session;
        }

        public static string GetCookieValue(string cookieName)
        {
            return System.Web.HttpContext.Current.Request.Cookies[cookieName].Value.ToString();
        }

        public static void SetCookieValue(string cookieName, object cookieValue)
        {
            System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(cookieName, cookieValue.ToString()));
        }

        private static IDictionary GetApplicationContext()
        {
            return HttpContext.Current.Items;
        }

        public static string SessionToken
        {
            get 
            {
                if (IsSessionEnabled)
                {
                    if (GetSessionObjects()[SESSION_TOKEN_KEY] != null)
                    {
                        return GetSessionObjects()[SESSION_TOKEN_KEY].ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return requestToken;
                }
            }
            set 
            {
                if (IsSessionEnabled)
                {
                    GetSessionObjects()[SESSION_TOKEN_KEY] = value;
                }
                else
                {
                    requestToken = value;
                }
            }
        }

        public static string LastError
        {
            get 
            {
                if (IsSessionEnabled)
                {
                    return GetSessionObjects()[LAST_ERROR_KEY] != null ? (string)GetSessionObjects()[LAST_ERROR_KEY] : string.Empty;
                }
                else
                {
                    return requestError;
                }
            }
            set 
            {
                if (IsSessionEnabled)
                {
                    GetSessionObjects()[LAST_ERROR_KEY] = value;
                }
                else
                {
                    requestError = value;
                }
            }
        }

        public static string UserName
        {
            get 
            {
                if (IsSessionEnabled)
                {
                    return GetSessionObjects()[USER_NAME_KEY] != null ? (string)GetSessionObjects()[USER_NAME_KEY] : string.Empty;
                }
                else
                {
                    return requestNickName;
                }
            }
            set 
            {
                if (IsSessionEnabled)
                {
                    GetSessionObjects()[USER_NAME_KEY] = value;
                }
                else
                {
                    requestNickName = value;
                }
            }
        }

        public static decimal UserBalance
        {
            get
            {
                if (IsSessionEnabled)
                {
                    return GetSessionObjects()[BALANCE_KEY] == null ? 0M : decimal.Parse(GetSessionObjects()[BALANCE_KEY].ToString());
                }
                else
                {
                    return requestBalance;
                }
            }
            set 
            {
                if (IsSessionEnabled)
                {
                    GetSessionObjects()[BALANCE_KEY] = value;
                }
                else
                {
                    requestBalance = value;
                }
            }
        }
    }
}
