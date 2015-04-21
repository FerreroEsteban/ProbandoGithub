using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Web.Http;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ADOL.APP.WebApi.Controllers
{
    public class ApiBaseController : ApiController
    {
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

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var cookieToken = System.Web.HttpContext.Current.Request.Cookies["sessionToken"].Value.ToString();
            RequestContextHelper.SessionToken = cookieToken;
        }
        
        public dynamic GetView(dynamic data)
        {
            dynamic returnValue = new ExpandoObject();
            returnValue.userName = RequestContextHelper.UserName;
            returnValue.balance = RequestContextHelper.UserBalance;
            returnValue.lastError = RequestContextHelper.LastError;
            returnValue.data = data;
            RequestContextHelper.LastError = null;
            System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("sessionToken", RequestContextHelper.SessionToken));
            return returnValue;
        }
    }
}