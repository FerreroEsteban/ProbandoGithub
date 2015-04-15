using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Web.Http;
using System.Dynamic;

namespace ADOL.APP.WebApi.Controllers
{
    public class ApiBaseController : ApiController
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
        protected BaseRequest CurrentRequest
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

        public dynamic GetView(dynamic data)
        {
            UpdateCookieValue();
            dynamic returnValue = new ExpandoObject();
            returnValue.userName = this.UserName;
            returnValue.balance = this.CurrentSessionBalance;
            returnValue.lastError = this.LastError;
            returnValue.data = data;
            return returnValue;
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            UpdateContextValues();
        }

        protected void UpdateContextValues()
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