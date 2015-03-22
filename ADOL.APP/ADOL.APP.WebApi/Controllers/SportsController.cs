﻿using System;
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

            foreach (var sport in sports)
            { 
                dynamic viewSport = new ExpandoObject();
                viewSport.LeagueID = sport.IDdeProveedor;
                viewSport.League = sport.Liga;
                viewSport.Pais = sport.NombePais;
                viewSport.InternalCode = sport.NombreInterno;
                viewSport.Nombre = sport.Nombre;
                view.Add(viewSport);
            }
            return view;
        }
    }
}
