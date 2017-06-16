using EZRAC.Risk.UI.Web.Controllers;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}

