using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class IncidentInfoViewModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "Loss Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> LossDate { get; set; }


        public IEnumerable<SelectListItem> Locations { get; set; }

        [Display(Name = "Location")]
        public Nullable<Int64> SelectedLocationId { get; set; }
        public string SelectedLocationName { get; set; }


        public IEnumerable<SelectListItem> PoliceAgencies { get; set; }

        [Display(Name = "Police Agency")]
        public Nullable<int> SelectedPoliceAgencyId { get; set; }
        public string SelectedPoliceAgencyName { get; set; }

         [Display(Name = "Case Number")]
        public string CaseNumber { get; set; }

        [Display(Name = "Renter Fault")]
        public bool RenterFault { get; set; }

        [Display(Name = "Third Party Fault")]
        public bool ThirdPartyFault { get; set; }

        [Display(Name = "Reported Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> ReportedDate { get; set; }
    }
}