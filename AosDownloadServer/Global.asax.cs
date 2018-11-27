using AosP2PClient.DBase;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TestHttpSrv.App_Start;
using TestHttpSrv.Logger;
using TestHttpSrv.Stats;

namespace TestHttpSrv
{
    public class MyMvcApplication : HttpApplication
    {
        public static BaseManager BaseManager { get; private set; }
        public static LogEntryList LogEntryList {get; private set;}
        public static LogEntryManager logEntryManager { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BaseManager = BaseManager.Instance;
            LogEntryList = new LogEntryList();
            logEntryManager = new LogEntryManager();
        }
    }
}