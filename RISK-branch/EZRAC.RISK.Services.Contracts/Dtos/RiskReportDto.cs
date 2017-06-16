using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class RiskReportDto
    {
        public long RiskReportId { get; set; }
        public string RiskReportName { get; set; }
        public string RiskReportKey { get; set; }
        public long ReportCategoryId { get; set; }
        public string ReportCategoryName { get; set; }
    }
    public class RiskReportCategoryDto
    {
        public long RiskReportCategoryId { get; set; }
        public string ReportCategoryName { get; set; }
        public IEnumerable<RiskReportDto> RiskReports { get; set; }
    }
}
