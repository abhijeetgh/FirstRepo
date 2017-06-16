namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDocumentType : BaseEntity
    {
       
        public string Description { get; set; }

        public long CategoryId { get; set; }

        public bool IsDeleted { get; set; }

        public RiskDocumentCategory RiskDocumentCategory { get; set; }
    }
}
