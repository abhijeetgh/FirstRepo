using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateShopper.Domain.Entities
{
    public partial class LocationBrand:BaseAuditableEntity
    {
        public LocationBrand()
        {
            this.Formulas = new List<Formula>();
            this.GlobalLimits = new List<GlobalLimit>();
            this.LocationBrandRentalLength = new List<LocationBrandRentalLength>();
            this.LocationBrandCarClass = new List<LocationBrandCarClass>();
            this.RuleSets = new List<RuleSet>();            
            this.TSDTransactions = new List<TSDTransaction>();
            this.UserLocationBrands = new List<UserLocationBrands>();

            this.FTBRate = new List<FTBRate>();
            this.FTBTarget = new List<FTBTarget>();
        }

        //public long ID { get; set; }
        public long BrandID { get; set; }
        public long LocationID { get; set; }
        public Nullable<decimal> WeeklyExtraDenom { get; set; }
        public Nullable<decimal> DailyExtraDayFactor { get; set; }
        [Required, MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(200)]
        public string TSDCustomerNumber { get; set; }
        [MaxLength(200)]
        public string TSDPassCode { get; set; }
        public bool IsDeleted { get; set; }
        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        [Required, MaxLength(50)]
        public string LocationBrandAlias { get; set; }
        [Required]
        public bool UseLORRateCode { get; set; }
        [MaxLength(20)]
        public string BranchCode { get; set; }

        public string CompetitorCompanyIDs { get; set; }
        public string QuickViewCompetitorCompanyIds { get; set; }
        public bool IsFTBDominantBrand { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Formula> Formulas { get; set; }
        public virtual ICollection<GlobalLimit> GlobalLimits { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<LocationBrandRentalLength> LocationBrandRentalLength { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<LocationBrandCarClass> LocationBrandCarClass { get; set; }
        public virtual ICollection<RuleSet> RuleSets { get; set; }
        
        public virtual ICollection<TSDTransaction> TSDTransactions { get; set; }
        public virtual ICollection<UserLocationBrands> UserLocationBrands { get; set; }
        public virtual ICollection<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }

        public virtual ICollection<FTBRate> FTBRate { get; set; }
        public virtual ICollection<FTBTarget> FTBTarget { get; set; }
    }
}
