using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RateShopper.Domain.Entities
{
    public partial class FTBRate : BaseAuditableEntity
    {
        public FTBRate()
        {
            this.FTBRatesDetail = new List<FTBRatesDetail>();
        }
        public long LocationBrandId { get; set; }
        public DateTime Date { get; set; }
        public bool? IsSplitMonth { get; set; }
        public bool? HasBlackOutPeroid { get; set; }
        public DateTime? BlackOutStartDate { get; set; }
        public DateTime? BlackOutEndDate { get; set; }

        public virtual LocationBrand LocationBrands { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<FTBRatesDetail> FTBRatesDetail { get; set; }
        public virtual ICollection<FTBRatesSplitMonthDetails> FTBRatesSplitMonthDetails { get; set; }

    }
   
}

