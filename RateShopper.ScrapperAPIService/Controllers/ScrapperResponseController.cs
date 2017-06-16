using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using RateShopper.ScrapperAPIService.Models;
using System.Transactions;
using RateShopper.Data;
using Microsoft.Practices.Unity;
using RateShopper.Core.Cache;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;

namespace RateShopper.ScrapperAPIService.Controllers
{
    public class ScrapperResponseController : ApiController
    {

        // GET api/<controller>
        //GET method temp added
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public HttpResponseMessage PostScrapper([FromBody]JObject inputJSON)
        {
            HttpResponseMessage Message = new HttpResponseMessage();

            if (inputJSON != null)
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

                unityContainer.RegisterType<IFTBTargetService, FTBTargetService>();
                unityContainer.RegisterType<IFTBTargetsDetailService, FTBTargetsDetailService>();
                unityContainer.RegisterType<IFTBRatesService, FTBRatesService>();
                unityContainer.RegisterType<IFTBScheduleJobService, FTBScheduleJobService>();
                unityContainer.RegisterType<IFTBRatesSplitMonthsService, FTBRatesSplitMonthsService>();
                unityContainer.RegisterType<ISplitMonthDetailsService, SplitMonthDetailsService>();
                unityContainer.RegisterType<IJobTypeMapperService, JobTypeMapperService>();
                

                unityContainer.RegisterType<ITSDTransactionsService, TSDTransactionService>();
                unityContainer.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
                unityContainer.RegisterType<IQuickViewResultsService, QuickViewResultService>();
                unityContainer.RegisterType<IProvidersService, ProvidersService>();
                unityContainer.RegisterType<IQuickViewService, QuickViewService>();
                unityContainer.RegisterType<ICallbackResponseService, CallbackResponseService>();
                unityContainer.RegisterType<IScrappingServersService, ScrappingServersService>();
                unityContainer.RegisterType<IPostJSONLogService, PostJSONLogService>();
                unityContainer.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();
                unityContainer.RegisterType<IRateCodeService, RateCodeService>();
                unityContainer.RegisterType<IRateCodeDateRangeService, RateCodeDateRangeService>();

                unityContainer.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();

                var _ftbRatesService = unityContainer.Resolve<IFTBRatesService>();

                var _EZRACRateShopperContext = unityContainer.Resolve<IEZRACRateShopperContext>();
                var _cacheManager = unityContainer.Resolve<ICacheManager>();
                var _carClass = unityContainer.Resolve<ICarClassService>();
                var _locations = unityContainer.Resolve<ILocationService>();
                var _rentalLengths = unityContainer.Resolve<IRentalLengthService>();
                var _scrapperSources = unityContainer.Resolve<IScrapperSourceService>();
                var _vendorCodes = unityContainer.Resolve<ICompanyService>();
                var _searchResultRawDataService = unityContainer.Resolve<ISearchResultRawDataService>();
                var _searchSummaryService = unityContainer.Resolve<ISearchSummaryService>();
                var _statusService = unityContainer.Resolve<IStatusesService>();
                var _quickViewService = unityContainer.Resolve<IQuickViewService>();
                var _searchResultService = unityContainer.Resolve<ISearchResultsService>();
                var _ftbscheduledJobService = unityContainer.Resolve<IFTBScheduleJobService>();
                
                var _scheduledJobService = unityContainer.Resolve<IScheduledJobService>();
                var _postJSONLogService = unityContainer.Resolve<IPostJSONLogService>();

                string failedKeyVal = ConfigurationManager.AppSettings["FailedKey"];

                //Create Dictionary to get locations and Carclasses,rentalLengths,scrapperSources,vendorCodes
                Dictionary<string, long> locations = new Dictionary<string, long>();
                Dictionary<string, long> carClasses = new Dictionary<string, long>();
                Dictionary<long, long> rentalLengths = new Dictionary<long, long>();
                Dictionary<string, long> scrapperSources = new Dictionary<string, long>();
                Dictionary<string, long> vendorCodes = new Dictionary<string, long>();

                //Remove all cache object
                _cacheManager.RemoveAllCacheObjects();

                locations = _EZRACRateShopperContext.Locations.Where(loc => !loc.IsDeleted).Select(obj => new { location = obj.Code, ID = obj.ID })
                                                                     .ToDictionary(obj => obj.location, obj => obj.ID);
                carClasses = _EZRACRateShopperContext.CarClasses.Where(cars => !cars.IsDeleted).Select(obj => new { carClass = obj.Code, ID = obj.ID })
                                                                     .ToDictionary(obj => obj.carClass, obj => obj.ID);
                rentalLengths = _EZRACRateShopperContext.RentalLengths.Select(obj => new { MappedId = obj.MappedID, ID = obj.ID })
                                                                     .ToDictionary(obj => obj.MappedId, obj => obj.ID);
                scrapperSources = _EZRACRateShopperContext.ScrapperSources.Select(obj => new { Code = obj.Code, ID = obj.ID })
                                                                     .ToDictionary(obj => obj.Code, obj => obj.ID);
                vendorCodes = _EZRACRateShopperContext.Companies.Where(company => !company.IsDeleted).Select(obj => new { Code = obj.Code, ID = obj.ID })
                                                                     .ToDictionary(obj => obj.Code, obj => obj.ID);

                long scrapperSourceID, carClassId, locationId, vendorId, rentalLengthId;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                dynamic jsonObject = serializer.Deserialize<dynamic>(inputJSON.ToString().Trim());
                var keys = jsonObject.Keys as Dictionary<string, object>.KeyCollection;

                if (keys.FirstOrDefault().ToString().Equals(failedKeyVal, StringComparison.OrdinalIgnoreCase))
                {
                    SearchFail failedSearch = new SearchFail();
                    foreach (var searchresult in jsonObject[failedKeyVal])
                    {
                        failedSearch.SearchSummaryID = searchresult["searchId"];
                        failedSearch.reason = searchresult["reason"];
                    }
                    #region Insert Raw Json Response Data in SearchResultRawData
                    saveRawData(failedSearch.SearchSummaryID, inputJSON.ToString(), _searchResultRawDataService);
                    #endregion
                    //Update SearchSummary Table to reflect status
                    updateSearchStatus(failedSearch.SearchSummaryID, true, failedSearch.reason, _searchSummaryService, _statusService, _searchResultService, _quickViewService, _scheduledJobService);
                }
                else if (keys.FirstOrDefault().ToString().Equals(ConfigurationManager.AppSettings["SuccessKey"], StringComparison.OrdinalIgnoreCase))
                {
                    Dictionary<long, string> lors = new Dictionary<long, string>();
                    //fetch search summaryId 
                    var searchData = jsonObject[ConfigurationManager.AppSettings["SuccessKey"]][0];
                    long scrappedSummaryId = Convert.ToInt64(searchData["SearchId"]);
                    SearchSummary search = null;
                    Formula ezLocationBrandFormula = null;
                    Formula adLocationBrandFormula = null;
                    long ezLbId = 0;
                    long adLbId = 0;
                    string sourceCode = Convert.ToString(searchData["DataSource"]);
                    bool isTravelocity = false;
                    bool isDiscountedRatesSource = false;//10% discount is given on onetravel and cheapoair
                    long shopLocationBrand = 0;
                    string[] discountedSources = ConfigurationManager.AppSettings["DiscountedSources"].Split(',');

                    //check if scrapper source is Travelocity,Car Rentals,Orbitz or CheapTickets then calculate weekly LORs base rate from total rate.
                    if (sourceCode.Equals(ConfigurationManager.AppSettings["Travelocity"], StringComparison.OrdinalIgnoreCase) || sourceCode.Equals(ConfigurationManager.AppSettings["CarRentals"], StringComparison.OrdinalIgnoreCase) ||
                        sourceCode.Equals(ConfigurationManager.AppSettings["CheapTickets"], StringComparison.OrdinalIgnoreCase) || sourceCode.Equals(ConfigurationManager.AppSettings["Orbitz"], StringComparison.OrdinalIgnoreCase) ||
                        sourceCode.Equals(ConfigurationManager.AppSettings["OneTravel"], StringComparison.OrdinalIgnoreCase) || sourceCode.Equals(ConfigurationManager.AppSettings["Cheapoair"], StringComparison.OrdinalIgnoreCase) ||
                        sourceCode.Equals(ConfigurationManager.AppSettings["Southwest"], StringComparison.OrdinalIgnoreCase))
                    {
                        isTravelocity = true;
                        if (discountedSources.Any(d => d.ToUpper() == sourceCode.ToUpper()))
                        {
                            isDiscountedRatesSource = true;
                        }
                        search = _EZRACRateShopperContext.SearchSummaries.Where(shop => shop.ID == scrappedSummaryId).FirstOrDefault();
                        if (search != null && !string.IsNullOrEmpty(search.LocationBrandIDs))
                        {                            
                            shopLocationBrand = Convert.ToInt64(search.LocationBrandIDs);
                            LocationBrand brand = _EZRACRateShopperContext.LocationBrands.Where(locationbrand => locationbrand.ID == shopLocationBrand).FirstOrDefault();
                            string[] locationBrandAlias = Convert.ToString(brand.LocationBrandAlias).Split('-');
                            LocationBrand adLocationBrand = new LocationBrand();
                            LocationBrand ezLocationBrand = new LocationBrand();
                            if (locationBrandAlias.Length > 0)
                            {
                                if (locationBrandAlias[1].Equals("EZ", StringComparison.OrdinalIgnoreCase))
                                {
                                    ezLbId = shopLocationBrand;
                                    ezLocationBrandFormula = _EZRACRateShopperContext.Formulas.Where(formula => formula.LocationBrandID == ezLbId).FirstOrDefault();

                                    adLocationBrand = _EZRACRateShopperContext.LocationBrands.Where(locationbrand => !locationbrand.IsDeleted &&
                                         locationbrand.LocationID == brand.LocationID && locationbrand.ID != ezLbId).FirstOrDefault();

                                    if (adLocationBrand != null)
                                    {
                                        adLbId = adLocationBrand.ID;
                                        adLocationBrandFormula = _EZRACRateShopperContext.Formulas.Where(formula => formula.LocationBrandID == adLbId).FirstOrDefault();
                                    }
                                }
                                else if (locationBrandAlias[1].Equals("AD", StringComparison.OrdinalIgnoreCase))
                                {
                                    adLbId = shopLocationBrand;
                                    adLocationBrandFormula = _EZRACRateShopperContext.Formulas.Where(formula => formula.LocationBrandID == adLbId).FirstOrDefault();

                                    ezLocationBrand = _EZRACRateShopperContext.LocationBrands.Where(locationbrand => !locationbrand.IsDeleted &&
                                          locationbrand.LocationID == brand.LocationID && locationbrand.ID != adLbId).FirstOrDefault();

                                    if (ezLocationBrand != null)
                                    {
                                        ezLbId = ezLocationBrand.ID;
                                        ezLocationBrandFormula = _EZRACRateShopperContext.Formulas.Where(formula => formula.LocationBrandID == ezLbId).FirstOrDefault();
                                    }
                                }
                            }

                        }
                        lors = _EZRACRateShopperContext.RentalLengths.Select(obj => new { ID = obj.ID, Code = obj.Code })
                                                                     .ToDictionary(obj => obj.ID, obj => obj.Code);
                    }                    

                    DateTimeOffset dtoArrivalDate;
                    DateTimeOffset dtoReturnDate;
                    List<MapTableResult> searchSuccess = new List<MapTableResult>();
                    decimal discountedValue = Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountedValue"]);
                    DateTime southwestDiscountStartDate = DateTime.ParseExact(ConfigurationManager.AppSettings["SouthwestDiscountStartDate"], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime southwestDiscountEndDate = DateTime.ParseExact(ConfigurationManager.AppSettings["SouthwestDiscountEndDate"], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    foreach (var searchresult in jsonObject[ConfigurationManager.AppSettings["SuccessKey"]])
                    {
                        MapTableResult result = new MapTableResult();
                        result.SearchSummaryID = Convert.ToInt64(searchresult["SearchId"]);
                        result.ScrapperSourceID = scrapperSources.TryGetValue(searchresult["DataSource"], out scrapperSourceID) ? scrapperSources[searchresult["DataSource"]] : 0;
                        result.LocationID = locations.TryGetValue(searchresult["CityCd"], out locationId) ? locations[searchresult["CityCd"]] : 0;
                        result.RentalLengthID = rentalLengths.TryGetValue(Convert.ToInt64(searchresult["Lor"]), out rentalLengthId) ? rentalLengths[Convert.ToInt64(searchresult["Lor"])] : 0;
                        result.CarClassID = carClasses.TryGetValue(searchresult["CarTypeCd"], out carClassId) ? carClasses[searchresult["CarTypeCd"]] : 0;
                        result.CompanyID = vendorCodes.TryGetValue(searchresult["VendCd"], out vendorId) ? vendorCodes[searchresult["VendCd"]] : 0;
                        //Travelocity source scraps weekly rate in Daily format -> convert Total rate to Base Rate for weekly lors
                        string vendorCode = Convert.ToString(searchresult["VendCd"]);
                        string rentalLength = string.Empty;
                        result.TotalRate = Convert.ToDecimal(searchresult["EstRentalChrgAmt"]);

                        string lorCode = lors.TryGetValue(Convert.ToInt64(searchresult["Lor"]), out rentalLength) ? lors[Convert.ToInt64(searchresult["Lor"])] : string.Empty;
                        if (isTravelocity && !isDiscountedRatesSource && lorCode.ToUpper().IndexOf('W') > -1 &&
                            (vendorCode.Equals(ConfigurationManager.AppSettings["EZBrand"], StringComparison.OrdinalIgnoreCase) ||
                                    vendorCode.Equals(ConfigurationManager.AppSettings["ADBrand"], StringComparison.OrdinalIgnoreCase)))
                        {
                            if (vendorCode.Equals(ConfigurationManager.AppSettings["EZBrand"], StringComparison.OrdinalIgnoreCase) && ezLocationBrandFormula != null)
                            {
                                result.BaseRate = _searchResultService.CalculateSuggestedBaseRate(ezLbId, ezLocationBrandFormula, Convert.ToDecimal(searchresult["EstRentalChrgAmt"]), lorCode, lorCode.Substring(0, 1));                                
                            }
                            else if (vendorCode.Equals(ConfigurationManager.AppSettings["ADBrand"], StringComparison.OrdinalIgnoreCase) && adLocationBrandFormula != null)
                            {
                                result.BaseRate = _searchResultService.CalculateSuggestedBaseRate(adLbId, adLocationBrandFormula, Convert.ToDecimal(searchresult["EstRentalChrgAmt"]), lorCode, lorCode.Substring(0, 1));                                                                
                            }
                            else
                            {
                                result.BaseRate = Convert.ToDecimal(searchresult["RtAmt"]);
                            }
                        }
                        else if (isDiscountedRatesSource && (vendorCode.Equals(ConfigurationManager.AppSettings["EZBrand"], StringComparison.OrdinalIgnoreCase) || vendorCode.Equals(ConfigurationManager.AppSettings["ADBrand"], StringComparison.OrdinalIgnoreCase)))
                        {
                            DateTimeOffset arrivalOffset = DateTimeOffset.Parse(searchresult["ArvDt"].ToString());
                            
                            if (sourceCode.Equals(ConfigurationManager.AppSettings["Southwest"], StringComparison.OrdinalIgnoreCase) &&
                                !(southwestDiscountStartDate <= arrivalOffset.UtcDateTime.Date && arrivalOffset.UtcDateTime.Date <= southwestDiscountEndDate))
                            {
                                result.BaseRate = Convert.ToDecimal(searchresult["RtAmt"]);
                            }
                            else
                            {
                                decimal numberOfDays = Convert.ToDecimal(lorCode.Substring(1));
                                decimal dailyRate = Convert.ToDecimal(searchresult["RtAmt"]) * discountedValue;                               
                                if (sourceCode.Equals(ConfigurationManager.AppSettings["OneTravel"], StringComparison.OrdinalIgnoreCase) || sourceCode.Equals(ConfigurationManager.AppSettings["Cheapoair"], StringComparison.OrdinalIgnoreCase))
                                {
                                    if (lorCode.ToUpper().IndexOf('D') > -1)
                                    {
                                        result.BaseRate = Math.Round(dailyRate);
                                    }
                                    else if (lorCode.ToUpper().IndexOf('W') > -1)
                                    {
                                        //show base rate maximum of 7 days rental i.e. per week for weekly rates
                                        numberOfDays = numberOfDays <= 7 ? numberOfDays : 7;
                                        result.BaseRate = Math.Round(dailyRate * numberOfDays);
                                    }
                                    else if (lorCode.ToUpper().IndexOf('M') > -1)
                                    {
                                        result.BaseRate = Math.Round(dailyRate * numberOfDays);
                                    }
                                }
                                else
                                {
                                    result.BaseRate = Math.Round(dailyRate, 2);
                                }

                                if (vendorCode.Equals(ConfigurationManager.AppSettings["EZBrand"], StringComparison.OrdinalIgnoreCase) && ezLocationBrandFormula != null)
                                {
                                    if (result.BaseRate.HasValue)
                                    {
                                        result.TotalRate = _searchResultService.CalculateSuggestedTotalRate(ezLbId, ezLocationBrandFormula, result.BaseRate.Value, lorCode, lorCode.Substring(0, 1));
                                        result.BaseRate = Math.Round(dailyRate);
                                    }
                                }
                                else if (vendorCode.Equals(ConfigurationManager.AppSettings["ADBrand"], StringComparison.OrdinalIgnoreCase) && adLocationBrandFormula != null)
                                {
                                    if (result.BaseRate.HasValue)
                                    {
                                        result.TotalRate = _searchResultService.CalculateSuggestedTotalRate(adLbId, adLocationBrandFormula, result.BaseRate.Value, lorCode, lorCode.Substring(0, 1));
                                        result.BaseRate = Math.Round(dailyRate);
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.BaseRate = Convert.ToDecimal(searchresult["RtAmt"]);
                        }

                        
                        result.UpdatedDateTime = DateTime.Now;
                        dtoArrivalDate = DateTimeOffset.Parse(searchresult["ArvDt"].ToString());
                        dtoReturnDate = DateTimeOffset.Parse(searchresult["RtrnDt"].ToString());
                        result.ArvDt = dtoArrivalDate.UtcDateTime.Date;
                        result.RtrnDt = dtoArrivalDate.UtcDateTime.Date;
                        searchSuccess.Add(result);
                    }

                    EZRACRateShopperContext context = new EZRACRateShopperContext();

                    //        //Convert list to dataTable for bulk copy 
                    DataTable results = RateShopper.ScrapperAPIService.Helper.ListToDataTable.ConvertToDataTable(searchSuccess);
                    if (context != null)
                    {
                        using (var tx = new TransactionScope())
                        {
                            using (var bcp = new SqlBulkCopy(context.Database.Connection.ConnectionString))
                            {
                                bcp.BatchSize = searchSuccess.ToList().Count;
                                bcp.DestinationTableName = ConfigurationManager.AppSettings["TableName"].Trim();
                                bcp.WriteToServer(results);
                                tx.Complete();
                            }
                        }
                    }
                    long searchSummaryId = searchSuccess.Select(obj => obj.SearchSummaryID).FirstOrDefault();
                    //Update SearchSummary Table to reflect status
                    updateSearchStatus(searchSummaryId, false, string.Empty, _searchSummaryService, _statusService, _searchResultService, _quickViewService, _scheduledJobService);

                    #region Insert Raw Json Response Data in SearchResultRawData
                    saveRawData(searchSummaryId, inputJSON.ToString(), _searchResultRawDataService);
                    #endregion
                }

                //send response to caller 
                Message.StatusCode = HttpStatusCode.OK;
                return Request.CreateResponse(HttpStatusCode.OK, "You Have Posted Value  " + Convert.ToString(inputJSON));
            }
            Message.StatusCode = HttpStatusCode.NotAcceptable;
            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "You Have Posted Value  " + Convert.ToString(inputJSON));
        }

        private decimal CalculateRentalBaseRate()
        {
            return 0;
        }       

        public void saveRawData(long searchSummaryId, string response, ISearchResultRawDataService _searchResultRawDataService)
        {
            try
            {

                SearchResultRawData rawData = new SearchResultRawData
                {
                    SearchSummaryID = searchSummaryId,
                    JSON = response,
                    CreatedDateTime = DateTime.Now
                };
                _searchResultRawDataService.Add(rawData);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void updateSearchStatus(long searchSummaryId, bool isSearchFailed, string reason, ISearchSummaryService _searchSummaryService, IStatusesService _statusService, ISearchResultsService _searchResultService, IQuickViewService _quickViewService, IScheduledJobService _scheduledJobService)
        {

            SearchSummary searchSummary = _searchSummaryService.GetById(searchSummaryId, false);
            if (searchSummary != null && searchSummary.StatusID != _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["DeletedRequest"])))
            {
                if (isSearchFailed)
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["FailedRequest"]));
                    searchSummary.UpdatedDateTime = DateTime.Now;
                    searchSummary.Response = reason;
                    if (searchSummary.ScheduledJobID.HasValue && searchSummary.ScheduledJobID.Value > 0)
                    {
                        searchSummary.IsReviewed = false;
                    }
                    _searchSummaryService.Update(searchSummary);
                    //if it is quick view update quick View status
                    if (searchSummary.IsQuickView.HasValue && searchSummary.IsQuickView.Value)
                    {
                        QuickView quickViewRow = _quickViewService.GetAll(false).Where(quick => quick.ChildSearchSummaryId == searchSummary.ID).FirstOrDefault();
                        if (quickViewRow != null)
                        {
                            quickViewRow.IsExecutionInProgress = false;
                            quickViewRow.StatusId = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["FailedRequest"]));
                            _quickViewService.Update(quickViewRow);
                        }
                    }
                    //if it is scheduled job update scheduled job status
                    if (searchSummary.ScheduledJobID.HasValue && searchSummary.ScheduledJobID.Value > 0)
                    {
                        ScheduledJob scheduledJob = _scheduledJobService.GetAll(false).Where(job => job.ID == searchSummary.ScheduledJobID.Value).SingleOrDefault();
                        if (scheduledJob != null)
                        {
                            scheduledJob.ExecutionInProgress = false;
                            _scheduledJobService.Update(scheduledJob);
                        }
                    }
                }
                else
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["DataReceived"]));
                    searchSummary.UpdatedDateTime = DateTime.Now;
                    _searchSummaryService.Update(searchSummary);

                    //call data processing service to generate search result JSON.
                    Task.Run(() => _searchResultService.GenerateSeachResultProcesssedData(searchSummary.ID));
                }
            }
        }


    }

    public class ElmahErrorAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {

        public override void OnException(
            System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {

            if (actionExecutedContext.Exception != null)
                Elmah.ErrorSignal.FromCurrentContext().Raise(actionExecutedContext.Exception);

            base.OnException(actionExecutedContext);
        }
    }

}