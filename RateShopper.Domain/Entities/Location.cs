using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class Location : BaseAuditableEntity
    {
        public Location()
        {
            this.GlobalTetherSettings = new List<GlobalTetherSetting>();
            this.LocationCompany = new List<LocationCompany>();
            this.LocationBrands = new List<LocationBrand>();
            this.SearchResults = new List<SearchResult>();
            this.SearchResultSuggestedRates = new List<SearchResultSuggestedRate>();
        }
        [Required,MaxLength(50)]
        public string Code { get; set; }        
        [Required]
        public bool IsDeleted { get; set; }
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        public virtual ICollection<LocationCompany> LocationCompany { get; set; }
        public virtual ICollection<LocationBrand> LocationBrands { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
    }
}
