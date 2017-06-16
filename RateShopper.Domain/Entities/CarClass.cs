using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RateShopper.Domain.Entities
{
    public partial class CarClass : BaseAuditableEntity
    {
        public CarClass()
        {
            this.GlobalLimitDetails = new List<GlobalLimitDetail>();
            this.GlobalTetherSettings = new List<GlobalTetherSetting>();
            this.LocationBrandCarClass = new List<LocationBrandCarClass>();
            this.RuleSetCarClasses = new List<RuleSetCarClasses>();
            
            this.SearchResults = new List<SearchResult>();
            this.SearchResultSuggestedRates = new List<SearchResultSuggestedRate>();
            this.ScheduledJobMinRates = new List<ScheduledJobMinRates>();
            this.FTBRatesDetail = new List<FTBRatesDetail>();
        }
        
        [Required, MaxLength(50)]
        public string Code { get; set; }

        public string Description { get; set; }
        public string Logo { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<GlobalLimitDetail> GlobalLimitDetails { get; set; }
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        public virtual ICollection<LocationBrandCarClass> LocationBrandCarClass { get; set; }
        public virtual ICollection<RuleSetCarClasses> RuleSetCarClasses { get; set; }
        
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
        public virtual ICollection<ScheduledJobMinRates> ScheduledJobMinRates { get; set; }
        public virtual ICollection<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }
        public virtual ICollection<FTBRatesDetail> FTBRatesDetail { get; set; }
    }
}
