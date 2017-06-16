namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskBillingType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    
        public IList<RiskBilling> RiskBillings { get; set; }
    }
}
