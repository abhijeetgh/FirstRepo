using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class DamageViewModel
    {
        //public List<SelectListItem> Section { get; set; }
        public string SelectedSection { get; set; }
        //public int SectionId { get; set; }
        public string Details { get; set; }
        //public int ClaimId { get; set; }
        //public int VehicleId { get; set; }
        public long DamageId { get; set; }
        //public List<DamageDto> Damages { get; set; }
    }
}