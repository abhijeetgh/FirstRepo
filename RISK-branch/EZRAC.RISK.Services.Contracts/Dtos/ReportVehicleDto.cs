using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportVehicleDto
    {
        public string UnitNumber { get; set; }
        public string UnitDetails { get; set; }
        public string Model { get; set; }
        public string VehicleSection { get; set; }
    }
}
