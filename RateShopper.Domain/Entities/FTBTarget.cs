using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class FTBTarget : BaseAuditableEntity
    {
        public FTBTarget()
        {
            this.FTBTargetsDetail = new List<FTBTargetsDetail>();
        }

        public long LocationBrandId { get; set; }
        public DateTime Date { get; set; }
        public long DayOfWeekId { get; set; }
        public Nullable<long> Target { get; set; }

        public virtual LocationBrand LocationBrands { get; set; }
        public virtual WeekDay WeekDays { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<FTBTargetsDetail> FTBTargetsDetail { get; set; }
    }
}
