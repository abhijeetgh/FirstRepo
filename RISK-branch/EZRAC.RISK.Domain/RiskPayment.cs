using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskPayment
    {
        public long Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public int PaymentTypeId { get; set; }
        public Nullable<long> ReasonId { get; set; }
        public string ReferenceNumber { get; set; }
        public long ClaimId { get; set; }

        public Claim Claim { get; set; }
        public RiskPaymentType RiskPaymentType { get; set; }
        public RiskPaymentReason RiskPaymentReason { get; set; }
    }
}
