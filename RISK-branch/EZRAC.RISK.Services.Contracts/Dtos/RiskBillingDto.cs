using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class RiskBillingDto
    {
        public Int64 Id { get; set; }
        public int BillingTypeId { get; set; }
        public string BillingTypeDesc { get; set; }
        public Nullable<Int64> ClaimId { get; set; }
        public double Amount { get; set; }
        public Nullable<double> Discount { get; set; }
        public double SubTotal { get; set; }

        public bool AutoAdminFeeCalculate { get; set; }
    }
}
