using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskReportDefaultselection :BaseEntity
    {
        public string Property { get; set; }
        public long PropId { get; set; }
        public string ReportKey { get; set; }
        public bool IsSelected { get; set; }
    }
}
