using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class VehicleSectionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Please enter section")]
        [StringLength(100, ErrorMessage = "Section should not exceed 100 characters.")]
        public string Section { get; set; }
        public bool IsNew { get; set; }
    }
}