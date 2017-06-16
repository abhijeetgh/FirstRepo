using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {

        private IBillingService _billingService = null;
        private IClaimService _claimService = null;

        public BillingController(IBillingService billingService,IClaimService claimService)
        {
            _billingService = billingService;
            _claimService = claimService;

        }

        //
        // GET: /Billing/
        [UserActionLog(UserAction = UserActionLogConstant.ViewBilling)]
        public async Task<PartialViewResult> Index(Int64 id)
        {

            BillingsDto billings = await _billingService.GetBillingsByClaimIdAsync(id);

            Nullable<double> labourHour = await _claimService.GetLabourHoursByClaimIdAsync(id);

            BillingViewModel model = ClaimHelper.GetBillingsViewModel(billings, labourHour, true);


            return PartialView(model);
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType=ClaimsConstant.AddBilling)]
        [UserActionLog(UserAction = UserActionLogConstant.AddBilling)]
        public async Task<ActionResult> AddBilling(BillingViewModel billingViewModel)
        {

            RiskBillingDto billingDto = ClaimHelper.GetBillingsDto(billingViewModel);

            await _billingService.AddOrUpdateBillingToClaimAsync(billingDto);

            return RedirectToAction("ViewBilling", new { id = billingViewModel.ClaimId });
            
        }

        [HttpGet]
        public async Task<PartialViewResult> ViewBilling(Int64 id)
        {

            BillingsDto billings = await _billingService.GetBillingsByClaimIdAsync(id);
            BillingViewModel model = ClaimHelper.GetBillingsViewModel(billings, null, false);

            return PartialView(model);
        }

        [HttpPost]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteBilling)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteBilling)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBilling(Int64 id,Int64 claimId) {

            await _billingService.DeleteBillingByIdAsync(id);

            return RedirectToAction("ViewBilling", new { id = claimId });
        }

        public async Task<ActionResult> UpdateLabourHour(Nullable<double> LabourHour, long claimId)
        {
            bool isSuccess = false;
            isSuccess = await _claimService.UpdateLabourHour(LabourHour, claimId);

            return RedirectToAction("ViewBilling", new { id = claimId });
        
        }

	}
}