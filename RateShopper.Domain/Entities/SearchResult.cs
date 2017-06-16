using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateShopper.Domain.Entities
{
    public partial class SearchResult : BaseEntity
    {
        //public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        public long ScrapperSourceID { get; set; }
        public long LocationID { get; set; }
        public long RentalLengthID { get; set; }
        //[Required]
        //public System.DateTime Date { get; set; }
        public long CarClassID { get; set; }
        public long CompanyID { get; set; }
        
        //Dummy field for manipulation
       [NotMapped]
        public int CompanyRankBasedOnMCcarRate { get; set; }

        public Nullable<decimal> BaseRate { get; set; }
        public Nullable<decimal> TotalRate { get; set; }
        //public long RuleSetID { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public virtual CarClass CarClass { get; set; }
        public virtual Company Company { get; set; }
        public virtual Location Location { get; set; }
        public virtual RentalLength RentalLength { get; set; }
        //public virtual RuleSet RuleSet { get; set; }
        public virtual ScrapperSource ScrapperSource { get; set; }
        public virtual SearchSummary SearchSummary { get; set; }
        //public virtual User User { get; set; }
        //public virtual User User1 { get; set; }
    }
}
