namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskSalvage :BaseEntity
    {        
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    
        public Claim Claim { get; set; }
    }
}
