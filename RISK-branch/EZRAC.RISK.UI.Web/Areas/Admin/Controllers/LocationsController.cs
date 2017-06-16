using EZRAC.Core.Caching;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class LocationsController : Controller
    {

        #region  Private Members
        
        
        private ILookUpService _lookupService;
        private ILocationService _locationService;

        #endregion
        public LocationsController(ILookUpService lookupService, ILocationService locationService)
        {
            _lookupService =  lookupService;
            _locationService = locationService;
        }

        //
        // GET: /Admin/Location/
        public async Task<ActionResult> Index()
        {

            IEnumerable<LocationDto> locations = await _lookupService.GetAllLocationsAsync();

            LocationMasterViewModel model = AdminHelper.GetLocationMasterViewModel(locations);

            return View(model);
        }

        public async Task<ActionResult> View() {

            IEnumerable<LocationDto> locations = await _lookupService.GetAllLocationsAsync();

            LocationMasterViewModel model = AdminHelper.GetLocationMasterViewModel(locations);

            return PartialView(model.Locations);
        
        }

        public async Task<ActionResult> AddOrUpdate(long id) {
            // If id equals to zero the create new location or update location with location ID
            LocationViewModel model = null;

            LocationDto locationDto = id != 0 ? await _locationService.GetLocationByIdAsync(id) : null;

            model = locationDto != null ? AdminHelper.GetLocationViewModel(locationDto) : AdminHelper.GetEmptyLocationViewModel();

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdate(LocationViewModel locationViewModel)
        {
            if (await IsModelValid(locationViewModel))
            {
                long userId = SecurityHelper.GetUserIdFromContext();

                LocationDto locationDto = AdminHelper.GetLocationDto(locationViewModel, userId);

                bool isSucess = await _locationService.AddOrUpdateLocationAsync(locationDto);

                if (isSucess)
                Cache.Remove(Constants.CacheConstants.Locations);

            }
            else {

                locationViewModel.Companies = LookUpHelpers.GetCompanyListItem();

                return PartialView(locationViewModel);
            
            }

            return RedirectToAction("View");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id) {

            bool isSuccess = false;
            if (id != 0)
            {

                isSuccess = await _locationService.DeleteByIdAsync(id);
                Cache.Remove(Constants.CacheConstants.Locations);
            }


            return RedirectToAction("View");
        }

        public async Task<ActionResult> IsLocationUsed(long id) {

            bool isSuccess = false;
            if (id != 0)
            {

                isSuccess = await _locationService.IsLocationUsed(id);
            }
            
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        private async Task<bool> IsModelValid(LocationViewModel locationViewModel)
        {
            bool isSuccess = false;
            if (ModelState.IsValid)
            {
                LocationDto locationDto = AdminHelper.GetLocationDto(locationViewModel);

                if (await _locationService.IsLocationValidAsync(locationDto))
                    isSuccess = true;
                else
                    ModelState.AddModelError("Code", "Airport code already in this company");
            }

            return isSuccess;
        }

        
	}
}