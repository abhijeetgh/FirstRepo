using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class ScheduledJobMinRates : BaseEntity
    {
        //public long ID { get; set; }
        public long ScheduleJobID { get; set; }
        public long CarClassID { get; set; }
        public decimal? DayMin { get; set; }
        public decimal? WeekMin { get; set; }
        public decimal? MonthMin { get; set; }
        public decimal? DayMax { get; set; }
        public decimal? WeekMax { get; set; }
        public decimal? MonthMax { get; set; }
        public decimal? Day2Min { get; set; }
        public decimal? Day2Max { get; set; }
        public decimal? Week2Min { get; set; }
        public decimal? Week2Max { get; set; }
        public decimal? Month2Min { get; set; }
        public decimal? Month2Max { get; set; }
        public string Days1 { get; set; }
        public string Days2 { get; set; }

        public virtual CarClass CarClass { get; set; }
        public virtual ScheduledJob ScheduledJob { get; set; }
    }
}
