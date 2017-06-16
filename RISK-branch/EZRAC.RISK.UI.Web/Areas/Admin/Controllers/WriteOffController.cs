using EZRAC.Core.Caching;
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
    public class WriteOffController : Controller
    {
        #region  Private Members
        
        private IAdminService _adminService;
        private ILookUpService _lookUpService;

        #endregion
        public WriteOffController(ILookUpService lookupService, IAdminService adminService)
        {
            _lookUpService =  lookupService;
            _adminService = adminService;
        }

        //
        // GET: /Admin/WriteOff/
        public async Task<ActionResult> Index()
        {
            var masterViewModel = new WriteOffTypeMasterViewModel();
            var writeOfftypes = await _lookUpService.GetAllWriteOffTypesAsync();
            var writeOffTypeList = AdminHelper.GetWriteOffListViewModel(writeOfftypes);
            masterViewModel.WriteOffTypes  = writeOffTypeList;
            masterViewModel.WriteOffTypeViewModel = new WriteOffTypeViewModel();
            masterViewModel.WriteOffTypeViewModel.IsNew = true;
            return View("Index", masterViewModel);            
        }

        public async Task<PartialViewResult> WriteOffTypeDetail(long id)
        {
            var writeOffTypeDetails = await _adminService.GetWriteOffTypeDetails(id);
            var viewModel = AdminHelper.GetClaimViewModel(writeOffTypeDetails);
            return PartialView("_AddUpdateWriteOffType", viewModel);
        }


        public async Task<PartialViewResult> WriteOffTypes()
        {
            var writeOfftypes = await _lookUpService.GetAllWriteOffTypesAsync();
            var writeOffTypeList = AdminHelper.GetWriteOffListViewModel(writeOfftypes);
            return PartialView("_WriteOffTypeTable", writeOffTypeList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateWriteOffType(WriteOffTypeViewModel viewModel)
        {
            var user = SecurityHelper.GetUserIdFromContext();
            var success = await _adminService.AddOrUpdateWriteOffType(viewModel.Id, viewModel.WriteOffType, viewModel.Description, user);
            Cache.Remove(Constants.CacheConstants.WriteOffTypes);
            if (success)
            {
                return RedirectToAction("WriteOffTypes");
            }
            return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewWriteOffType()
        {
            var viewModel = new WriteOffTypeViewModel();
            viewModel.IsNew = true;
            return PartialView("_AddUpdateWriteOffType", viewModel);
        }

        public JsonResult IsWriteOffTypeUsed(long id)
        {
            var writeOffTypeUsed = _adminService.IsWriteOffTypeUsedInClaim(id);
            return Json(new { Data = writeOffTypeUsed }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteWriteOffType(int id)
        {
            var success = await _adminService.DeleteWriteOffType(id);
            Cache.Remove(Constants.CacheConstants.WriteOffTypes);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("WriteOffTypes");
        }
	}
}