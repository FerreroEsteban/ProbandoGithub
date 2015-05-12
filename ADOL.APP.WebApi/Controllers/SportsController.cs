//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using ADOL.APP.CurrentAccountService.ServiceManager;
//using ADOL.APP.CurrentAccountService.BusinessEntities;
//using System.Dynamic;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using Newtonsoft.Json;
//using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;
//using ADOL.APP.CurrentAccountService.Helpers;

//using System.Web;

//namespace ADOL.APP.WebApi.Controllers
//{
//    public class SportsController : ApiController
//    {
//        public dynamic RefreshEventsData()
//        {
//            try
//            {
//                EventsManager mgr = new EventsManager();
//                mgr.UpdateEvents();
//                return "excecution ok";
//            }
//            catch (Exception ex)
//            {
//                return ex.ToString();
//            }
//        }
        
        //[System.Web.Http.HttpPost]
        //[System.Web.Http.ActionName("RefreshEventsData")]
        //public dynamic RefreshEventsData()
        //{
        //    try
        //    {
        //        EventsManager mgr = new EventsManager();
        //        mgr.UpdateEvents();
        //        return "excecution ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }
        //}

        //[System.Web.Http.HttpPost]
        //[System.Web.Http.ActionName("CheckEventResults")]
        //public dynamic CheckEventResults()
        //{
        //    try
        //    {
        //        EventsManager mgr = new EventsManager();
        //        mgr.CheckResults("1");
        //        return "excecution ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }
        //}
//    }
//}

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
using System.ServiceModel.Web;
using ADOL.APP.CurrentAccountService.ServiceManager;

namespace ADOL.APP.WebApi.Controllers
{
    public class SportsController : ApiController
    {
        public dynamic GetMessage()
        {
            return "message";
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.ActionName("RefreshEventsData")]
        public dynamic RefreshEventsData()
        {
            try
            {
                EventsManager mgr = new EventsManager();
                mgr.UpdateEvents();
                return "excecution ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.ActionName("RefreshSportsData")]
        public dynamic RefreshSportsData()
        {
            try
            {
                EventsManager mgr = new EventsManager();
                mgr.CheckResults();
                return "excecution RefreshSportsData";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}