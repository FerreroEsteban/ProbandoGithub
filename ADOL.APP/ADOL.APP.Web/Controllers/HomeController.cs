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
        public class match
        {
            public string Local { get; set; }
            public string Visitante { get; set; }
        }

        private string baseURL = "http://xml2.tip-ex.com/feed/odds/";

        public ActionResult Index()
        {
            EventsManager mgr = new EventsManager();
            var sports = mgr.GetActiveSports(this.currentRequest);
            dynamic data = new List<ExpandoObject>();
            dynamic view = new ExpandoObject();
            view.errorMessage = this.LastError;
            view.userNick = this.UserName;
            view.userBalance = this.CurrentSessionBalance;
            
            string lastCode = null, lastRegion = null, lastCountry = null;
      
            foreach (var sport in sports.OrderBy(s => s.Code).ThenBy(s => s.RegionName).ThenBy(s => s.CountryName).ThenBy(s => s.Name))
            {
                if (lastCode != sport.Code)
                {
                    lastCode = sport.Code;
                    dynamic viewSport = new ExpandoObject();
                    dynamic regions = new List<ExpandoObject>();

                    viewSport.code = lastCode;
                    viewSport.name = sport.Name;

                    foreach (var region in sports.Where(s => s.Code == lastCode).OrderBy(s => s.RegionName).ThenBy(s => s.CountryName).ThenBy(s => s.Name))
                    {
                        if (lastRegion != region.RegionID)
                        {
                            lastRegion = region.RegionID;
                            dynamic newRegion = new ExpandoObject();
                            dynamic countries = new List<ExpandoObject>();

                            newRegion.name = region.RegionName;
                            newRegion.code = region.RegionID;

                            #region paises
                            foreach (var pais in sports.Where(s => s.Code == lastCode && s.RegionID == lastRegion).OrderBy(s => s.CountryName).ThenBy(s => s.TournamentName))
                            {
                                if (lastCountry != pais.Country)
                                {
                                    dynamic country = new ExpandoObject();
                                    dynamic leagues = new List<ExpandoObject>();

                                    lastCountry = pais.Country;
                                    country.code = pais.Country;
                                    country.name = pais.CountryName;
                                    country.flag = pais.MenuFlagKey;
                                    #region Ligas
                                    foreach (var liga in sports.Where(s => s.Code == lastCode && s.RegionID == lastRegion && s.Country == lastCountry).OrderBy(s => s.TournamentName))
                                    {
                                        dynamic league = new ExpandoObject();
                                        league.code = liga.TournamentID;
                                        league.name = liga.TournamentName == "" ? liga.InternalName : liga.TournamentName;
                                        leagues.Add(league);
                                    }
                                    #endregion
                                    country.leagues = leagues;
                                    countries.Add(country);
                                }
                            }
                            #endregion
                            newRegion.countries = countries;
                            regions.Add(newRegion);
                        }
                    }
                    viewSport.regions = regions;

                    data.Add(viewSport);
                }
            }

            view.data = data;
            ViewBag.menu = view;
            
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
