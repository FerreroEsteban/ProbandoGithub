using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.Web.Controllers
{
    public class BaseController : Controller
    {
        public string CurrentSessionToken
        {
            get
            {
                return RequestContextHelper.GetCurrentToken();
            }

            set
            {
                this.req.SessionToken = value;
                RequestContextHelper.SetCurrentToken(value);
            }
        }

        public decimal CurrentSessionBalance
        {
            get
            {
                return RequestContextHelper.GetCurrentBalance();
            }

            set
            {
                RequestContextHelper.SetCurrentBalance(value);
            }
        }

        public bool IsLoginRequest
        {
            get
            {
                return RequestContextHelper.IsLogin();
            }
            set
            {
                this.req.LaunchToken = this.CurrentSessionToken;
                RequestContextHelper.SetIfLogin(value);
            }
        }

        public string LastError
        {
            get
            {
                return RequestContextHelper.GetLastError();
            }

            private set
            {
                //Do nothing
            }
        }

        public string UserName
        {
            get
            {
                return RequestContextHelper.GetUserName();
            }
            set
            {
                RequestContextHelper.SetUserName(value);
            }
        }

        private BaseRequest req = new BaseRequest();
        public BaseRequest currentRequest
        {
            get
            {
                return req;
            }
            private set
            {
                //DO NOTHING
            }
        }

        protected BaseController()
        {
            if (!string.IsNullOrEmpty(Request.Params["token"]))
            {
                this.CurrentSessionToken = Request.Params["token"].ToString();
                this.IsLoginRequest = true;
            }
        }

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            UpdateCookieValue();
            base.EndExecute(asyncResult);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            UpdateContextValues();
        }

        private void UpdateContextValues()
        {
            HttpCookie userCookie = System.Web.HttpContext.Current.Request.Cookies["ADOL.APP"];
            this.UserName = userCookie["UserName"];
            this.CurrentSessionBalance = decimal.Parse(userCookie["Balance"]);
            this.CurrentSessionToken = userCookie["sessionToken"];
        }

        private void UpdateCookieValue()
        {
            HttpCookie userCookie = new HttpCookie("ADOL.APP");
            userCookie["UserName"] = this.UserName;
            userCookie["Balance"] = this.CurrentSessionBalance.ToString();
            userCookie["sessionToken"] = this.CurrentSessionToken;
            userCookie.Expires = DateTime.Now.AddDays(30);
            System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);
        }
    }
}