namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    public class Location : AuditableEntity
    {
        public Location()
        {
            this.OpenLocationClaims = new List<Claim>();
            this.CloseLocationClaims = new List<Claim>();
        }   
        
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyAbbr { get; set; }
        public string State { get; set; }
        public Company Company { get; set; }

        public IList<RiskIncident> Incidents { get; set; }

        public IList<Sli> Sli { get; set; }
        public IList<Claim> OpenLocationClaims { get; set; }
        public IList<Claim> CloseLocationClaims { get; set; }
        public IList<User> Users { get; set; }
    }
}
