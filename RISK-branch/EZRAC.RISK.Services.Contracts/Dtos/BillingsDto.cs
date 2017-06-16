using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class BillingsDto
    {
        public IEnumerable<RiskBillingDto> Billings { get; set; }
        public Int64 ClaimId { get; set; }
        public double TotalBilling { get; set; }
        public double TotalDue { get; set; }
    }
}
