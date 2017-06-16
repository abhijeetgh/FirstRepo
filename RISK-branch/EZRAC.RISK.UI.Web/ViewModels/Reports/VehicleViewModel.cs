using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class VehicleViewModel
    {
        public string UnitNumber { get; set; }
        public string UnitDetails { get; set; }
        public string TagNumber { get; set; }
        public string Model { get; set; }
        public string VehicleSection { get; set; }
    }
}