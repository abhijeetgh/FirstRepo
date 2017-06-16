using System;
using System.IO;
using System.Configuration;
using RateShopper.Services.Data;
using Microsoft.Practices.Unity;
using RateShopper.Data;
using RateShopper.Core.Cache;
using System.Threading.Tasks;
using RateShopper.Services.Helper;

namespace RateShopper.FTB.Scheduler
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
                unityContainer.RegisterType<ICarClassService, CarClassService>();
                unityContainer.RegisterType<IRentalLengthService, RentalLengthService>();
                unityContainer.RegisterType<ILocationService, LocationsService>();
                unityContainer.RegisterType<IGlobalLimitService, GlobalLimitService>();
                unityContainer.RegisterType<IGlobalLimitDetailService, GlobalLimitDetailService>();
                unityContainer.RegisterType<IWeekDayService, WeekDayService>();
                unityContainer.RegisterType<IRangeIntervalsService, RangeIntervalsService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetRentalLengthService, RuleSetRentalLengthService>();
                unityContainer.RegisterType<IRuleSetWeekDayService, RuleSetWeekDayService>();
                unityContainer.RegisterType<IRuleSetGapSettingService, RuleSetGapSettingService>();
                unityContainer.RegisterType<IRuleSetService, RuleSetService>();
                unityContainer.RegisterType<ILocationBrandService, LocationBrandService>();
                unityContainer.RegisterType<IFTBScheduleJobService, FTBScheduleJobService>();
                unityContainer.RegisterType<IFormulaService, FormulaService>();
                unityContainer.RegisterType<IJobTypeMapperService, JobTypeMapperService>();
                unityContainer.RegisterType<IUserRolesService, UserRolesService>();
                unityContainer.RegisterType<IUserService, UserService>();
                unityContainer.RegisterType<IUserScrapperSourcesService, UserScrapperSourcesService>();
                unityContainer.RegisterType<IFTBTargetService, FTBTargetService>();
                unityContainer.RegisterType<IFTBRatesService, FTBRatesService>();
                unityContainer.RegisterType<IScheduledJobFrequencyService, ScheduledJobFrequencyService>();
                unityContainer.RegisterType<IFTBRatesDetailService, FTBRateDetailsService>();
                unityContainer.RegisterType<ISplitMonthDetailsService, SplitMonthDetailsService>();
                unityContainer.RegisterType<IUserLocationBrandsService, UserLocationBrandsService>();
                unityContainer.RegisterType<IStatusesService, StatusesService>();
                unityContainer.RegisterType<ICompanyService, CompanyService>();
                unityContainer.RegisterType<IScrapperSourceService, ScrapperSourceService>();
                unityContainer.RegisterType<IGlobalTetherSettingService, GlobalTetherSettingService>();
                unityContainer.RegisterType<ICallbackResponseService, CallbackResponseService>();
                unityContainer.RegisterType<IFTBTargetsDetailService, FTBTargetsDetailService>();
                unityContainer.RegisterType<IProvidersService, ProvidersService>();
                unityContainer.RegisterType<ISearchSummaryService, SearchSummaryService>();
                unityContainer.RegisterType<ITSDTransactionsService, TSDTransactionService>();
                unityContainer.RegisterType<IScrappingServersService, ScrappingServersService>();
                unityContainer.RegisterType<IFTBRatesSplitMonthsService, FTBRatesSplitMonthsService>();
                unityContainer.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();
                unityContainer.RegisterType<IRateCodeService, RateCodeService>();
                unityContainer.RegisterType<IRateCodeDateRangeService, RateCodeDateRangeService>();

                unityContainer.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();

                var _cacheManager = unityContainer.Resolve<ICacheManager>();
                var ftbScheduledJobService = unityContainer.Resolve<IFTBScheduleJobService>();

                Task.Run(async () => await ftbScheduledJobService.RunFTBAutomationShops()).Wait();
                //Console.Read();
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Exception Occured " + ex.InnerException, LogHelper.GetLogFilePath());
            }
        }
    }
}

