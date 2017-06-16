using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.RISK.Util.Common;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class VehicleDto
    {
        public string UnitNumber { get; set; }
        public string TagNumber { get; set; }
        public Nullable<DateTime> TagExpires { get; set; }
        public string VIN { get; set; }
        public string Description { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public Nullable<long> Mileage { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public string SwappedVehicles { get; set; }

        public Nullable<PurchaseType> PurchaseType { get; set; }

        public Int64 Id { get; set; }
    }
}
