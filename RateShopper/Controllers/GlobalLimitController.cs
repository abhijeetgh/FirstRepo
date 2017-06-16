using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class GlobalLimitController : Controller
    {
        IGlobalLimitService _globalLimitService;
        IGlobalLimitDetailService _globalLimitDetailService;

        public GlobalLimitController(IGlobalLimitService globalLimitService, IGlobalLimitDetailService globalLimitDetailService)
        {
            _globalLimitService = globalLimitService;
            _globalLimitDetailService = globalLimitDetailService;
        }

        public ActionResult Index()
        {
            ViewBag.LocationBrands = _globalLimitService.GetLocationBrands();
            return View();
        }

        [HttpPost]
        public JsonResult GetGlobalLimits(long brandLocationID)
        {
            return Json(_globalLimitService.GetGlobalLimitDetails(brandLocationID));
        }

        [HttpPost]
        public JsonResult DeleteGlobalLimit(long globalLimitID)
        {
            if (_globalLimitDetailService.DeleteGlobalLimitDetails(globalLimitID))
            {
                return Json(_globalLimitService.DeleteGlobalLimit(globalLimitID));
            }
            return Json("0");
        }

        [HttpPost]
        public JsonResult SaveGlobalLimit(GlobalLimitDTO objGlobalLimit)
        {
            if (objGlobalLimit != null)
            {
                long? globalLimitID = objGlobalLimit.GlobalLimitID;
                objGlobalLimit.GlobalLimitID = _globalLimitService.SaveGlobalLimit(objGlobalLimit);
                if (objGlobalLimit.GlobalLimitID > 0)
                {
                    _globalLimitDetailService.SaveGlobalLimitDetails(objGlobalLimit);
                    if (globalLimitID > 0)
                    {
                        return Json(objGlobalLimit);
                    }
                    return Json(_globalLimitService.GetGlobalLimitDetails(objGlobalLimit.BrandLocation));
                }
                return Json("0");
            }
            return null;
        }
	}
}