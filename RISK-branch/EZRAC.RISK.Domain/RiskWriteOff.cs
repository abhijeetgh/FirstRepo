using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskWriteOff : BaseEntity
    {
        public Nullable<double> Amount { get; set; }
        public DateTime WriteOffDate { get; set; }
        public long WriteOffTypeId { get; set; }
        public long ClaimId { get; set; }
        public string ReferenceNumber { get; set; }        

        public Claim Claim { get; set; }
        public RiskWriteOffType RiskWriteOffType { get; set; }
    }
}
