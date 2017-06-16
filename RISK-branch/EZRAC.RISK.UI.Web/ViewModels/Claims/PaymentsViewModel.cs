using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class PaymentsViewModel
    {
        public IEnumerable<SelectListItem> PaymentTypes { get; set; }

        [Required(ErrorMessage = "Please enter received date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> ReceivedDate { get; set; }

        [Required(ErrorMessage="Please select payment")]
        public int SelectedPaymentTyepId { get; set; }

        
        public Nullable<long> SelectedPaymentReasonId { get; set; }

        [Required(ErrorMessage="Please enter amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        public Nullable<double> Amount { get; set; }

        public Nullable<double> TotalPayment { get; set; }

        public Nullable<double> TotalBilling { get; set; }

        public long ClaimId { get; set; }
        public IEnumerable<PaymentInfoViewModel> Payments { get; set; }
        public IEnumerable<SelectListItem> Reasons { get; set; }

        public Nullable<double> TotalDue { get; set; }
    }
}