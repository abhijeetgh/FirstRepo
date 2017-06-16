using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportCriteriaDto
    {
        public string ReportKey { get; set; }
        public string ReportText { get; set; }

        public string TagNumber { get; set; }

        public string DateTypeKey { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AsOfDate { get; set; }

        public string ReportTypeKey { get; set; }

        public string ChargeTypeCondition { get; set; }

        public int Status { get; set; }
        public string TicketOrViolations { get; set; }
        
        public int GreaterThanLessThan { get; set; }
        public long GreaterThanLessThanValue { get; set; }

        public bool IncludeTicket { get; set; }
        public bool IncludeCollection { get; set; }

        public long AgentId { get; set; }
        public string PaymentTypeId { get; set; }
        public long ChargeTypeId { get; set; }

        //Multiple dropdown selection
        public string ClaimStatusIds { get; set; }

        public string LossTypeIds { get; set; }

        public string LocationIds { get; set; }

        public string UserIds { get; set; }

        public string VehicalDamageIds { get; set; }

        public string AgingDays { get; set; }

    }
}
