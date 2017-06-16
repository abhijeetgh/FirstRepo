using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class AdminFeeCommissionReportDto
    {
        
        public IList<ClaimWiseAdminFeeCommissionReportDto> ClaimWiseAdminFeeCommissionReportDto { get; set; }

        public IList<UserWiseAdminFeeCommissionReportDto> UserWiseAdminFeeCommissionReportDto { get; set; }

        public int TotalContracts { get; set; }
        public int TotalPayments { get; set; }
    }

    [Serializable]
    public class ClaimWiseAdminFeeCommissionReportDto
    {
        public long Claim { get; set; }
        public string Contract { get; set; }
        public string Location { get; set; }
        public string CompanyAbbr { get; set; }
        public double Payment { get; set; }

    }

    [Serializable]
    public class UserWiseAdminFeeCommissionReportDto
    {
        public string Employee { get; set; }
        public int AmountEach { get; set; }
        public int Quantity { get; set; }
        public int Commission { get; set; }

    }
}
