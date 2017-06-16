using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportPaymentDto
    {
        public double? Charges { get; set; }
        public double? Payments { get; set; }
        public double? Estimated { get; set; }
        public double? OtherChanrges { get; set; }
        public string AdminFee { get; set; }
        public string DiminFee { get; set; }
        public string AppFee { get; set; }
        public string PayForm { get; set; }
        public string PaidDate { get; set; }
        public string CheckAmount { get; set; }
        public string Billed { get; set; }
        public double? Balance { get; set; }
        public string Location { get; set; }
        public string Contract { get; set; }
        public long Claim { get; set; }
        public string CompanyAbbr { get; set; }
    }
}
