using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Web.Mvc;
using System;


namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class PaymentHelper
    {

        internal static PaymentsViewModel GetPaymentViewModel(PaymentDto paymentDto)
        {
            PaymentsViewModel paymentViewModel = new PaymentsViewModel();

            paymentViewModel.Payments = GetPaymentsInfo(paymentDto);
            paymentViewModel.TotalBilling = Math.Round( paymentDto.TotalBilling.HasValue ? paymentDto.TotalBilling.Value : default(double) ,2);
            paymentViewModel.ClaimId = paymentDto.ClaimId;
            paymentViewModel.TotalPayment = Math.Round(paymentDto.TotalPayment.HasValue ? paymentDto.TotalPayment.Value : default(double), 2);

            paymentViewModel.TotalDue = paymentViewModel.TotalBilling - paymentViewModel.TotalPayment;
            paymentViewModel.TotalDue = Math.Round(paymentViewModel.TotalDue.HasValue ? paymentViewModel.TotalDue.Value : default(double), 2);
            return paymentViewModel;
        }

        internal static List<PaymentInfoViewModel> GetPaymentsInfo(PaymentDto paymentDto)
        {
            List<PaymentInfoViewModel> paymentViewModelList = new List<PaymentInfoViewModel>();
            PaymentInfoViewModel paymentViewModel = null;

            foreach (var payment in paymentDto.Payments)
            {
                paymentViewModel = new PaymentInfoViewModel();

                paymentViewModel.Amount = Math.Round(payment.Amount.HasValue ? payment.Amount.Value : default(double), 2);
                paymentViewModel.SelectedPaymentFrom = payment.PaymentFrom;                
                paymentViewModel.ReceivedDate = payment.ReceivedDate;
                paymentViewModel.PaymentId = payment.PaymentId;
                paymentViewModel.SelectedReason = payment.SelectedReason;
                paymentViewModel.ReasonId = payment.ReasonId;
                paymentViewModel.SelectedReasonTypeId = payment.SelectedReasonId;
                paymentViewModel.SelectedPaymentTypeId = payment.PaymentTypeId;

                paymentViewModelList.Add(paymentViewModel);
            }
            return paymentViewModelList;
        }

        internal static List<SelectListItem> GetSelectListItems(IEnumerable<PaymentTypesDto> paymentTypesDto)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            if (paymentTypesDto != null)
            {
                lookupValueResult = paymentTypesDto.Select(x =>
                   new SelectListItem
                   {
                       Text = x.PaymentType,
                       Value = x.Id.ToString()
                   }).ToList();
            }

            return lookupValueResult.OrderBy(x => x.Text).ToList();
        }

        internal static List<SelectListItem> GetReasonSelectListItems(IEnumerable<RiskPaymentReasonDto> PaymentReasonsDto)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            if (PaymentReasonsDto != null)
            {
                lookupValueResult = PaymentReasonsDto.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Reason,
                       Value = x.Id.ToString()
                   }).ToList();
            }

            return lookupValueResult.OrderBy(x => x.Text).ToList();
        }

    }
}