using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using APV.Avtoliga.UI.Web.App_Start;
using APV.Avtoliga.UI.Web.Controllers;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            string url = httpContext.Request.Url.ToString();
            string method = httpContext.Request.HttpMethod;

            RouteData currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            string controllerName = null;
            string actionName = null;
            if (currentRouteData != null)
            {
                if ((currentRouteData.Values["controller"] != null) && (!string.IsNullOrEmpty(currentRouteData.Values["controller"].ToString())))
                {
                    controllerName = currentRouteData.Values["controller"].ToString();
                }
                if ((currentRouteData.Values["action"] != null) && (!string.IsNullOrEmpty(currentRouteData.Values["action"].ToString())))
                {
                    actionName = currentRouteData.Values["action"].ToString();
                }
            }

            Exception exception = Server.GetLastError();
            int statusCode = (exception is HttpException) ? ((HttpException)exception).GetHttpCode() : 500;

            Server.ClearError();
            httpContext.ClearError();

            var controller = new ErrorController();
            var routeData = new RouteData();

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";
            routeData.Values["exception"] = exception;

            var errorInfo = new ErrorInfo
                {
                    Url = url,
                    Method = method,
                    Controller = controllerName,
                    Action = actionName,
                    StatusCode = statusCode,
                    Exception = exception,
                };

            controller.ViewData.Model = errorInfo;
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }
    }
}