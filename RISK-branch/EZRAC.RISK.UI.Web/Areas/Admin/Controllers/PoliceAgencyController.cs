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
    public class PoliceAgencyController : Controller
    {
         #region  Private Members
        
        private ILookUpService _lookupService;
        private IPoliceAgencyService _policeAgencyService;

        #endregion
        public PoliceAgencyController(ILookUpService lookupService, IPoliceAgencyService policeAgencyService)
        {
            _lookupService =  lookupService;
            _policeAgencyService = policeAgencyService;
        }

        public async Task<ActionResult> Index()
        {
            var masterViewModel = new PoliceAgencyMasterViewModel();

            var viewModels = await GetPoliceAgencyViewModel();
            masterViewModel.PoliceAgencyList = viewModels;

            masterViewModel.PoliceAgencyViewModel = new PoliceAgencyViewModel();
            masterViewModel.PoliceAgencyViewModel.IsNew = true;
            return View ("Index",masterViewModel);
        }

        public async Task<PartialViewResult> PoliceAgency(long id)
        {
            var policeAgencyDto = await _policeAgencyService.GetPoliceAgencyDetailById(id);
            var viewModel = AdminHelper.GetPoliceAgencyViewModel(policeAgencyDto);
            return PartialView("_AddUpdatePoliceAgency",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdatePoliceAgency(PoliceAgencyViewModel viewModel)
        {
            var userId = SecurityHelper.GetUserIdFromContext();
            var dto = AdminHelper.GetPoliceAgencyDto(viewModel);
            var success = await _policeAgencyService.AddOrUpdatePoliceAgency(dto, userId);
            Cache.Remove(Constants.CacheConstants.PoliceAgenecies);
            if (success)
            {
                return RedirectToAction("GetPoliceAgencyList");
            }
            return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeletePoliceAgency(int id)
        {
            var userId = SecurityHelper.GetUserIdFromContext();

            var success = await _policeAgencyService.DeletePoliceAgency(
                new PoliceAgencyDto
                {
                    Id = id
                }, userId);
            Cache.Remove(Constants.CacheConstants.PoliceAgenecies);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("GetPoliceAgencyList");
        }


        public async Task<PartialViewResult> GetPoliceAgencyList()
        {
            var viewModels = await GetPoliceAgencyViewModel();
            return PartialView("_PoliceAgencyTable", viewModels);
        }

        public PartialViewResult AddPoliceAgency()
        {
            var viewModel = new PoliceAgencyViewModel() {  IsNew = true};
            return PartialView("_AddUpdatePoliceAgency", viewModel);
        }

        public JsonResult PoliceAgencyAlreadyUsed(long id)
        {
            var alreadyUsed = _policeAgencyService.IsPoliceAgencyAlreadyUsed(id);
            return Json(new { Data = alreadyUsed }, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods
        private async Task<IEnumerable<PoliceAgencyViewModel>> GetPoliceAgencyViewModel()
        {
            var policeAgencies = await _lookupService.GetAllPoliceAgenciesAsync();
            var viewModel = AdminHelper.GetPoliceAgencyListVideModel(policeAgencies);
            return viewModel;
        } 
        #endregion
	}
}