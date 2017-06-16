using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class LocationViewModel
    {
        public long Id { get; set; }

        [StringLength(20, ErrorMessage = "Code should not exceed 20 characters.")]
        [Required(ErrorMessage = "Please enter code")]
        public string Code { get; set; }

        [StringLength(50, ErrorMessage = "Description should not exceed 50 characters.")]
        [Required(ErrorMessage = "Please enter description")]
        public string Description { get; set; }

        [StringLength(10, ErrorMessage = "Company abbreviation should not exceed 10 characters.")]
        public string CompanyAbbr { get; set; }

        [StringLength(2, ErrorMessage = "State code should not exceed two characters.")]
        [MinLength(2, ErrorMessage="State code cannot be of less than two characters.")]
        [RegularExpression(@"^[^\s\,\!\@\#\$\%\^\&\*\(\)\`\~\-\=\+\{\}\[\]\:\;\'\/\>\<\|\?]+$", ErrorMessage = "Please use alphabets only")]
        [Required(ErrorMessage = "Please enter state")]
        public string State { get; set; }

       
        public bool Status { get; set; }

       
        public IEnumerable<SelectListItem> Companies { get; set; }

        [Required(ErrorMessage = "Please select company.")]
        public int SelectedCompany { get; set; }

        public bool IsUpdate { get; set; }
    }
}
