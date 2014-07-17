using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WattApp.api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            
            // Enable CORS
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("wattappapi.azurewebsites.net", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // ROUTEs
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
