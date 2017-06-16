using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.RISK.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EZRAC.Core.Caching;
using EZRAC.Risk.UI.Web.Attributes;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class VehicleSectionController : Controller
    {
       #region  Private Members
        
        private IVehicleSectionService _vehicleService;
        private ILookUpService _lookUpService;

        #endregion
        public VehicleSectionController(ILookUpService lookupService, IVehicleSectionService vehicleService)
        {
            _lookUpService =  lookupService;
            _vehicleService = vehicleService;
        }

        public async Task<ActionResult> Index()
        {
            var masterViewModel = new VehicleSectionMasterViewModel();
            var viewModelList = await GetVehicleSectionFromService();
            masterViewModel.VehicleSectionsList = viewModelList;
            masterViewModel.VehicleSectionViewModel = new VehicleSectionViewModel() { IsNew = true };
            return View(masterViewModel);
        }

        public async Task<PartialViewResult> VehicleSectionDetail(long id)
        {
            var vehicleSectionDto = await _vehicleService.GetVehicleSection(id);
            var viewModel = AdminHelper.GetVehicleSectionViewModel(vehicleSectionDto);
            return PartialView("_AddUpdateVehicleSection", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUpdateVehicleSection(long id, string section)
        { 
            var userId = SecurityHelper.GetUserIdFromContext();
            var success = await _vehicleService.AddOrUpdateVehicleSection(id, section, userId);
            Cache.Remove(Constants.CacheConstants.DamageTypes);
            if (success)
            {
                return RedirectToAction("VehicleSections");
            }
            return Json(new { Data = false},JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteVehicleSection(long id)
        {
            var success = await _vehicleService.DeleteVehicleSection(id);
            Cache.Remove(Constants.CacheConstants.DamageTypes);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("VehicleSections");
        }

        public async Task<PartialViewResult> VehicleSections()
        {
            var viewModelList = await GetVehicleSectionFromService();
            return PartialView("_VehicleSectionTable", viewModelList);
                 
        }

        public JsonResult IsVehicleSectionUsed(long id)
        {
            var lossTypeUsed = _vehicleService.IsVehicleSectionAlreadyUsed(id);
            return Json(new { Data = lossTypeUsed }, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> NewVehicleSection()
        {
            var viewModel = new VehicleSectionViewModel() { IsNew = true };
            return PartialView("_AddUpdateVehicleSection", viewModel);
        }

        private async Task<IEnumerable<VehicleSectionViewModel>> GetVehicleSectionFromService() 
        {
            var riskDamageTypes = await _lookUpService.GetAllDamageTypesAsync();
            var viewModelList = AdminHelper.GetVehicleSectionListVideModel(riskDamageTypes);
            return viewModelList;
        }
	}
}