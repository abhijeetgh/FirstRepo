using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class InsuranceCompanyViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage="Please enter company name")]
        [StringLength(100, ErrorMessage = "Company Name should not exceed 100 characters.")]
        public string CompanyName { get; set; }

        [StringLength(200, ErrorMessage = "Address should not exceed 200 characters.")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "City should not exceed 100 characters.")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "State should not exceed 50 characters.")]
        public string State { get; set; }

        [StringLength(50, ErrorMessage = "Zip should not exceed 50 characters.")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid format")]
        public string Zip { get; set; }

        [StringLength(12, ErrorMessage = "Phone Number should not exceed 50 characters.")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",ErrorMessage="Invalid format")]
        public string PhoneNumber { get; set; }

        [StringLength(50, ErrorMessage = "Fax should not exceed 50 characters.")]
        public string Fax { get; set; }

        [EmailAddress(ErrorMessage="Invalid email format")]
        [StringLength(100, ErrorMessage = "Email should not exceed 100 characters.")]
        public string Email { get; set; }

        public string CompanyNotes { get; set; }

        [StringLength(100, ErrorMessage = "Contact should not exceed 100 characters.")]
        public string Contact { get; set; }
        public bool IsNew { get; set; }
    }
}