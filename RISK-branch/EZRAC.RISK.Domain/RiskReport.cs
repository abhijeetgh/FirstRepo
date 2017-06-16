using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskReport :BaseEntity
    {
        public string ReportName { get; set; }
        public string ReportKey { get; set; }
        public long ReportCategoryId { get; set; }
        public RiskReportCategory RiskReportCategory { get; set; }
    }
}
