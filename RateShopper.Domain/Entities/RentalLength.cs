using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RentalLength : BaseEntity
    {
        public RentalLength()
        {
            this.LocationBrandRentalLength = new List<LocationBrandRentalLength>();
            this.RuleSetRentalLength = new List<RuleSetRentalLength>();
            
            this.SearchResults = new List<SearchResult>();
            this.SearchResultSuggestedRates = new List<SearchResultSuggestedRate>();
            this.FTBRatesDetail = new List<FTBRatesDetail>();
        }

        //public long ID { get; set; }
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public long MappedID { get; set; }
        public long? AssociateMappedId { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<bool> IsSearchEnable { get; set; } 

        public virtual ICollection<LocationBrandRentalLength> LocationBrandRentalLength { get; set; }
        public virtual ICollection<RuleSetRentalLength> RuleSetRentalLength { get; set; }
        
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }

        public virtual ICollection<FTBRatesDetail> FTBRatesDetail { get; set; }
    }
}
