namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDamageType : AuditableEntity
    {
        
        new public int Id { get; set; }

        public string Section { get; set; }

        public bool IsDeleted { get; set; }
        public IList<RiskDamage> RiskDamage { get; set; }

    }
}
