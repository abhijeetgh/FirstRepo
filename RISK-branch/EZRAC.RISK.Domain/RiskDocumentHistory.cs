namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDocumentHistory :BaseEntity
    {
        public RiskDocumentHistory()
        {
            this.RiskDocumentAction = new List<RiskDocumentAction>();
        }   
        
        public byte[] Document { get; set; }
    
        public IList<RiskDocumentAction> RiskDocumentAction { get; set; }
    }
}
