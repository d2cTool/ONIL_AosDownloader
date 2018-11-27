using System.Web.Mvc;
using System.Web.Routing;

namespace TestHttpSrv.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.RouteExistingFiles = true;
            //routes.MapRoute("DiskFile1", "index.html", new { Controller = "Home", Action = "Index" });
            //routes.MapRoute("DiskFile2", "/", new { Controller = "Home", Action = "Index" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapRoute("SpecificRoute", "{action}/{id}", new { controller = "BaseController", action = "get", id = UrlParameter.Optional });
            //routes.MapRoute("Base", "{action}/{id}", new { controller = "Base", action = "Get", id = UrlParameter.Optional });
            routes.MapRoute("Default", "api/{controller}/{action}/{id}", new { controller = "Base", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("Log", "api/{controller}/{action}/{id}", new { controller = "Log", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("Home", "", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}