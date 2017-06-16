using System.Threading.Tasks;
using RateShopper.Services.Helper;
using RateShopper.Services.Data;
using RateShopper.Data;
using RateShopper.Core.Cache;
using Microsoft.Practices.Unity;
using System;
namespace RateShopper.QuickView.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IUnityContainer unityContainer = new UnityContainer();
                unityContainer.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>();
                unityContainer.RegisterType<ICacheManager, InMemoryCacheManager>();
                unityContainer.RegisterType<IStatusesService, StatusesService>();
                unityContainer.RegisterType<ICompanyService, CompanyService>();
                unityContainer.RegisterType<ICarClassService, CarClassService>();
                unityContainer.RegisterType<IRentalLengthService, RentalLengthService>();
                unityContainer.RegisterType<ILocationService, LocationsService>();
                unityContainer.RegisterType<IScrapperSourceService, ScrapperSourceService>();
                unityContainer.RegisterType<IUserScrapperSourcesService, UserScrapperSourcesService>();
                unityContainer.RegisterType<IUserLocationBrandsService, UserLocationBrandsService>();
                unityContainer.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
                unityContainer.RegisterType<IWeekDayService, WeekDayService>();
                unityContainer.RegisterType<IScheduledJobFrequencyService, ScheduledJobFrequencyService>();
                unityContainer.RegisterType<IScheduledJobMinRatesService, ScheduledJobMinRatesService>();

                unityContainer.RegisterType<IRuleSetService, RuleSetService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetRentalLengthService, RuleSetRentalLengthService>();
                unityContainer.RegisterType<IRuleSetWeekDayService, RuleSetWeekDayService>();

                unityContainer.RegisterType<IRuleSetsAppliedService, RuleSetsAppliedService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetGapSettingService, RuleSetGapSettingService>();
                unityContainer.RegisterType<IRuleSetGroupCompanyService, RuleSetGroupCompanyService>();
                unityContainer.RegisterType<IRuleSetGroupService, RuleSetGroupService>();

                unityContainer.RegisterType<IRangeIntervalsService, RangeIntervalsService>();

                unityContainer.RegisterType<ILocationBrandService, LocationBrandService>();
                unityContainer.RegisterType<IUserRolesService, UserRolesService>();
                unityContainer.RegisterType<IUserService, UserService>();
                unityContainer.RegisterType<IQuickViewService, QuickViewService>();
                unityContainer.RegisterType<ISearchSummaryService, SearchSummaryService>();
                unityContainer.RegisterType<IScheduledJobService, ScheduledJobService>();
                unityContainer.RegisterType<IScheduledJobTetheringsService, ScheduledJobTetheringsService>();
                unityContainer.RegisterType<IGlobalLimitService, GlobalLimitService>();
                unityContainer.RegisterType<IGlobalLimitDetailService, GlobalLimitDetailService>();
                unityContainer.RegisterType<IProvidersService, ProvidersService>();
                unityContainer.RegisterType<ICallbackResponseService, CallbackResponseService>();
                unityContainer.RegisterType<IScrappingServersService, ScrappingServersService>();
                unityContainer.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();

                unityContainer.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();

                var _cacheManager = unityContainer.Resolve<ICacheManager>();
                var quickViewService = unityContainer.Resolve<IQuickViewService>();
                //Remove all cache object
                _cacheManager.RemoveAllCacheObjects();
                string asyncResponse = string.Empty;
                LogHelper.WriteToLogFile("Send Request to run quick views", LogHelper.GetLogFilePath());
                Task.Run(async () => { asyncResponse = await quickViewService.RunQuickViewShop(); }).Wait();

            }
            catch (Exception ex)
            {

                LogHelper.WriteToLogFile("Exception Occured " + ex.InnerException, LogHelper.GetLogFilePath());
            }

        }
    }
}
