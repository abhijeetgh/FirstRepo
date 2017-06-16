using RateShopper.Controllers;
using System.Web;
using System.Web.Mvc;

namespace RateShopper
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters != null)
            {
                filters.Add(new ElmahHandledErrorLoggerFilter());
                filters.Add(new HandleErrorAttribute());
            }
        }
    }
}
