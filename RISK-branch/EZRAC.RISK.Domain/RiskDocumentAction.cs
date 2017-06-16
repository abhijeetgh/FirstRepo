namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDocumentAction :BaseEntity
    {        
        public Nullable<System.DateTime> DocumentGeneratedDate { get; set; }
        public Nullable<int> GeneratedBy { get; set; }
    
        public Claim Claim { get; set; }
        public RiskDocumentHistory RiskDocumentHistory { get; set; }
        public RiskDocumentType RiskDocumentType { get; set; }
    }
}
