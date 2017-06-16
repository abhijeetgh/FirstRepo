using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class WriteOffTypeViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter write off type")]
        [StringLength(50, ErrorMessage = "Write off Type should not exceed 50 characters.")]
        public string WriteOffType { get; set; }

        [Required(ErrorMessage = "Please enter description")]
        [StringLength(100, ErrorMessage = "Description should not exceed 100 characters.")]
        public string Description { get; set; }

        public bool Status { get; set; }
        public bool IsNew { get; set; }
    }
}