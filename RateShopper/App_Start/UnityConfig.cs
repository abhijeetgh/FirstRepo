using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using RateShopper.Services.Data;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using RateShopper.Core.Cache;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using RateShopper.Providers.Interface;

namespace RateShopper.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            using (var container = new UnityContainer())
            {
                RegisterTypes(container);
                return container;
            }
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
             container.RegisterType<IUserService, UserService>();
             container.RegisterType<ICarClassService, CarClassService>();
             container.RegisterType<ICacheManager, InMemoryCacheManager>();
             if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UsePerRequestLiftimeManager"]))
             {
                 container.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>(new PerRequestLifetimeManager());
             }
             else
             {
                 container.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>();
             }
             container.RegisterType<IGlobalTetherSettingService, GlobalTetherSettingService>();
             container.RegisterType<IGlobalLimitService, GlobalLimitService>();
             container.RegisterType<IGlobalLimitDetailService, GlobalLimitDetailService>();
             container.RegisterType<IFormulaService, FormulaService>();
             container.RegisterType<IStatusesService, StatusesService>();
             container.RegisterType<ISearchResultsService, SearchResultsService>();
             container.RegisterType<ISearchResultRawDataService, SearchResultRawDataService>();
             container.RegisterType<ISearchResultProcessedDataService, SearchResultProcessedDataService>();
             container.RegisterType<ISearchSummaryService, SearchSummaryService>();
             container.RegisterType<ILocationService, LocationsService>();
             container.RegisterType<ICompanyService, CompanyService>();
             container.RegisterType<ILocationBrandService, LocationBrandService>();
             container.RegisterType<ILocationCompanyService, LocationCompanyService>();
             container.RegisterType<IUserRolesService, UserRolesService>();
             container.RegisterType<IScrapperSourceService, ScrapperSourceService>();
             container.RegisterType<IRentalLengthService, RentalLengthService>();

             container.RegisterType<IRuleSetService, RuleSetService>();
             container.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
             container.RegisterType<IRuleSetGroupCompanyService, RuleSetGroupCompanyService>();
             container.RegisterType<IRuleSetGroupService, RuleSetGroupService>();
             container.RegisterType<IRuleSetGapSettingService, RuleSetGapSettingService>();
             container.RegisterType<IRuleSetRentalLengthService, RuleSetRentalLengthService>();
             container.RegisterType<IRuleSetsAppliedService, RuleSetsAppliedService>();
             container.RegisterType<IRuleSetWeekDayService, RuleSetWeekDayService>();
             container.RegisterType<IRuleSetDefaultSettingService, RuleSetDefaultSettingService>();
             container.RegisterType<IWeekDayService, WeekDayService>();
             container.RegisterType<IUserScrapperSourcesService, UserScrapperSourcesService>();
             container.RegisterType<IRangeIntervalsService, RangeIntervalsService>();
             container.RegisterType<IUserLocationBrandsService, UserLocationBrandsService>();
                        
            container.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
            container.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();
            container.RegisterType<ILocationBrandCarClassService, LocationBrandCarClassService>();
            
                        
            container.RegisterType<ITSDTransactionsService, TSDTransactionService>();
            container.RegisterType<IScheduledJobService, ScheduledJobService>();
			container.RegisterType<IScheduledJobFrequencyService, ScheduledJobFrequencyService>();
            container.RegisterType<IScheduledJobMinRatesService, ScheduledJobMinRatesService>();
            container.RegisterType<IScheduledJobTetheringsService, ScheduledJobTetheringsService>();
            container.RegisterType<IQuickViewService, QuickViewService>();
            container.RegisterType<IQuickViewResultsService, QuickViewResultService>();
            container.RegisterType<IProviderService, Providers.ProviderService>();
            container.RegisterType<IRequestProcessor, Providers.Helper.RequestProcessor>();
            container.RegisterType<IRateHighway, Providers.Providers.RateHighway>();
            container.RegisterType<IScrappingService, Providers.Providers.ScrappingService>();
            container.RegisterType<IProvidersService, ProvidersService>();            
            container.RegisterType<ISearchJobUpdateAllService, SearchJobUpdateAllService>();
            container.RegisterType<ICallbackResponseService, CallbackResponseService>();
            container.RegisterType<IScrappingServersService, ScrappingServersService>();

            container.RegisterType<IFTBScheduleJobService, FTBScheduleJobService>();
            container.RegisterType<IJobTypeMapperService, JobTypeMapperService>();
            container.RegisterType<IFTBTargetService, FTBTargetService>();
            container.RegisterType<IFTBRatesService, FTBRatesService>();
            container.RegisterType<IFTBRatesDetailService, FTBRateDetailsService>();
            container.RegisterType<ISplitMonthDetailsService, SplitMonthDetailsService>();
            container.RegisterType<IFTBTargetsDetailService, FTBTargetsDetailService>();
            container.RegisterType<IUserPermissionService, UserPermissionService>();
            container.RegisterType<IFTBRatesSplitMonthsService, FTBRatesSplitMonthsService>();
            container.RegisterType<IPostJSONLogService, PostJSONLogService>();
            container.RegisterType<IRateCodeService, RateCodeService>();
            container.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();

            container.RegisterType<IRateCodeDateRangeService, RateCodeDateRangeService>();
                         
        }
    }
}
