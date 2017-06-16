using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class VehicleViewModel
    {
        public string UnitNumber { get; set; }
        public string TagNumber { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string VIN { get; set; }

        public string Model { get; set; }

        public Nullable<long> Mileage { get; set; }

        public string Location { get; set; }

        public string Color { get; set; }
    }
}