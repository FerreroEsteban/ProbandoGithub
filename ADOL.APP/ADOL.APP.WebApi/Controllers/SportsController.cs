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
    public class SportsController : ApiBaseController
    {
        public dynamic GetActiveSports()
        {
            EventsManager mgr = new EventsManager();
            var sports = mgr.GetActiveSports(this.CurrentRequest);
            dynamic data = new List<ExpandoObject>();

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
            return GetView(data);
        }
    }
}
