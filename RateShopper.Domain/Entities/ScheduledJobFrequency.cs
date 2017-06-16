using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
	public partial class ScheduledJobFrequency : BaseEntity
	{
		public ScheduledJobFrequency()
		{
			this.ScheduledJobs = new List<ScheduledJob>();
            this.FTBScheduledJob = new List<FTBScheduledJob>();
            this.JobTypeFrequencyMapper = new List<JobTypeFrequencyMapper>();
		}

		//public long ID { get; set; }
		[Required, MaxLength(200)]
		public string Name { get; set; }
		public string UIControlID { get; set; }
		public Nullable<int> MinuteInterval { get; set; }
		public Nullable<int> DayInterval { get; set; }
		public virtual ICollection<ScheduledJob> ScheduledJobs { get; set; }
        public virtual ICollection<FTBScheduledJob> FTBScheduledJob { get; set; }
        public virtual ICollection<JobTypeFrequencyMapper> JobTypeFrequencyMapper { get; set; }
	}
}
