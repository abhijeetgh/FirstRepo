using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Abbr")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Please enter only 2 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter only alphabet Abbr")]
        [Display(Name = "Abbreviation")]
        public string Abbr { get; set; }

        [Required(ErrorMessage = "Please enter company name")]
        [Display(Name = "Company Name")]
        [StringLength(50, ErrorMessage = "Company name should not exceed 200 characters.")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Address should not exceed 200 characters.")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "City should not exceed 50 characters.")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "State should not exceed 50 characters.")]
        public string State { get; set; }

        [StringLength(50, ErrorMessage = "Zip should not exceed 50 characters.")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid format")]
        public string Zip { get; set; }

        [StringLength(50, ErrorMessage = "Phone should not exceed 50 characters.")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Invalid format")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Fax should not exceed 50 characters.")]
        public string Fax { get; set; }

        [StringLength(100, ErrorMessage = "Website should not exceed 100 characters.")]
        public string Website { get; set; }

        //[StringLength(50, ErrorMessage = "Zurich should not exceed 50 characters.")]
        //public string Zurich { get; set; }
    }
}