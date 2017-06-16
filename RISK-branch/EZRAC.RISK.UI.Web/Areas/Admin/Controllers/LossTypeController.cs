using EZRAC.Core.Caching;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.RISK.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class LossTypeController : Controller
    {
        #region  Private Members
        
        private IAdminService _adminService;
        private ILookUpService _lookUpService;

        #endregion
        public LossTypeController(ILookUpService lookupService, IAdminService adminService)
        {
            _lookUpService =  lookupService;
            _adminService = adminService;
        }

        public async Task<ActionResult> Index()
        {
            var masterViewModel = new LossTypeMasterViewModel();
            var losstypes =  await _lookUpService.GetAllLossTypesAsync();
            var lossTypeList = AdminHelper.GetClaimListViewModel(losstypes);
            masterViewModel.LossTypes = lossTypeList;
            masterViewModel.LossTypeViewModel = new LossTypeViewModel();
            masterViewModel.LossTypeViewModel.IsNew = true;
            return View("Index", masterViewModel);
        }

        public async Task<PartialViewResult> LossTypeDetail(long id)
        {
            var lossTypeDetails = await _adminService.GetLossTypeDetails(id);
            var viewModel = AdminHelper.GetClaimViewModel(lossTypeDetails);
            return PartialView("_AddUpdateLossType", viewModel);
        }


        public async Task<PartialViewResult> LossTypes()
        {
            var losstypes = await _lookUpService.GetAllLossTypesAsync();
            var lossTypeList = AdminHelper.GetClaimListViewModel(losstypes);
            return PartialView("_LossTypeTable", lossTypeList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateLossType(LossTypeViewModel viewModel)
        {
            var user = SecurityHelper.GetUserIdFromContext();
            var success = await _adminService.AddOrUpdateLossType(viewModel.Id, viewModel.LossType, viewModel.Description, user);
            Cache.Remove(Constants.CacheConstants.LossTypes);
            if (success)
            {
                return RedirectToAction("LossTypes");
            }
            return Json( new { Data = false },JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> NewLossType()
        {
            var viewModel = new LossTypeViewModel();
            viewModel.IsNew = true;
            return PartialView("_AddUpdateLossType", viewModel);
        }

        public JsonResult IsLossTypeUsed(long id)
        {
            var lossTypeUsed = _adminService.IsLossTypeUsedInClaim(id);
            return Json(new { Data = lossTypeUsed }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLossType(int id)
        {
            var success = await _adminService.DeleteLossType(id);
            Cache.Remove(Constants.CacheConstants.LossTypes);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("LossTypes");
        }

	}
}