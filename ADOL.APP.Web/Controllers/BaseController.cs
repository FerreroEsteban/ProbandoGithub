using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Net.Http.Headers;

namespace ADOL.APP.Web.Controllers
{
    public class BaseController : Controller
    {
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
            
        }

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (!string.IsNullOrEmpty(requestContext.HttpContext.Request.Params["token"]) && string.IsNullOrEmpty(RequestContextHelper.SessionToken))
            {
                this.req.LaunchToken = requestContext.HttpContext.Request.Params["token"];
            }
            return base.BeginExecute(requestContext, callback, state);
        }
    }
}