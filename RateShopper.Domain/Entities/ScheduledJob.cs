using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class ScheduledJob : BaseAuditableEntity
    {
        public ScheduledJob()
        {
            this.ScheduledJobMinRates = new List<ScheduledJobMinRates>();
        }

        //public long ID { get; set; }
        public long ScheduledJobFrequencyID { get; set; }
        public Nullable<System.DateTime> LastRunDateTime { get; set; }
        public Nullable<System.DateTime> NextRunDateTime { get; set; }

        [MaxLength(1000)]
        public string CustomHoursToRun { get; set; }
        [MaxLength(1000)]
        public string CustomMinutesToRun { get; set; }

        [MaxLength(500)]
        public string RunTime { get; set; }
        public Nullable<int> RunDay { get; set; }
        [Required]
        public System.DateTime DaysToRunStartDate { get; set; }
        [Required]
        public System.DateTime DaysToRunEndDate { get; set; }
        public bool IsStandardShop { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> StartDateOffset { get; set; }
        public Nullable<int> EndDateOffset { get; set; }
        [Required, MaxLength(500)]
        public string PickUpTime { get; set; }
        [Required, MaxLength(500)]
        public string DropOffTime { get; set; }

        [Required, MaxLength(500)]
        public string ScrapperSourceIDs { get; set; }
        [Required, MaxLength(500)]
        public string LocationBrandIDs { get; set; }
        [Required, MaxLength(500)]
        public string CarClassesIDs { get; set; }
        [Required, MaxLength(500)]
        public string RentalLengthIDs { get; set; }


        [Required]
        public string RequestURL { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsWideGapTemplate { get; set; }
        public bool IsActiveTethering { get; set; }
        public bool ExecutionInProgress { get; set; }

        [MaxLength(1000)]
        public string JobScheduleWeekDays { get; set; }

        [MaxLength(1000)]
        public string TSDUpdateWeekDay { get; set; }
       
        public bool IsDeleted { get; set; }
        public bool IsCustomRuleSet { get; set; }
        public string PostData { get; set; }
        public Nullable<long> ProviderId { get; set; }
        public bool? IsGov { get; set; }
        public bool? IsGovTemplate { get; set; }
        public bool? IsStopByFTB { get; set; }
        public bool CompeteOnBase { get; set; }
        public bool IsOpaqueActive { get; set; }
        public string OpaqueRateCodes { get; set; }

        public virtual ScheduledJobFrequency ScheduledJobFrequency { get; set; }
        public virtual ICollection<ScheduledJobMinRates> ScheduledJobMinRates { get; set; }
        public virtual ICollection<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }
        public virtual ICollection<ScheduledJobOpaqueValues> ScheduledJobOpaqueValues { get; set; }
    }
}
