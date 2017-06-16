using EZRAC.Risk.UI.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        [CRMSAdminAccess]
        public async Task<ActionResult> Index()
        {
            return View();
        }
	}
}