using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ClaimBasicInfoDto
    {
        public ClaimDto ClaimInfo { get; set; }

        public ContractDto ContractInfo { get; set; }

        public NonContractDto NonContractInfo { get; set; }

        public VehicleDto VehicleInfo { get; set; }

        public IncidentDto IncidentInfo { get; set; }
    }
}
