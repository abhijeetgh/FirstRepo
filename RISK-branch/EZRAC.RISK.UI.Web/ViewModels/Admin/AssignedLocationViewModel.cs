using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class AssignedLocationViewModel
    {
        public long LocationId { get; set; }
        public string LocationCode { get; set; }
        public bool IsLocationAssigned { get; set; }
        public string CompanyAbbreviation { get; set; }
    }
}