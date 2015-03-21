using ADOL.APP.CurrentAccountService.ServiceManager;
using System;
using System.Collections.Generic;
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
            //var events = new EventsManager();
            //events.UpdateEvents();
            //using (var client = new HttpClient())
            //{
            //    EventsManager man = new EventsManager();
            //    man.UpdateEvents();
            //    man.GetSportEvent("FB");
            //    client.BaseAddress = new Uri(baseURL);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    // New code:
            //    HttpResponseMessage response = client.GetAsync("xml.php?ident=discoverytx&passwd=57t6y67&mgstr=FBARG").Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        List<match> partidos = new List<match>();

            //        foreach (XElement partido in response.Content.ReadAsAsync<XElement>().Result.Elements("match"))
            //        {
            //            partidos.Add(new match() { Local = partido.Element("hteam").Value, Visitante = partido.Element("ateam").Value });
            //        }



            //        ViewBag.Partidos = partidos;
            //    }
            //}

            return View();
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
