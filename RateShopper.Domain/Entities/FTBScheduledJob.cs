using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RateShopper.Domain.Entities
{
    public partial class FTBScheduledJob : BaseAuditableEntity
    {
        [Required]
        public long LocationBrandID { get; set; }
        
        public Nullable<long> ScrapperSourceID { get; set; }
        public long ScheduledJobFrequencyID { get; set; }

        public Nullable<System.DateTime> LastRunDateTime { get; set; }
        public Nullable<System.DateTime> NextRunDateTime { get; set; }

        [Required]
        public DateTime DaysToRunStartDate { get; set; }
        [Required]
        public DateTime DaysToRunEndDate { get; set; }

        [MaxLength(500)]
        public string RunTime { get; set; }

        [Required]
        public System.DateTime StartDate { get; set; }
        [Required]
        public System.DateTime EndDate { get; set; }

        [Required]
        public bool IsActiveTethering { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        [Required]
        public bool ExecutionInProgress { get; set; }
        public string JobScheduleWeekDays { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ScheduledJobFrequency ScheduledJobFrequency { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
