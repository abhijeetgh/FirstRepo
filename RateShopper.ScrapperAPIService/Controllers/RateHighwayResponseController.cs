using Microsoft.Practices.Unity;
using Newtonsoft.Json.Linq;
using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Providers.Interface;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RateShopper.ScrapperAPIService.Controllers
{
    public class RateHighwayResponseController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
                
        [HttpPost]
        public HttpResponseMessage Callback([FromBody]JObject response)
        {
            HttpResponseMessage Message = new HttpResponseMessage();

            if (response != null)
            {
                IUnityContainer unityContainer = new UnityContainer();
                unityContainer.RegisterType<IUserService, UserService>();
                unityContainer.RegisterType<ICarClassService, CarClassService>();
                unityContainer.RegisterType<ICacheManager, InMemoryCacheManager>();
                unityContainer.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>();

                unityContainer.RegisterType<IGlobalTetherSettingService, GlobalTetherSettingService>();
                unityContainer.RegisterType<IFormulaService, FormulaService>();
                unityContainer.RegisterType<IStatusesService, StatusesService>();
                unityContainer.RegisterType<ISearchResultsService, SearchResultsService>();
                unityContainer.RegisterType<ISearchResultRawDataService, SearchResultRawDataService>();
                unityContainer.RegisterType<ISearchResultProcessedDataService, SearchResultProcessedDataService>();
                unityContainer.RegisterType<ISearchSummaryService, SearchSummaryService>();
                unityContainer.RegisterType<ILocationService, LocationsService>();
                unityContainer.RegisterType<ICompanyService, CompanyService>();
                unityContainer.RegisterType<ILocationBrandService, LocationBrandService>();
                unityContainer.RegisterType<ILocationCompanyService, LocationCompanyService>();
                unityContainer.RegisterType<IUserRolesService, UserRolesService>();
                unityContainer.RegisterType<IScrapperSourceService, ScrapperSourceService>();
                unityContainer.RegisterType<IRentalLengthService, RentalLengthService>();

                unityContainer.RegisterType<IRuleSetService, RuleSetService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetGroupCompanyService, RuleSetGroupCompanyService>();
                unityContainer.RegisterType<IRuleSetGroupService, RuleSetGroupService>();
                unityContainer.RegisterType<IRuleSetGapSettingService, RuleSetGapSettingService>();
                unityContainer.RegisterType<IRuleSetRentalLengthService, RuleSetRentalLengthService>();
                unityContainer.RegisterType<IRuleSetsAppliedService, RuleSetsAppliedService>();
                unityContainer.RegisterType<IRuleSetWeekDayService, RuleSetWeekDayService>();
                unityContainer.RegisterType<IRuleSetDefaultSettingService, RuleSetDefaultSettingService>();
                unityContainer.RegisterType<IWeekDayService, WeekDayService>();
                unityContainer.RegisterType<IUserScrapperSourcesService, UserScrapperSourcesService>();
                unityContainer.RegisterType<IRangeIntervalsService, RangeIntervalsService>();
                unityContainer.RegisterType<IUserLocationBrandsService, UserLocationBrandsService>();
                unityContainer.RegisterType<IGlobalLimitService, GlobalLimitService>();
                unityContainer.RegisterType<IGlobalLimitDetailService, GlobalLimitDetailService>();
                unityContainer.RegisterType<IScheduledJobService, ScheduledJobService>();
                unityContainer.RegisterType<IScheduledJobFrequencyService, ScheduledJobFrequencyService>();
                unityContainer.RegisterType<IScheduledJobMinRatesService, ScheduledJobMinRatesService>();
                unityContainer.RegisterType<IScheduledJobTetheringsService, ScheduledJobTetheringsService>();
                unityContainer.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
                unityContainer.RegisterType<IFTBScheduleJobService, FTBScheduleJobService>();
                unityContainer.RegisterType<IFTBTargetService, FTBTargetService>();
                unityContainer.RegisterType<IFTBTargetsDetailService, FTBTargetsDetailService>();
                unityContainer.RegisterType<IFTBRatesService, FTBRatesService>();
                unityContainer.RegisterType<IFTBRatesSplitMonthsService, FTBRatesSplitMonthsService>();
                unityContainer.RegisterType<ISplitMonthDetailsService, SplitMonthDetailsService>();
                unityContainer.RegisterType<IJobTypeMapperService, JobTypeMapperService>();

                unityContainer.RegisterType<ITSDTransactionsService, TSDTransactionService>();
                unityContainer.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
                unityContainer.RegisterType<IQuickViewResultsService, QuickViewResultService>();
                unityContainer.RegisterType<IProvidersService, ProvidersService>();
                unityContainer.RegisterType<IQuickViewService, QuickViewService>();
                unityContainer.RegisterType<IProviderService, Providers.ProviderService>();
                unityContainer.RegisterType<IRequestProcessor, Providers.Helper.RequestProcessor>();
                unityContainer.RegisterType<IRateHighway, Providers.Providers.RateHighway>();
                unityContainer.RegisterType<IScrappingService, Providers.Providers.ScrappingService>();
                unityContainer.RegisterType<ICallbackResponseService, CallbackResponseService>();
                unityContainer.RegisterType<IScrappingServersService, ScrappingServersService>();
                unityContainer.RegisterType<IPostJSONLogService, PostJSONLogService>();
                unityContainer.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();
                unityContainer.RegisterType<IRateCodeService, RateCodeService>();
                unityContainer.RegisterType<IRateCodeDateRangeService, RateCodeDateRangeService>();

                unityContainer.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();

                var _ftbRatesService = unityContainer.Resolve<IFTBRatesService>();
                var _ftbscheduledJobService = unityContainer.Resolve<IFTBScheduleJobService>();
                var _providerService = unityContainer.Resolve<IProviderService>();
                _providerService.ParseResponse("RH", Convert.ToString(response), 0);

                Message.StatusCode = HttpStatusCode.OK;
                return Request.CreateResponse(HttpStatusCode.OK, "You Have Successfully Posted Value");
            }
            Message.StatusCode = HttpStatusCode.NotAcceptable;
            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "You Have Successfully Posted Value");
            
        }
    }
}