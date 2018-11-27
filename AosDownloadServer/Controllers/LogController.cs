using System.Web;
using System.Web.Http;
using TestHttpSrv.Common;
using TestHttpSrv.Logger;

namespace TestHttpSrv.Controllers
{
    public class LogController : ApiController
    {
        private readonly LogEntryManager LogManager = MyMvcApplication.logEntryManager;

        [HttpGet]
        public string Get()
        {
            var str = Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress);
            return str;
        }

        [HttpPost]
        public void Post([FromBody] ErrorData llog)
        {
            string ipStr = Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress);
            LogManager.Add(new LogEntry(ipStr, llog));
        }
    }
}