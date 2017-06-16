using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class GlobalLimitDetail: BaseEntity
    {        
        public long GlobalLimitID { get; set; }
        public long CarClassID { get; set; }
        public Nullable<decimal> DayMin { get; set; }
        public Nullable<decimal> DayMax { get; set; }
        public Nullable<decimal> WeekMin { get; set; }
        public Nullable<decimal> WeekMax { get; set; }
        public Nullable<decimal> MonthMin { get; set; }
        public Nullable<decimal> MonthMax { get; set; }
        public virtual CarClass CarClass { get; set; }
        public virtual GlobalLimit GlobalLimit { get; set; }
    }
}
