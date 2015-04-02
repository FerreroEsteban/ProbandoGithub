using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;

namespace ADOL.APP.WebApi.Controllers
{
    public class SportsController : ApiController
    {
        public dynamic GetActiveSports()
        {
            EventsManager mgr = new EventsManager();
            var sports = mgr.GetActiveSports();
            dynamic view = new List<ExpandoObject>();

            string lastCode = null, lastCountry = null;

            foreach (var sport in sports.OrderBy(s => s.Code).ThenBy(s => s.CountryName).ThenBy(s => s.Name))
            {
                if (lastCode != sport.Code)
                {
                    lastCode = sport.Code;
                    dynamic viewSport = new ExpandoObject();
                    dynamic countries = new List<ExpandoObject>();

                    viewSport.code = lastCode;

                    #region paises
                    foreach (var pais in sports.Where(s => s.Code == lastCode).OrderBy(s => s.CountryName).ThenBy(s => s.Name))
                    {
                        if (lastCountry != pais.Country)
                        {
                            dynamic country = new ExpandoObject();
                            dynamic leagues = new List<ExpandoObject>();

                            lastCountry = pais.Country;
                            country.countryID = pais.Country;
                            country.countryName = pais.CountryName;
                            #region Ligas
                            foreach (var liga in sports.Where(s => s.Code == lastCode && s.CountryName == lastCountry).OrderBy(s => s.Name))
                            {
                                dynamic league = new List<ExpandoObject>();
                                league.code = liga.League;
                                league.name = liga.Name == "" ? liga.InternalName : liga.Name;
                                leagues.Add(league);
                            }
                            #endregion
                            country.leagues = leagues;
                            countries.Add(country);
                        }
                    }
                    #endregion
                    viewSport.countries = countries;
                    view.Add(viewSport);
                }
            }
            return view;
        }
    }
}
