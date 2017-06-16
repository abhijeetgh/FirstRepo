using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class VehicleSectionMasterViewModel
    {
        public IEnumerable<VehicleSectionViewModel> VehicleSectionsList { get; set; }
        public VehicleSectionViewModel VehicleSectionViewModel { get; set; }
    }
}