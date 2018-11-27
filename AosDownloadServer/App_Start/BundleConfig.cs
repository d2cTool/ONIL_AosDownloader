using System.Web.Optimization;

namespace TestHttpSrv.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            //bundles.Add(new ScriptBundle("~/Static/jquery").Include("~/Static/jquery/jquery-{version}.js"));
            //bundles.Add(new ScriptBundle("~/Static/bootstrap").Include("~/Static/bootstrap/js/bootstrap.js"));
            //bundles.Add(new StyleBundle("~/Static/jquery").Include("~/Static/jquery/jquery-{version}.js"));
        }
    }
}