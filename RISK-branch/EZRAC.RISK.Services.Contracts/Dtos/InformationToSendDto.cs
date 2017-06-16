using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class InformationToSendDto
    {
        public ContractDto ContractInfo { get; set; }
        public VehicleDto VehicleInfo { get; set; }
        public IncidentDto IncidentInfo { get; set; }
        public IEnumerable<DamageDto> Damages { get; set; }
        public BillingsDto Billings { get; set; }
        public PaymentDto Payments { get; set; }
        public IEnumerable<DriverInfoDto> DriverInfo { get; set; }
        public IEnumerable<ClaimsConstant.DriverTypes> SelectedInsuranceInfo { get; set; }
        public IEnumerable<ClaimsConstant.DriverTypes> SelectedDriverInfo { get; set; }

    }
}
