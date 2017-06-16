using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskPaymentReason : BaseEntity
    {
        public string Reason { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
