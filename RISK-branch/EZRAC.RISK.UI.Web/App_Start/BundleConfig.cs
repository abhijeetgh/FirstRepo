using System.Web;
using System.Web.Optimization;

namespace EZRAC.Risk.UI.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //Java Script
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-1.11.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/select").Include(
                      "~/Scripts/bootstrap-select/bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/appLevelScript").Include(
                      "~/Scripts/appScripts/app-level-script.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/datepicker").Include(
                   "~/Scripts/bootstrap-datepicker/bootstrap-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                   "~/Scripts/jquery-ui/jquery-ui-1.11.4.js",
                   "~/Scripts/jquery-ui/jquery-ui-timepicker-addon.js"));

            bundles.Add(new ScriptBundle("~/emailGenerator").Include(
                     "~/Scripts/appScripts/email-generator.js"));

            //CSS

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/Content/common.css"));

            //bundles.Add(new StyleBundle("~/Content/css/bootstrap/datepicker").Include(
            //        "~/Content/bootstrap-datepicker/bootstrap-datepicker3.min.css"));

            bundles.Add(new StyleBundle("~/Content/css/select").Include(
                   "~/Content/bootstrap-select/bootstrap-select.min.css"));

            bundles.Add(new StyleBundle("~/Content/css/jquery-ui").Include(
                   "~/Content/jquery-ui/jquery-ui.css"));


        }
    }
}
