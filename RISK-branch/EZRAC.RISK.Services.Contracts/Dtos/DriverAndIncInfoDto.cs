using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DriverInfoDto
    {

        public Int64 Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string State { get; set; }
       

        public string City { get; set; }
     

        public string Zip { get; set; }
       
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string OtherContact { get; set; }
        public string Email { get; set; }
        public string LicenceNumber { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public Nullable<DateTime> LicenceExpiry { get; set; }
        public string LicenceState { get; set; }

        public string Phone1 { get; set; }

        public bool IsAuthorizedDriver { get; set; }
        public Nullable<int> DriverTypeId { get; set; }

        
        public Nullable<int> InsuranceId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string PolicyNumber { get; set; }
        public Nullable<double> Deductible { get; set; }
        public string InsuranceClaimNumber { get; set; }
        public Nullable<DateTime> InsuranceExpiry { get; set; }
        public string CreditCardCompany { get; set; }
        public string CreditCardPolicyNumber { get; set; }
        public Nullable<double> CreditCardCoverageAmount { get; set; }



        
    }
}
