using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class DamageInfoViewModel
    {
        [Required(ErrorMessage="Please select section")]
        public IEnumerable<SelectListItem> Section { get; set; }

        [Required(ErrorMessage = "Please select section")]
        public int SelectedSectionId { get; set; }

        [Required(ErrorMessage="Please add details of damage")]
        public string Details { get; set; }
        public int ClaimId { get; set; }
        public IEnumerable<DamageViewModel> Damages { get; set; }
    }
}