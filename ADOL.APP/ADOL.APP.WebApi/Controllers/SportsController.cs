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
using ADOL.APP.CurrentAccountService.BusinessEntities.DTOs;

namespace ADOL.APP.WebApi.Controllers
{
    public class SportsController : ApiController
    {
        public List<SportDTO> GetActiveSports()
        {
            EventsManager mgr = new EventsManager();
            List<SportDTO> sports = mgr.GetActiveSports();

            return sports;           
        }
    }
}
