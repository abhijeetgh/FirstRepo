using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class PoliceAgencyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Please enter police agency name.")]
        [StringLength(100, ErrorMessage = "Agency Name should not exceed 100 characters.")]
        public string AgencyName { get; set; }

        [StringLength(200, ErrorMessage = "Address should not exceed 200 characters.")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "City should not exceed 100 characters.")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "State should not exceed 50 characters.")]
        public string State { get; set; }

        [StringLength(50, ErrorMessage = "Zip should not exceed 50 characters.")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid format")]
        public string Zip { get; set; }

        [StringLength(50, ErrorMessage = "Phone should not exceed 50 characters.")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Invalid format")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Contact should not exceed 50 characters.")]
        public string Contact { get; set; }

        [StringLength(50, ErrorMessage = "Notes should not exceed 50 characters.")]
        public string Notes { get; set; }

        [StringLength(50, ErrorMessage = "Fax should not exceed 50 characters.")]
        public string Fax { get; set; }

        [EmailAddress(ErrorMessage="Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email should not exceed 100 characters.")]
        public string Email { get; set; }
        public bool IsNew { get; set; }
    }
}