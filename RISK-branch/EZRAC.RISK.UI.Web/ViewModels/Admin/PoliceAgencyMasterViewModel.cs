using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class PoliceAgencyMasterViewModel
    {
        public IEnumerable<PoliceAgencyViewModel> PoliceAgencyList { get; set; }
        public PoliceAgencyViewModel PoliceAgencyViewModel { get; set; }
    }
}