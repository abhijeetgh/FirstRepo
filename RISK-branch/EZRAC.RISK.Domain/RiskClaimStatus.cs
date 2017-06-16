namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RiskClaimStatus :AuditableEntity
    {
        new public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
       
        public IList<Claim> Claims { get; set; }
        
    }
}
