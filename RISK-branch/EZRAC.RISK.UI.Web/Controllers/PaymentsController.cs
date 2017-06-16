using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
     [Authorize]
    public class PaymentsController : Controller
    {
       
         #region  Private Members

       
        private IPaymentService _paymentService;
        

        #endregion


        public PaymentsController(IPaymentService paymentService)
        {
           
            _paymentService = paymentService;
           
        }

         [UserActionLog(UserAction = UserActionLogConstant.ViewPayment)]
        public async Task<PartialViewResult> GetPayments(int claimNumber)
        {
            PaymentsViewModel viewmodel = new PaymentsViewModel(); 
            var paymentsDto  =  await _paymentService.GetAllPaymentsByClaimId(claimNumber);
            viewmodel = PaymentHelper.GetPaymentViewModel(paymentsDto);

            var paymentType = await _paymentService.GetAllPaymentTypesAsync();
            viewmodel.PaymentTypes = PaymentHelper.GetSelectListItems(paymentType);

            var paymentReasons = await _paymentService.GetAllPaymentReasonsAsync();
            viewmodel.Reasons = PaymentHelper.GetReasonSelectListItems(paymentReasons);

            viewmodel.ClaimId = claimNumber;
            return PartialView("_PaymentTab",viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.AddPayments)]
        [UserActionLog(UserAction = UserActionLogConstant.AddPayment)]       
        public async Task<ActionResult> AddPayment(PaymentsViewModel viewModel)
        {
            var isValidAmount = await _paymentService.IsPaymentValid(viewModel.ClaimId, viewModel.Amount.Value);

            if (!isValidAmount)
            {
                ModelState.AddModelError("Amount", "Payment exceeds billed amount.");
            }
            if (!ModelState.IsValid)
            {
                var paymentType = await _paymentService.GetAllPaymentTypesAsync();
                viewModel.PaymentTypes = PaymentHelper.GetSelectListItems(paymentType);

                var paymentReasons = await _paymentService.GetAllPaymentReasonsAsync();
                viewModel.Reasons = PaymentHelper.GetReasonSelectListItems(paymentReasons);
                return PartialView("_AddPayments", viewModel);
            }
            if (viewModel != null)
            {
               await _paymentService.AddPaymentInfo(
                    new PaymentInfoDto {
                        Amount = (double)viewModel.Amount,
                        ClaimId = viewModel.ClaimId,
                        PaymentTypeId = viewModel.SelectedPaymentTyepId,
                        ReasonId=viewModel.SelectedPaymentReasonId,
                        ReceivedDate = (DateTime)viewModel.ReceivedDate
                    });
            }
            return RedirectToAction("GetPaymentsTable", new { claimNumber = viewModel.ClaimId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeletePayments)]
        [UserActionLog(UserAction = UserActionLogConstant.DeletePayment)]
        public async Task<ActionResult> DeletePayment(int claimId, int paymentId)
        {
            if (claimId != null)
            {
                await _paymentService.DeletePaymentInfo(
                    new PaymentInfoDto { 
                        PaymentId = paymentId,
                        ClaimId = claimId 
                    });
            }
            return RedirectToAction("GetPaymentsTable", new { claimNumber = claimId });
        }

        public async Task<PartialViewResult> GetPaymentsTable(long claimNumber)
        {
            var paymentsDto = await _paymentService.GetAllPaymentsByClaimId(claimNumber);
            PaymentsViewModel viewmodel = PaymentHelper.GetPaymentViewModel(paymentsDto);

            return PartialView("_PaymentTableInfo", viewmodel);
        }
	}
}