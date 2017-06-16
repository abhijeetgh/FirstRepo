using RateShopper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using System.Data.Entity;

namespace RateShopper.Data
{
    public interface IEZRACRateShopperContext : IBaseContext
    {
        IDbSet<CarClass> CarClasses { get; set; }
        IDbSet<Company> Companies { get; set; }
        IDbSet<Formula> Formulas { get; set; }
        IDbSet<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        IDbSet<LocationCompany> LocationCompany { get; set; }
        IDbSet<GlobalLimitDetail> GlobalLimitDetails { get; set; }
        IDbSet<GlobalLimit> GlobalLimits { get; set; }
        IDbSet<LocationBrand> LocationBrands { get; set; }
        IDbSet<LocationBrandCarClass> LocationBrandCarClass { get; set; }
        IDbSet<LocationBrandRentalLength> LocationBrandRentalLength { get; set; }
        IDbSet<Location> Locations { get; set; }
        IDbSet<RangeInterval> RangeIntervals { get; set; }
        IDbSet<RateCode> RateCodes { get; set; }
        IDbSet<RateCodeDateRange> RateCodeDateRanges { get; set; }
        IDbSet<RateSystem> RateSystems { get; set; }
        IDbSet<RentalLength> RentalLengths { get; set; }
        IDbSet<RuleSetCarClasses> RuleSetCarClasses { get; set; }
        IDbSet<RuleSetGroup> RuleSetGroup { get; set; }
        IDbSet<RuleSetGroupCompany> RuleSetGroupCompanies { get; set; }
        IDbSet<RuleSetRentalLength> RuleSetRentalLength { get; set; }
        IDbSet<RuleSetWeekDay> RuleSetWeekDay { get; set; }
        IDbSet<RuleSetDefaultSetting> RuleSetDefaultSettings { get; set; }
        IDbSet<RuleSetGapSetting> RuleSetGapSettings { get; set; }
        IDbSet<RuleSet> RuleSets { get; set; }
        IDbSet<RuleSetsApplied> RuleSetsApplied { get; set; }


        IDbSet<ScheduledJobMinRates> ScheduledJobMinRates { get; set; }
        IDbSet<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }
        IDbSet<ScheduledJobRuleSets> ScheduledJobRuleSets { get; set; }
        IDbSet<ScheduledJobFrequency> ScheduledJobFrequencies { get; set; }
        IDbSet<ScheduledJob> ScheduledJobs { get; set; }
        IDbSet<ScrapperSource> ScrapperSources { get; set; }
        IDbSet<SearchResult> SearchResults { get; set; }
        IDbSet<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
        IDbSet<SearchSummary> SearchSummaries { get; set; }
        IDbSet<sysdiagram> sysdiagrams { get; set; }
        IDbSet<TSDTransaction> TSDTransactions { get; set; }
        IDbSet<UserLocationBrands> UserLocationBrands { get; set; }
        IDbSet<UserScrapperSources> UserScrapperSources { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<UserRole> UserRoles { get; set; }
        IDbSet<LocationPricingManager> LocationPricingManagers { get; set; }
        IDbSet<UserPermissions> UserPermissions { get; set; }
        IDbSet<UserPermissionMapper> UserPermissionMappers { get; set; }
        IDbSet<WeekDay> WeekDays { get; set; }
        IDbSet<SearchResultProcessedData> SearchResultProcessedDatas { get; set; }
        IDbSet<SearchResultRawData> SearchResultRawDatas { get; set; }
        IDbSet<Statuses> StatuseDesc { get; set; }
        IDbSet<SearchJobUpdateAll> SearchJobUpdateAlls { get; set; }
        IDbSet<QuickView> QuickView { get; set; }
        IDbSet<QuickViewResults> QuickViewResults { get; set; }
        IDbSet<Domain.Entities.Providers> Providers { get; set; }
        IDbSet<PostXMLLogs> PostXMLLogs { get; set; }
        IDbSet<PostJSONRequestLog> PostJSONRequestLogs { get; set; }
        IDbSet<PostJSONResponseLog> PostJSONResponseLogs { get; set; }

        IDbSet<FTBRate> FTBRates { get; set; }
        IDbSet<FTBRatesDetail> FTBRatesDetails { get; set; }
        IDbSet<FTBTarget> FTBTargets { get; set; }
        IDbSet<FTBTargetsDetail> FTBTargetsDetails { get; set; }
        IDbSet<FTBScheduledJob> FTBScheduledJobs { get; set; }
        IDbSet<JobTypeFrequencyMapper> JobTypeFrequencyMappers { get; set; }
        IDbSet<SplitMonthDetails> SplitMonthDetails { get; set; }
        IDbSet<FTBRatesTSDLogs> FTBRatesTSDLogs { get; set; }
        IDbSet<FTBRatesSplitMonthDetails> FTBRatesSplitMonthDetails { get; set; }

        IDbSet<QuickviewCarClassGroup> QuickviewCarClassGroup { get; set; }
        IDbSet<QuickViewGroup> QuickViewGroups { get; set; }
        IDbSet<QuickViewGroupCompanies> QuickViewGroupCompanies { get; set; }
        IDbSet<QuickViewGapDevSettings> QuickViewGapDevSettings { get; set; }
        IDbSet<ScheduledJobOpaqueValues> ScheduledJobOpaqueValues { get; set; }

        Task<IEnumerable<T>> ExecuteSQLQuery<T>(string query, params object[] parameters);
        int ExecuteSqlCommand(string query, params object[] parameters);
    }
}

