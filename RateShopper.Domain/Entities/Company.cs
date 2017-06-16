using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class Company : BaseAuditableEntity
    {
        public Company()
        {
            this.GlobalTetherSettings = new List<GlobalTetherSetting>();
            this.GlobalTetherSettings1 = new List<GlobalTetherSetting>();
            //this.ScheduledJobTetherings = new List<ScheduledJobTetherings>();
            //this.ScheduledJobTetherings1 = new List<ScheduledJobTetherings>();
            this.LocationCompany = new List<LocationCompany>();
            this.LocationBrands = new List<LocationBrand>();
            this.RuleSetGroupCompanies = new List<RuleSetGroupCompany>();
            this.SearchResults = new List<SearchResult>();
            this.SearchResultSuggestedRates = new List<SearchResultSuggestedRate>();
        }

        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(500)]
        public string Name { get; set; }
        public string Logo { get; set; }
        [Required]
        public bool IsBrand { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        //end

        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings1 { get; set; }
        public virtual ICollection<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }
        public virtual ICollection<ScheduledJobTetherings> ScheduledJobTetherings1 { get; set; }
        public virtual ICollection<LocationCompany> LocationCompany { get; set; }
        public virtual ICollection<LocationBrand> LocationBrands { get; set; }
        public virtual ICollection<RuleSetGroupCompany> RuleSetGroupCompanies { get; set; }
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
    }
}
