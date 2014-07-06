using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WattApp.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            // Enable CORS
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("wattappapi.azurewebsites.net", "*", "*");
            config.EnableCors(cors);
            
            // ROUTEs
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
