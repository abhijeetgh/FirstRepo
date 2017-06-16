using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RateShopper.Domain.Entities
{
    public partial class FTBRatesDetail : BaseEntity
    {
        public long FTBRatesId { get; set; }
        public long RentalLengthId { get; set; }
        public long CarClassId { get; set; }
        public decimal? Sunday { get; set; }
        public decimal? Monday { get; set; }
        public decimal? Tuesday { get; set; }
        public decimal? Wednesday { get; set; }
        public decimal? Thursday { get; set; }
        public decimal? Friday { get; set; }
        public decimal? Saturday { get; set; }
        public int SplitPartId { get; set; }

        public virtual FTBRate FTBRates { get; set; }
        public virtual CarClass CarClasses { get; set; }
        public virtual RentalLength RentalLengths { get; set; }
    }
}
