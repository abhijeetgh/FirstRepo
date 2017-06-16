using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Util.Common;
using System.Threading.Tasks;
using System.Web.Mvc;
using EZRAC.Risk.UI.Web.Helper;
using System.Threading.Tasks;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using EZRAC.Risk.UI.Web.Attributes;
using System;

namespace EZRAC.Risk.UI.Web.Controllers
{    
    public class SalvageController : Controller
    {
        public SalvageController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        #region  Private Members
            private IClaimService _claimService;
        #endregion

        [HttpGet]
        [UserActionLog(UserAction = UserActionLogConstant.ViewSalvage)]
        public async Task<PartialViewResult> GetSalvage(int claimNumber)
        {            
            var salvageDto = await _claimService.GetSalvageInfoByClaimIdAsync(claimNumber);
            var salvageViewModel = ClaimHelper.GetSalvageInfo(salvageDto);
            //ViewBag.HasSalvageAccess = SecurityHelper.IsAuthorized(ClaimsConstant.Salvage);
            return PartialView("_SalvageInfo", salvageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.AddSalvage)]
        public async Task<ActionResult> SaveSalvageInfo(ViewModels.Claims.SalvageViewModel salvageViewModel)
        {
            await _claimService.SaveSalvageInfo(salvageViewModel.ClaimId, salvageViewModel.SalvageAmount, salvageViewModel.SalvageReceiptDate);
            //return Json(success, JsonRequestBehavior.AllowGet);
            return RedirectToAction("GetSalvage", new { claimNumber = salvageViewModel.ClaimId });
        }

	}
}