using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace RateShopper
{
	public static class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			BundleTable.EnableOptimizations = bool.Parse(ConfigurationManager.AppSettings["EnableOptimizations"]);

            if (bundles != null)
            {
                //Master bundle for js
                bundles.Add(new ScriptBundle("~/bundles/masterBundleJS").Include(
                    "~/Scripts/jquery-1.10.2.min.js",
                    "~/Scripts/knockoutmin.js",
                    "~/scripts/master.js",
                    "~/Scripts/tabs.js",
                    "~/Scripts/jquery-ui-1.11.4.js"));

                //Master bundle for css
                bundles.Add(new ScriptBundle("~/bundles/masterBundleCSS").Include(
                    "~/css/master.css",
                    "~/css/jquery-ui.css"));

                //search page js bundle
                bundles.Add(new ScriptBundle("~/bundles/searchBundleJS").Include(
                    "~/Scripts/search-knockout.js",
                    "~/Scripts/search-functions.js",
                    "~/Scripts/search-knockout-classic.js",
                    "~/Scripts/search-functions-classic.js",
                    "~/Scripts/SearchSummary-popup-functions.js",
                    "~/Scripts/QuickViewSchedule.js",
                    "~/Scripts/QuickView-knockout.js",
                    "~/Scripts/SearchSummary-popup-functions.js",
                    "~/Scripts/UpdateAllTSD.js",
                    "~/Scripts/OpaqueRate-functions.js"
                    ));

                bundles.Add(new ScriptBundle("~/bundles/AutomationConsoleBundleJS").Include(
                "~/Scripts/jquery-ui-timepicker-addon.js",
                 "~/Scripts/AutomationConsole-functions.js",
                 "~/Scripts/AutomationConsoleJobSchedules.js",
                 "~/Scripts/AutomationConsoleMinRate-functions.js",
                 "~/Scripts/AutomationConsoleTethering.js",
                 "~/Scripts/AutomationConsoleOpaqueRate.js",
                 "~/Scripts/markResults.js",
                "~/Scripts/AutomationConsoleRuleSet.js",
                "~/Scripts/RuleSet-functions.js"
                  ));

                bundles.Add(new ScriptBundle("~/bundles/FTBScheduledJobBundleJS").Include(
               "~/Scripts/jquery-ui-timepicker-addon.js",
                "~/Scripts/FTBScheduledJob-functions.js",
                "~/Scripts/FTBAutomationFunctions.js"
                 ));

                bundles.Add(new ScriptBundle("~/bundles/FTBRateBundleJS").Include(
               "~/Scripts/FTBRateSetting.js",
                "~/Scripts/FTBTargetReservation.js"
                 ));

                bundles.Add(new ScriptBundle("~/bundles/SummaryBundleJS").Include(
                    "~/Scripts/Summary-knockout.js",
                    "~/Scripts/Summary-functions.js"
                    ));

                bundles.Add(new ScriptBundle("~/bundles/RezCentralUpdateBundleJS").Include(
              "~/Scripts/jquery-ui-timepicker-addon.js",
               "~/Scripts/RezCentralUpdate.js"
                ));
            }
		}
	}
}
