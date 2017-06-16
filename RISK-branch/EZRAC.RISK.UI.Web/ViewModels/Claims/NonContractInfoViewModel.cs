using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class NonContractInfoViewModel
    {
        public long Id { get; set; }

        [Display(Name = "Non Contract Number")]
        public string NonContractNumber { get; set; }
        public long ClaimId { get; set; }
    }
}