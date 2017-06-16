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
    public class ClaimStatusController : Controller
    {
       #region  Private Members
        
        private IAdminService _adminService;
        private ILookUpService _lookUpService;

        #endregion
        public ClaimStatusController(ILookUpService lookupService,IAdminService adminService)
        {
            _lookUpService =  lookupService;
            _adminService = adminService;
        }

        public async Task<ViewResult> Index()
        {
            ClaimStatusMasterViewModel viewModel = new ClaimStatusMasterViewModel();

            var viewModelList = await GetClaimStatusTable();

            viewModel.ClaimStatusListViewModel = viewModelList;
            viewModel.ClaimStatusViewModel = new ClaimStatusViewModel();
            viewModel.ClaimStatusViewModel.IsNewClaimStatus = true;
            return View("Index", viewModel);
        }

        public async Task<PartialViewResult> ClaimStatus()
        {
            var viewModelList = await GetClaimStatusTable();
            return PartialView("_AdminClaimStatus", viewModelList);
        }

        public async Task<ActionResult> GetClaimStatus(long statusId)
        {
            var claimStatusDto =  await _adminService.GetClaimStatus(statusId);
            var viewModel = AdminHelper.GetClaimViewModel(claimStatusDto);
            return PartialView("_AddUpdateClaimStatus", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteClaimStatus(long statusId)
        {
            var success= await _adminService.DeleteClaimStatus(statusId);
            Cache.Remove(Constants.CacheConstants.ClaimStatuses);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("ClaimStatus");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateClaimStatus(ClaimStatusViewModel viewModel)
        {
            var user = SecurityHelper.GetUserIdFromContext();
            var success = await _adminService.AddOrUpdateClaimStatus(viewModel.Id, viewModel.ClaimStatus, user);
            Cache.Remove(Constants.CacheConstants.ClaimStatuses);
            if (success)
            {
                return RedirectToAction("ClaimStatus");
            }
            else
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetNewClaimStatus()
        {
            var viewModel = new ClaimStatusViewModel();
            viewModel.IsNewClaimStatus = true;
            return PartialView("_AddUpdateClaimStatus", viewModel);
        }

        public JsonResult IsClaimStatusUsed(long id)
        {
            var used = _adminService.IsClaimStatusUsedInClaim(id);
            return Json(new { Data = used }, JsonRequestBehavior.AllowGet);
        }

        private async  Task<IEnumerable<ClaimStatusViewModel>> GetClaimStatusTable()
        {
            var claimStatusList = await _lookUpService.GetAllClaimStatusesAsync();
            var viewModelList = AdminHelper.GetClaimListViewModel(claimStatusList).ToList();
            return viewModelList;
        
        }
	}
}