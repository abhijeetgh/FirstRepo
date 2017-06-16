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
    // [Authorize]
    public class RiskWriteOffController : Controller
    {
        //
        // GET: /WriteOff/
        //private ILookUpService _lookUpService;
        private IRiskWriteOffService _riskWriteOffService;
        private IPaymentService _paymentService;

        public RiskWriteOffController(IRiskWriteOffService riskWriteOffService, IPaymentService paymentService)
        {
            //  _lookUpService = lookUpService;
            _riskWriteOffService = riskWriteOffService;
            _paymentService = paymentService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<PartialViewResult> GetWriteOff(int claimNumber)
        {
            WriteOffViewModel model = new WriteOffViewModel();
            WriteOffDto writeoff = await _riskWriteOffService.GetWriteOffByClaimIdAsync(claimNumber);

            model = ClaimHelper.GetWriteOffViewModel(writeoff, true);

            return PartialView("_WriteOffIndex", model);
        }
        [UserActionLog(UserAction = UserActionLogConstant.AddWriteOff)]
        public async Task<ActionResult> AddWriteOff(WriteOffViewModel viewModel)
        {
            var isValidAmount = await _paymentService.IsPaymentValid(viewModel.ClaimId, viewModel.Amount.Value);

            if (!isValidAmount)
            {
                ModelState.AddModelError("Amount", "Payment exceeds billed amount.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.WriteOffTypes = LookUpHelpers.GetAllWriteOffTypes();
                return PartialView("_AddWriteOff", viewModel);
            }
            if (viewModel != null)
            {
                await _riskWriteOffService.AddWriteOffInfo(new WriteOffDto()
                {
                    Amount = (double)viewModel.Amount,
                    ClaimId = viewModel.ClaimId,
                    WriteOffTypeId = viewModel.SelectedWriteOffTypeId
                });
            }
            return RedirectToAction("GetWriteOffInfo", new { claimNumber = viewModel.ClaimId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CRMSAuthroize(ClaimType = ClaimsConstant.DeletePayments)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteWriteOff)]
        public async Task<ActionResult> DeleteWriteOff(int WriteOffId, int ClaimId)
        {
            if (ClaimId > 0)
            {
                await _riskWriteOffService.DeleteWriteOffInfo(ClaimId, WriteOffId);
            }
            return RedirectToAction("GetWriteOffInfo", new { claimNumber = ClaimId });
        }
        public async Task<PartialViewResult> GetWriteOffInfo(long claimNumber)
        {
            WriteOffViewModel model = new WriteOffViewModel();
            var writeoff = await _riskWriteOffService.GetWriteOffByClaimIdAsync(claimNumber);
            model = ClaimHelper.GetWriteOffViewModel(writeoff, true);

            return PartialView("_ViewWriteOff", model);
        }
    }
}