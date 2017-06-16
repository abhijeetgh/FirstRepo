namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskInsurance :AuditableEntity
    {
        
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Contact { get; set; }
        public string Notes { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

        public IList<RiskDriverInsurance> RiskDriverInsurances { get; set; }
     
    }
}
