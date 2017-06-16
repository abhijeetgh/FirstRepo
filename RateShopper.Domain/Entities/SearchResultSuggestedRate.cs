using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class SearchResultSuggestedRate : BaseEntity
    {
        //public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        public long LocationID { get; set; }
        public long BrandID { get; set; }
        public long RentalLengthID { get; set; }
        [Required]
        public System.DateTime Date { get; set; }
        public long CarClassID { get; set; }
        [Required]
        public decimal BaseRate { get; set; }
        public Nullable<decimal> MinBaseRate { get; set; }
        public Nullable<decimal> MaxBaseRate { get; set; }
       
        
        public Nullable<decimal> TotalRate { get; set; }
        public Nullable<long> RuleSetID { get; set; }
        public string RuleSetName { get; set; }
		
		public DateTime? TSDUpdateDateTime { get; set; }
		public long? TSDUpdatedBy { get; set; }

        public Nullable<decimal> CompanyBaseRate { get; set; }
        public Nullable<decimal> CompanyTotalRate { get; set; }

        public bool NoCompitetorRateFound { get; set; }

        public virtual CarClass CarClass { get; set; }
        public virtual Company Company { get; set; }
        public virtual Location Location { get; set; }
        public virtual RentalLength RentalLength { get; set; }
        public virtual SearchSummary SearchSummary { get; set; }
    }
}
