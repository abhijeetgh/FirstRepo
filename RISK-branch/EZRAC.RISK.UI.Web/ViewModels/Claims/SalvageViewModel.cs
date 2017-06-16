using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class SalvageViewModel
    {
        [Required(ErrorMessage="Please enter salvage amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name="Salvage amount")]
        public Nullable<decimal> SalvageAmount { get; set; }

        [Required(ErrorMessage = "Please select salvage receipt date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> SalvageReceiptDate { get; set; }

        public long ClaimId { get; set; }
    }
}