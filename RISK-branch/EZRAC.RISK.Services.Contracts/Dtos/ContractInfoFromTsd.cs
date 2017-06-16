using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    /// <summary>
    /// This class is used to hold data from TSD
    /// </summary>
    public class ContractInfoFromTsd
    {
        public ContractDto ContractInfo { get; set; }
        public VehicleDto VehicleInfo { get; set; }
        public IEnumerable<DriverInfoDto> DriverAndInsuranceInfo { get; set; }

        /// <summary>
        ///Get Location Id based on loation code and save in Claims Table OpenLocationId.
        /// </summary>
        public string OpenLocationCode { get; set; }

        /// <summary>
        /// Get Location Id based on loation code and save in Claims Table CloseLocationId.
        /// </summary>
        public string CloseLocationCode { get; set; }
    }
}
