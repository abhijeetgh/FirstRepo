using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class User : BaseAuditableEntity
    {
        public User()
        {
            this.CarClasses = new List<CarClass>();
            this.CarClasses1 = new List<CarClass>();
            this.Companies = new List<Company>();
            this.Companies1 = new List<Company>();
            this.Formulas = new List<Formula>();
            this.Formulas1 = new List<Formula>();
            this.GlobalLimits = new List<GlobalLimit>();
            this.GlobalLimits1 = new List<GlobalLimit>();
            this.GlobalTetherSettings = new List<GlobalTetherSetting>();
            this.GlobalTetherSettings1 = new List<GlobalTetherSetting>();
            this.LocationBrands = new List<LocationBrand>();
            this.LocationBrands1 = new List<LocationBrand>();
            this.LocationBrandRentalLength = new List<LocationBrandRentalLength>();
            this.LocationBrandRentalLength1 = new List<LocationBrandRentalLength>();
            this.Locations = new List<Location>();
            this.Locations1 = new List<Location>();
            this.RateCodes = new List<RateCode>();
            this.RateCodes1 = new List<RateCode>();
            this.RateSystems = new List<RateSystem>();
            this.RateSystems1 = new List<RateSystem>();
            this.RuleSetDefaultSettings = new List<RuleSetDefaultSetting>();
            this.RuleSetDefaultSettings1 = new List<RuleSetDefaultSetting>();
            this.RuleSets = new List<RuleSet>();
            this.RuleSets1 = new List<RuleSet>();
            //this.SearchResults = new List<SearchResult>();
            //this.SearchResults1 = new List<SearchResult>();
            this.SearchSummaries = new List<SearchSummary>();
            this.SearchSummaries1 = new List<SearchSummary>();
            this.TSDTransactions = new List<TSDTransaction>();
            this.TSDTransactions1 = new List<TSDTransaction>();
            this.UserLocationBrands = new List<UserLocationBrands>();
            this.UserScrapperSources = new List<UserScrapperSources>();
            this.FTBRate = new List<FTBRate>();
            this.FTBRate1 = new List<FTBRate>();
            this.FTBTarget = new List<FTBTarget>();
            this.FTBTarget1 = new List<FTBTarget>();
            this.FTBScheduledJob = new List<FTBScheduledJob>();
            this.FTBScheduledJob1 = new List<FTBScheduledJob>();
        }

        public long UserRoleID { get; set; }
        [Required,MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required,MaxLength(100)]
        public string UserName { get; set; }
        [MaxLength(500)]
        public string EmailAddress { get; set; }
        [Required, MaxLength(500)]
        public string Password { get; set; }
        public bool IsAutomationUser { get; set; }
        public bool IsTetheringAccess { get; set; }
        public Nullable<bool> IsTSDUpdateAccess { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<CarClass> CarClasses { get; set; }
        public virtual ICollection<CarClass> CarClasses1 { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Company> Companies1 { get; set; }
        public virtual ICollection<Formula> Formulas { get; set; }
        public virtual ICollection<Formula> Formulas1 { get; set; }
        public virtual ICollection<GlobalLimit> GlobalLimits { get; set; }
        public virtual ICollection<GlobalLimit> GlobalLimits1 { get; set; }        
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        public virtual ICollection<GlobalTetherSetting> GlobalTetherSettings1 { get; set; }
        public virtual ICollection<LocationBrand> LocationBrands { get; set; }
        public virtual ICollection<LocationBrand> LocationBrands1 { get; set; }
        public virtual ICollection<LocationBrandRentalLength> LocationBrandRentalLength { get; set; }
        public virtual ICollection<LocationBrandRentalLength> LocationBrandRentalLength1 { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Location> Locations1 { get; set; }
        public virtual ICollection<RateCode> RateCodes { get; set; }
        public virtual ICollection<RateCode> RateCodes1 { get; set; }
        public virtual ICollection<RateSystem> RateSystems { get; set; }
        public virtual ICollection<RateSystem> RateSystems1 { get; set; }
        public virtual ICollection<RuleSetDefaultSetting> RuleSetDefaultSettings { get; set; }
        public virtual ICollection<RuleSetDefaultSetting> RuleSetDefaultSettings1 { get; set; }
        public virtual ICollection<RuleSet> RuleSets { get; set; }
        public virtual ICollection<RuleSet> RuleSets1 { get; set; }
        //public virtual ICollection<SearchResult> SearchResults { get; set; }
        //public virtual ICollection<SearchResult> SearchResults1 { get; set; }
        public virtual ICollection<SearchSummary> SearchSummaries { get; set; }
        public virtual ICollection<SearchSummary> SearchSummaries1 { get; set; }
        public virtual ICollection<TSDTransaction> TSDTransactions { get; set; }
        public virtual ICollection<TSDTransaction> TSDTransactions1 { get; set; }
        public virtual ICollection<UserLocationBrands> UserLocationBrands { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<UserScrapperSources> UserScrapperSources { get; set; }

        public virtual ICollection<FTBRate> FTBRate { get; set; }
        public virtual ICollection<FTBRate> FTBRate1 { get; set; }
        public virtual ICollection<FTBTarget> FTBTarget { get; set; }
        public virtual ICollection<FTBTarget> FTBTarget1 { get; set; }
        public virtual ICollection<FTBScheduledJob> FTBScheduledJob { get; set; }
        public virtual ICollection<FTBScheduledJob> FTBScheduledJob1 { get; set; }

        public virtual ICollection<UserPermissionMapper> UserPermissionMappers { get; set; }
    }
}
