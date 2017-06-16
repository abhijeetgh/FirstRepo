using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using RateShopper.Data.Mapping;
using RateShopper;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateShopper.Data
{
    public class EZRACRateShopperContext : DbContext, IEZRACRateShopperContext
    {
        private List<string> _modifiedEntries;

        public EZRACRateShopperContext()
            : base("Name=EZRACRateShopperContext")
        {
            Database.SetInitializer<EZRACRateShopperContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            _modifiedEntries = new List<string>();

            var adapter = (IObjectContextAdapter)this;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = 0;
        }

        public IDbSet<CarClass> CarClasses { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<Formula> Formulas { get; set; }
        public IDbSet<GlobalLimit> GlobalLimits { get; set; }
        public IDbSet<GlobalLimitDetail> GlobalLimitDetails { get; set; }
        public IDbSet<GlobalTetherSetting> GlobalTetherSettings { get; set; }
        public IDbSet<LocationCompany> LocationCompany { get; set; }
        public IDbSet<LocationBrand> LocationBrands { get; set; }
        public IDbSet<LocationBrandCarClass> LocationBrandCarClass { get; set; }
        public IDbSet<LocationBrandRentalLength> LocationBrandRentalLength { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<RangeInterval> RangeIntervals { get; set; }
        public IDbSet<RateCode> RateCodes { get; set; }
        public IDbSet<RateCodeDateRange> RateCodeDateRanges { get; set; }
        public IDbSet<RateSystem> RateSystems { get; set; }
        public IDbSet<RentalLength> RentalLengths { get; set; }
        public IDbSet<RuleSetCarClasses> RuleSetCarClasses { get; set; }
        public IDbSet<RuleSetGroup> RuleSetGroup { get; set; }
        public IDbSet<RuleSetGroupCompany> RuleSetGroupCompanies { get; set; }
        public IDbSet<RuleSetRentalLength> RuleSetRentalLength { get; set; }
        public IDbSet<RuleSetWeekDay> RuleSetWeekDay { get; set; }
        public IDbSet<RuleSetDefaultSetting> RuleSetDefaultSettings { get; set; }
        public IDbSet<RuleSetGapSetting> RuleSetGapSettings { get; set; }
        public IDbSet<RuleSet> RuleSets { get; set; }
        public IDbSet<RuleSetsApplied> RuleSetsApplied { get; set; }
        
        
        public IDbSet<ScheduledJobMinRates> ScheduledJobMinRates { get; set; }
        public IDbSet<ScheduledJobTetherings> ScheduledJobTetherings { get; set; }
        public IDbSet<ScheduledJobOpaqueValues> ScheduledJobOpaqueValues { get; set; }
        public IDbSet<ScheduledJobRuleSets> ScheduledJobRuleSets { get; set; }
        
        
        public IDbSet<ScheduledJobFrequency> ScheduledJobFrequencies { get; set; }
        public IDbSet<ScheduledJob> ScheduledJobs { get; set; }
		
        public IDbSet<ScrapperSource> ScrapperSources { get; set; }
        public IDbSet<SearchResult> SearchResults { get; set; }
        public IDbSet<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
        public IDbSet<SearchSummary> SearchSummaries { get; set; }
        public IDbSet<sysdiagram> sysdiagrams { get; set; }
        public IDbSet<TSDTransaction> TSDTransactions { get; set; }
        public IDbSet<UserLocationBrands> UserLocationBrands { get; set; }
        public IDbSet<UserScrapperSources> UserScrapperSources { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<WeekDay> WeekDays { get; set; }
        public IDbSet<UserRole> UserRoles { get; set; }
        public IDbSet<LocationPricingManager> LocationPricingManagers { get; set; }
        public IDbSet<UserPermissions> UserPermissions { get; set; }
        public IDbSet<UserPermissionMapper> UserPermissionMappers { get; set; }
        public IDbSet<SearchResultProcessedData> SearchResultProcessedDatas { get; set; }
        public IDbSet<SearchResultRawData> SearchResultRawDatas { get; set; }
        public IDbSet<Statuses> StatuseDesc { get; set; }
        public IDbSet<SearchJobUpdateAll> SearchJobUpdateAlls { get; set; }

        public IDbSet<QuickView> QuickView { get; set; }
        public IDbSet<QuickViewResults> QuickViewResults { get; set; }

        public IDbSet<Domain.Entities.Providers> Providers { get; set; }
        public IDbSet<CallbackResponse> CallbackResponse { get; set; }
        public IDbSet<ScrappingServers> ScrappingServers { get; set; }
        public IDbSet<PostXMLLogs> PostXMLLogs { get; set; }
        public IDbSet<PostJSONRequestLog> PostJSONRequestLogs { get; set; }
        public IDbSet<PostJSONResponseLog> PostJSONResponseLogs { get; set; }

        public IDbSet<FTBRate> FTBRates { get; set; }
        public IDbSet<FTBRatesDetail> FTBRatesDetails { get; set; }
        public IDbSet<FTBTarget> FTBTargets { get; set; }
        public IDbSet<FTBTargetsDetail> FTBTargetsDetails { get; set; }
        public IDbSet<FTBScheduledJob> FTBScheduledJobs { get; set; }
        public IDbSet<SplitMonthDetails> SplitMonthDetails { get; set; }
        public IDbSet<JobTypeFrequencyMapper> JobTypeFrequencyMappers { get; set; }
        public IDbSet<FTBRatesTSDLogs> FTBRatesTSDLogs { get; set; }
        public IDbSet<FTBRatesSplitMonthDetails> FTBRatesSplitMonthDetails { get; set; }

        public IDbSet<QuickviewCarClassGroup> QuickviewCarClassGroup { get; set; }
        public IDbSet<QuickViewGroup> QuickViewGroups { get; set; }
        public IDbSet<QuickViewGroupCompanies> QuickViewGroupCompanies { get; set; }
        public IDbSet<QuickViewGapDevSettings> QuickViewGapDevSettings { get; set; }       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CarClassMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new FormulaMap());
            modelBuilder.Configurations.Add(new GlobalLimitMap());
            modelBuilder.Configurations.Add(new GlobalLimitDetailMap());
            modelBuilder.Configurations.Add(new GlobalTetherSettingMap());
            modelBuilder.Configurations.Add(new LocationCompanyMap());
            modelBuilder.Configurations.Add(new LocationBrandMap());
            modelBuilder.Configurations.Add(new LocationBrandCarClassMap());
            modelBuilder.Configurations.Add(new LocationBrandRentalLengthMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new RangeIntervalMap());
            modelBuilder.Configurations.Add(new RateCodeMap());
            modelBuilder.Configurations.Add(new RateCodeDateRangeMap());
            modelBuilder.Configurations.Add(new RateSystemMap());
            modelBuilder.Configurations.Add(new RentalLengthMap());
            modelBuilder.Configurations.Add(new RuleSetCarClassesMap());
            modelBuilder.Configurations.Add(new RuleSetGroupMap());
            modelBuilder.Configurations.Add(new RuleSetGroupCompanyMap());
            modelBuilder.Configurations.Add(new RuleSetRentalLengthMap());
            modelBuilder.Configurations.Add(new RuleSetWeekDayMap());
            modelBuilder.Configurations.Add(new RuleSetDefaultSettingMap());
            modelBuilder.Configurations.Add(new RuleSetGapSettingMap());
            modelBuilder.Configurations.Add(new RuleSetMap());
            modelBuilder.Configurations.Add(new RuleSetsAppliedMap());
            
            
            modelBuilder.Configurations.Add(new ScheduledJobMinRatesMap());
            modelBuilder.Configurations.Add(new ScheduledJobTetheringsMap());
            modelBuilder.Configurations.Add(new ScheduledJobRuleSetsMap());
            modelBuilder.Configurations.Add(new ScheduledJobOpaqueValuesMap());
						
            modelBuilder.Configurations.Add(new ScheduledJobFrequencyMap());
            modelBuilder.Configurations.Add(new ScheduledJobMap());
            
            modelBuilder.Configurations.Add(new ScrapperSourceMap());
            modelBuilder.Configurations.Add(new SearchResultMap());
            modelBuilder.Configurations.Add(new SearchResultSuggestedRateMap());
            modelBuilder.Configurations.Add(new SearchSummaryMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new TSDTransactionMap());
            modelBuilder.Configurations.Add(new UserLocationBrandsMap());
            modelBuilder.Configurations.Add(new UserScrapperSourcesMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserPermissionsMap());
            modelBuilder.Configurations.Add(new UserPermissionMapperMap());
            modelBuilder.Configurations.Add(new WeekDayMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new LocationPricingManagerMap());
            modelBuilder.Configurations.Add(new SearchResultProcessedDataMap());
            modelBuilder.Configurations.Add(new SearchResultRawDataMap());
            modelBuilder.Configurations.Add(new StatusMap());
            modelBuilder.Configurations.Add(new QuickViewMap());
            modelBuilder.Configurations.Add(new QuickViewResultsMap());
            modelBuilder.Configurations.Add(new ProvidersMap());
            modelBuilder.Configurations.Add(new SearchJobUpdateAllMap());
            modelBuilder.Configurations.Add(new CallbackResponseMap());
            modelBuilder.Configurations.Add(new ScrappingServersMap());
            modelBuilder.Configurations.Add(new PostXMLLogsMap());
            modelBuilder.Configurations.Add(new PostJSONRequestLogMap());
            modelBuilder.Configurations.Add(new PostJSONResponseLogMap());

            modelBuilder.Configurations.Add(new FTBRateMap());
            modelBuilder.Configurations.Add(new FTBRatesDetailMap());
            modelBuilder.Configurations.Add(new FTBTargetMap());
            modelBuilder.Configurations.Add(new FTBTagetsDetailMap());
            modelBuilder.Configurations.Add(new FTBScheduledJobMap());
            modelBuilder.Configurations.Add(new SplitMonthDetailsMap());
            modelBuilder.Configurations.Add(new JobTypeFrequencyMapperMap());
            modelBuilder.Configurations.Add(new FTBRatesTSDLogsMap());
            modelBuilder.Configurations.Add(new FTBRatesSplitMonthDetailsMap());

            modelBuilder.Configurations.Add(new QuickviewCarClassGroupMap());
            modelBuilder.Configurations.Add(new QuickViewGroupMap());
            modelBuilder.Configurations.Add(new QuickViewGroupCompaniesMap());
            modelBuilder.Configurations.Add(new QuickViewGapDevSettingsMap());
        }

        public async Task<IEnumerable<T>> ExecuteSQLQuery<T>(string query, params object[] parameters)
        {
            return await this.Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        public int ExecuteSqlCommand(string query, params object[] parameters)
        {
            return this.Database.ExecuteSqlCommand(query, parameters);
        }
    }
}
