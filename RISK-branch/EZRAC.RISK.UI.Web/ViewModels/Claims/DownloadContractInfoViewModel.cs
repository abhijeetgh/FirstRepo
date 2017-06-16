using System;
using System.ComponentModel.DataAnnotations;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class DownloadContractInfoViewModel
    {
        public Int64 ClaimId { get; set; }

        [Display(Name = "Contract Number")]
        [Required(ErrorMessage = "Please enter contract number")]
        [ContractNumberValidator(ErrorMessage = "Please provide valid contract number")]
        public string ContractNumber { get; set; }

        public Int64 ContractId { get; set; }
    }
}