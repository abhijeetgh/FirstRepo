using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    [Serializable]
    public class UserActionLogViewModel
    {
        public int Id { get; set; }

        public string UserName{ get; set; }

        public string Name { get; set; }

        public string ContractNo { get; set; }

        public string ClaimId { get; set; }

        public string Date { get; set; }

        public string UserAction { get; set; }

        [Required(ErrorMessage = "Please select from date")]
        [Display(Name = "FromDate")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Please select To date")]
        [Display(Name = "ToDate")]
        public DateTime ToDate { get; set; }
        public string CompanyAbbr { get; set; }
    }
}