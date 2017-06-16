namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    public class RiskIncident :BaseEntity
    {
        
        public Nullable<DateTime> LossDate { get; set; }
        public string LocationName { get; set; }
        public Nullable<Int64> LocationId { get; set; }

        public Nullable<int> PoliceAgencyId { get; set; }
        public string PoliceAgencyName { get; set; }
        public string CaseNumber { get; set; }
        public bool RenterFault { get; set; }
        public bool ThirdPartyFault { get; set; }
        public Nullable<System.DateTime> ReportedDate { get; set; }  
  
        public  Claim Claim { get; set; }

        public Location Location { get; set; }

        public RiskPoliceAgency PoliceAgency { get; set; }
    }
}
