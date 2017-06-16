using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportClaimVehiclePaymentDto
    {
        public ReportClaimDto ReportClaimDto { get; set; }
        public ReportVehicleDto ReportVehicleDto { get; set; }
        public ReportPaymentDto ReportPaymentDto { get; set; }
    }
}
