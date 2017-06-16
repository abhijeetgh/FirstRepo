namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDriver :BaseEntity
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string OtherContact { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<Int32> DriverTypeId { get; set; }
        public Nullable<System.DateTime> LicenceExpiryDate { get; set; }

        public string LicenceNumber { get; set; }
        public string LicenceState { get; set; }

        public bool IsAuthorized { get; set; }

        public Nullable<Int64> ClaimId { get; set; }

      
        public Claim Claim { get; set; }

        public RiskDriverInsurance RiskDriverInsurance { get; set; }
 

    }
}
