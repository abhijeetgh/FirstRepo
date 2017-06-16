using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class LocationMasterViewModel
    {
        public IEnumerable<LocationViewModel> Locations { get; set; }
        public LocationViewModel LocationViewModel { get; set; }
    }
}