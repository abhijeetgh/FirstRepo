using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class JobTypeFrequencyMapper : BaseEntity
    {
        public long ScheduleJobFrequencyId { get; set; }
        public string JobType { get; set; }
        public virtual ScheduledJobFrequency ScheduledJobFrequency { get; set; }
    }
}
