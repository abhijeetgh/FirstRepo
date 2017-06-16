using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EZRAC.RISK.Util.Common;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class FetchDetailsViewModel
    {
        public string ContractNo { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string OriginalDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string LicenceNo { get; set; }
        public string UnitNo { get; set; }
        public long LocationId { get; set; }

      
        public IEnumerable<ClaimDto> ClaimsOnContract { get; set; }

        public IEnumerable<string> SwappedVehicles { get; set; }

        public Nullable<PurchaseType> PurchaseType { get; set; }
    }
}