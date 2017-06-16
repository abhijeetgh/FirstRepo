using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class ScheduledJobRuleSets : BaseEntity
    {
        public long? ScheduleJobID { get; set; }
        [Required]
        public long RuleSetID { get; set; }
        [Required]
        public long OriginalRuleSetID { get; set; }
        public string IntermediateID { get; set; }
        public Nullable<DateTime> CreatedDateTime { get; set; }
    }
}
