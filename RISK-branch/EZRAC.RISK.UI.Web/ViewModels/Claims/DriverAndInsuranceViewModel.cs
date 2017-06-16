using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class DriverAndInsuranceViewModel
    {

        public Int64 DriverId { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
         [Display(Name = "Last Name")]
        public string LastName { get; set; }


        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string  Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }

        [Display(Name = "Other Contact")]
        public string OtherContact { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter licence number")]
        [Display(Name = "Licence Number")]
        public string LicenceNumber { get; set; }

        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> DOB { get; set; }

        [Required(ErrorMessage = "Please enter licence expiry")]
         [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        [Display(Name = "Licence Expiry")]
        public Nullable<DateTime> LicenceExpiry { get; set; }

        [Display(Name = "Licence State")]
        public string LicenceState { get; set; }

        [Display(Name = "Authorized Driver")]
        public bool IsAuthorizedDriver { get; set; }

        public Nullable<int> DriverType { get; set; }

         
         public IEnumerable<SelectListItem> InsuranceCompanies { get; set; }

        [Display(Name = "Insurance Company")]
         public Nullable<int> InsuranceCompanyId { get; set; }

         [Display(Name = "Insurance Company")]
         public string InsuranceCompanyName { get; set; }

        [Display(Name = "Policy Number")]
        public string PolicyNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        public Nullable<double> Deductible { get; set; }

        [Display(Name = "Insurance Claim Number")]
        public string InsuranceClaimNumber { get; set; }

         [Display(Name = "Insurance Expiry")]
         [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> InsuranceExpiry { get; set; }

        [Display(Name = "Credit Card Company")]
        public string CreditCardCompany { get; set; }

        [Display(Name = "Credit Card Policy Number")]
        public string CreditCardPolicyNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "CC Deductible")]
        public Nullable<double> CreditCardCoverageAmt { get; set; }

        public Int64 ClaimId { get; set; }
        public int DriverInsuranceId { get; set; }

       
    }
}