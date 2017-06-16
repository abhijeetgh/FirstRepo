using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ScheduledJobRuleSetDTO
    {
        public long ID { get; set; }
        public long? ScheduleJobID { get; set; }        
        public long RuleSetID { get; set; }        
        public long OriginalRuleSetID { get; set; }
        public string IntermediateID { get; set; }
        public Nullable<DateTime> CreatedDateTime { get; set; }
    }
}
