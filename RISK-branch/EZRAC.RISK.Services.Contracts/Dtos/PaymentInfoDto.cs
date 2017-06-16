using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class PaymentInfoDto
    {
        public long PaymentId { get; set; }
        public string PaymentFrom { get; set; }
        public int PaymentTypeId { get; set; }
        public Nullable<long> ReasonId { get; set; }
        public Nullable<double> Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public long ClaimId { get; set; }
        public int SelectedPaymentTypeId { get; set; }
        public long SelectedReasonId { get; set; }
        public string SelectedPaymentFrom { get; set; }
        public string SelectedReason { get; set; }
    }
}
