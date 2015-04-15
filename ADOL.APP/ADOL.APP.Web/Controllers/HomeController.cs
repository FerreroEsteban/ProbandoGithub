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
    public class HomeController : Controller
    {
        public class match
        {
            public string Local { get; set; }
            public string Visitante { get; set; }
        }

        private string baseURL = "http://xml2.tip-ex.com/feed/odds/";

        public ActionResult Index()
        {
            EventsManager mgr = new EventsManager();
            var sports = mgr.GetActiveSports();
            
            return View(model: sports);
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
