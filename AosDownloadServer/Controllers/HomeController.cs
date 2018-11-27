using System.Web.Mvc;

namespace TestHttpSrv.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("Static/clients.aspx");
        }
    }
}