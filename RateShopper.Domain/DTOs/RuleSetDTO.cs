using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;

namespace RateShopper.Domain.DTOs
{
    public class RuleSetDTO
    {
        public long ScheduledJobRuleSetID { get; set; }
        public long ScheduledJobID { get; set; }
        public long ruleSetID { get; set; }
        public long originalRuleSetID { get; set; }
        public long locationBrandID { get; set; }
        public string ruleSetName { get; set; }
        public int companyPositionAbvAvg { get; set; }
        public bool isPositionOffset { get; set; }
        public bool isWideGapTemple { get; set; }
        public bool isGov { get; set; }
        public string selectedCompanyIDs { get; set; }
        public bool isCopiedAutomationRuleSet { get; set; }
        public string carClassID { get; set; }
        public string rentalLengthID { get; set; }
        public string weekDaysID { get; set; }
        public string AppliedStartDate { get; set; }
        public string AppliedEndDate { get; set; }
    }
}
