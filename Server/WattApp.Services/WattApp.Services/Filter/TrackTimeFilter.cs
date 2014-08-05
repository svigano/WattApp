using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;

namespace WattApp.api.Filter
{
    public class TrackTimeFilter : ActionFilterAttribute
    {
        private Stopwatch _stopWatch;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            _stopWatch.Stop();
            Log(filterContext.ActionContext, _stopWatch.ElapsedMilliseconds);
        }

        private void Log(HttpActionContext data, long time)
        {
            var controllerName = data.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = data.ActionDescriptor.ActionName;
            _logger.Info(string.Format("{0}.{1}={2} (ms)", controllerName, actionName, time));
        }
    }
}