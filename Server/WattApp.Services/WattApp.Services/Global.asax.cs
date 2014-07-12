using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using WattApp.data.Models;
//using WattApp.Services.App_Start;
using WattApp.Services.Filter;

namespace WattApp.Services
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<WattAppContext>(null);
            WattAppContext c = new WattAppContext();
            
            GlobalConfiguration.Configuration.Filters.Add(new TrackTimeFilter());
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }
    }
}
