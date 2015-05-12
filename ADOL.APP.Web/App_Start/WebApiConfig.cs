﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ADOL.APP.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "WebApi",
                routeTemplate: "backoffice/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            

            //config.Routes.MapHttpRoute(
            //    name: "CustomApi",
            //    routeTemplate: "dataapi/{controller}/{action}/{id}",
            //    defaults: new { action = "RefreshEventsData", id = RouteParameter.Optional }
            //);
        }
    }
}
