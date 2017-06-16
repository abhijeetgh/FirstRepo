using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportCommonPaymentVehicleDto
    {
        public ReportCommonDto ReportCommonDto { get; set; }
        public ReportPaymentDto ReportPaymentDto { get; set; }
        public ReportVehicleDto ReportVehicleDto { get; set; }
    }
}
