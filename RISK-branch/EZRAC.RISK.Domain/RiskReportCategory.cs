using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskReportCategory :BaseEntity
    {
        public string ReportCategoryName { get; set; }

        public ICollection<RiskReport> RiskReport { get; set; }
    }
}
