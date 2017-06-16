using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class PaymentInfoViewModel
    {
        public int ClaimId { get; set; }
        public long PaymentId { get; set; }
        public Nullable<long> ReasonId { get; set; }
        public int SelectedPaymentTypeId { get; set; }
        public long SelectedReasonTypeId { get; set; }
        public string SelectedPaymentFrom { get; set; }
        public string SelectedReason { get; set; }
        public Nullable<double> Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string PaymentFrom { get; set; }
    }
}