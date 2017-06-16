using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.RISK.Util.Common;

namespace EZRAC.Risk.UI.Web.Controllers
{
     [Authorize]
    public class DamageController : Controller
    {
        #region  Private Members
            private IClaimService _claimService;
        #endregion
        
        
        /// <summary>
        /// public Claims controller Constructor
        /// </summary>
        #region Constructors
        public DamageController(IClaimService claimService) 
        {
            _claimService = claimService;
           
        }
        #endregion

        [UserActionLog(UserAction = UserActionLogConstant.ViewDamage)]
        public async Task<PartialViewResult> GetDamage(int claimNumber)
        {
            DamageInfoViewModel damageInfoViewModel = new DamageInfoViewModel();

            //IEnumerable<DamageTypesDto> damageTypes = await _lookUpService.GetAllDamageTypesAsync();
            damageInfoViewModel.Section = LookUpHelpers.GetAllDamageTypesListItem();
            damageInfoViewModel.ClaimId = claimNumber;
            return PartialView("_DamageInfo", damageInfoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CRMSAuthroize(ClaimType = ClaimsConstant.AddDamages)]
        [UserActionLog(UserAction = UserActionLogConstant.AddDamageInfo)]
        public async Task<ActionResult> AddDamage(DamageInfoViewModel damagesViewModel)
        {
            await _claimService.AddDamage(new DamageDto{
                                             ClaimId = damagesViewModel.ClaimId,
                                             SectionId = damagesViewModel.SelectedSectionId,
                                             Details = damagesViewModel.Details
                                         });

            return RedirectToAction("GetDamageList", new { claimNumber = damagesViewModel.ClaimId });
        }


        [HttpGet]        
        public PartialViewResult GetDamageList(int claimNumber)
        {
            IEnumerable<DamageDto> claimForDamages =  _claimService.GetDamagesInfoByClaimIdAsync(claimNumber).Result;
            var damagesGridData =  ClaimHelper.GetDamagesViewModel(claimForDamages).ToList();
            return PartialView("_DamageTableInfo", damagesGridData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeletedDamages)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteDamage)]
        public async Task<ActionResult> DeleteDamage(int damageId, int claimNumber)
        {
            await _claimService.DeleteDamage(new DamageDto() { DamageId =  damageId });
            return RedirectToAction("GetDamageList", new { claimNumber = claimNumber });
        }


	}
}