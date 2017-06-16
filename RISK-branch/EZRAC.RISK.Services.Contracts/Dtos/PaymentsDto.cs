using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class PaymentDto
    {
        public IEnumerable<PaymentInfoDto> Payments { get; set; }
        public long ClaimId { get; set; }
        public Nullable<double> TotalBilling { get; set; }
        public Nullable<double> TotalPayment { get; set; }
    }
}
