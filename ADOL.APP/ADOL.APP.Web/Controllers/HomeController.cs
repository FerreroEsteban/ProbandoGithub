using ADOL.APP.CurrentAccountService.BusinessEntities;
using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;
using ADOL.APP.CurrentAccountService.Helpers;
using ADOL.APP.CurrentAccountService.ServiceManager;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ADOL.APP.Web.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            EventsManager mgr = new EventsManager();

            ActionResultDTO dataModel = new ActionResultDTO();

            dataModel.Sports = mgr.GetActiveSports(this.currentRequest);
            dataModel.ErrorMessage = RequestContextHelper.LastError;
            dataModel.UserNick = RequestContextHelper.UserName;
            dataModel.UserBalance = RequestContextHelper.UserBalance;

            RequestContextHelper.LastError = null;

            System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("sessionToken", RequestContextHelper.SessionToken));
           
            return View(model: dataModel);
        }

        public ActionResult MatchBet(int? matchId)
        {
            return View(matchId);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Página de descripción de la aplicación.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Página de contacto.";

            return View();
        }
    }
}
