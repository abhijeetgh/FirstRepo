using RateShopper.Services.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class GlobalTetheringController : Controller
    {
        private IGlobalTetherSettingService _globalTetherService;
        private ICompanyService _companyService;
        private ILocationService _locationService;
        private IGlobalTetherSettingService _globalTetherSettingService;
        private ILocationBrandService _locationBrandService;

        public GlobalTetheringController(IGlobalTetherSettingService _globalTetherService, ICompanyService _companyService, ILocationService _locationService, IGlobalTetherSettingService _globalTetherSettingService, ILocationBrandService _locationBrandService)
        {
            this._globalTetherService = _globalTetherService;
            this._companyService = _companyService;
            this._locationService = _locationService;
            this._globalTetherSettingService = _globalTetherSettingService;
            this._locationBrandService = _locationBrandService;
        }

        //
        // GET: /GlobalTethering/
        public ActionResult Index()
        {
            ViewBag.Companies = _companyService.GetAll().Where(a => a.IsBrand && !a.IsDeleted).ToList();
            ViewBag.Locations = _locationService.GetAll().Where(a => !a.IsDeleted).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult GetLocationTetheringDetails(string locationId, string dominantBrandId, string dependantBrandId)
        {
            long LocationId = 0, DominantBrandId = 0, DependantBrandId = 0;
            List<GlobalTetherValuesDTO> GlobalTetherValues = new List<GlobalTetherValuesDTO>();

            List<long> LocationCarclassIds = new List<long>();
            if (long.TryParse(locationId, out LocationId) && long.TryParse(dominantBrandId, out DominantBrandId) && long.TryParse(dependantBrandId, out DependantBrandId))
            {
                long locationBrandId = _locationBrandService.GetAll().Where(a => a.BrandID == DominantBrandId && a.LocationID == LocationId && !a.IsDeleted).Select(a => a.ID).FirstOrDefault();

                LocationCarclassIds = _globalTetherService.GetLocationCarClasses(locationBrandId);

                if (LocationCarclassIds != null && LocationCarclassIds.Count > 0)
                {
                    GlobalTetherValues = _globalTetherService.GetLocationTetheringDetails(LocationId, DominantBrandId, DependantBrandId, LocationCarclassIds);
                }
            }
            var result = new { LocationCarclassIds = LocationCarclassIds, GlobalTetherValues = GlobalTetherValues };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLocationTetheringDetails(string locationId, string dominantBrandId, string dependantBrandId, string TetherValues, string loggedInUserId)
        {
            string message = string.Empty;

            if (!string.IsNullOrEmpty(TetherValues))
            {
                message = _globalTetherSettingService.SaveLocationTetheringDetails(locationId, dominantBrandId, dependantBrandId, TetherValues, loggedInUserId);
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExistingTetheredLocations()
        {

            List<ExistingTetheredLocationsDTO> tetheredLocations = _globalTetherSettingService.ExistingTetheredLocations();
            return Json(tetheredLocations, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteLocationTethering(long locationId, long dominentBrandId, long dependentBrandId)
        {
            string message = string.Empty;
            if (locationId > 0 && dominentBrandId > 0 && dependentBrandId > 0)
            {
                message = _globalTetherSettingService.DeleteTetherSetting(locationId, dominentBrandId, dependentBrandId);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

    }
}