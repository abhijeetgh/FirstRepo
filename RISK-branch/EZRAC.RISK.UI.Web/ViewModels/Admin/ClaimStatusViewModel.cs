using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class ClaimStatusViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage="Please enter claim status")]
        [StringLength(100, ErrorMessage = "Claim status should not exceed 100 characters.")]
        public string ClaimStatus { get; set; }

        public bool IsNewClaimStatus { get; set; }
    }
}