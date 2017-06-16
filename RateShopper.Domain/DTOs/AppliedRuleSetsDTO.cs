using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class AppliedRuleSetsDTO
    {
        public long Id { get; set; }
        public long RuleSetId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsWideGap { get; set; }
        public bool IsGOV { get; set; }
    }

    public class AppliedRuleSetDetailsDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsWideGap { get; set; }
        public string Companies { get; set; }
        public string CarClasses { get; set; }
        public string RentalLengths { get; set; }
        public string DaysOfWeek { get; set; }
        public bool IsGOV { get; set; }
    }
}
