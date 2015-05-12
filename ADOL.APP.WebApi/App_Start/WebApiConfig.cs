using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ADOL.APP.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );



            config.Routes.MapHttpRoute(
                name: "SportsApi",
                routeTemplate: "sportsapi/{controller}/{action}/{sportCode}",
                defaults: new { sportCode = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "GetEventsApi",
               routeTemplate: "{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
