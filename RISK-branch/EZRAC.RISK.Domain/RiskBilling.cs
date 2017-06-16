namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;

    public class RiskBilling : BaseEntity
    {        
        public int BillingTypeId { get; set; }
        public double Amount { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<Int64> ClaimId { get; set; }

    
        public Claim Claim { get; set; }
        public RiskBillingType RiskBillingType { get; set; }
    }
}
