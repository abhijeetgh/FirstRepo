using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class ScheduledJobOpaqueValues:BaseEntity
    {
        public long CarClassId { get; set; }
        public long ScheduledJobId { get; set; }
        public decimal PercentValue { get; set; }

        public virtual CarClass CarClass { get; set; }
        public virtual ScheduledJob ScheduledJob { get; set; }
    }
}
