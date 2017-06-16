using System;
using System.Collections.Generic;
using System.Linq;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
using RateShopper.Domain.DTOs;
using RateShopper.Services.Helper;
using System.Globalization;
using System.Configuration;
using System.Web;
namespace RateShopper.Services.Data
{
    public class ScheduledJobService : BaseService<ScheduledJob>, IScheduledJobService
    {
        private ISearchSummaryService _searchSummaryService;
        private ICarClassService _carClassService;
        private ISearchResultSuggestedRatesService _searchResultSuggestedRatesService;
        private IScrapperSourceService _scrapperService;
        private ILocationBrandService _locationBrandService;
        private ILocationService _locationService;
        private IRentalLengthService _rentalLengthService;
        private IWeekDayService _weekdayService;
        private IScheduledJobFrequencyService _scheduledJobFrequencyService;
        ICompanyService _companyService;
        private IScheduledJobMinRatesService _scheduledJobMinRatesService;
        private IScheduledJobTetheringsService _scheduledJobTetheringsService;
        private IRuleSetService _ruleSetService;
        private IProvidersService _providerService;
        private IScrappingServersService _scrappingServersService;
        private IUserService _userService;
        private IFTBScheduleJobService _ftbScheduleJobService;
        private IStatusesService _statusService;
        private ISearchResultProcessedDataService _searchResultsProcessedService;
        private IScheduledJobOpaqueValuesService _scheduledJobOpaqueValuesService;

        public ScheduledJobService(IEZRACRateShopperContext context, ICacheManager cacheManager, ISearchSummaryService searchSummaryService
            , ISearchResultSuggestedRatesService searchResultSuggestedRatesService, ICarClassService carClassService, IScrapperSourceService scrapperService,
            ILocationBrandService locationBrandService, ICompanyService companyService, IRentalLengthService rentalLengthService, IWeekDayService weekdayService,
            IScheduledJobFrequencyService scheduledJobFrequencyService, IScheduledJobMinRatesService scheduledJobMinRatesService, ILocationService locationService,
            IScheduledJobTetheringsService scheduledJobTetheringsService, IRuleSetService ruleSetService, IProvidersService providerService, IScrappingServersService scrappingServersService, IUserService userService,
            IFTBScheduleJobService ftbScheduleJobService, IStatusesService statusService, ISearchResultProcessedDataService searchResultsProcessedService, IScheduledJobOpaqueValuesService scheduledJobOpaqueValuesService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScheduledJob>();
            _cacheManager = cacheManager;
            _searchSummaryService = searchSummaryService;
            _searchResultSuggestedRatesService = searchResultSuggestedRatesService;
            _carClassService = carClassService;
            _rentalLengthService = rentalLengthService;
            _weekdayService = weekdayService;
            _scrapperService = scrapperService;
            _locationBrandService = locationBrandService;
            _companyService = companyService;
            _rentalLengthService = rentalLengthService;
            _scheduledJobFrequencyService = scheduledJobFrequencyService;
            _scheduledJobMinRatesService = scheduledJobMinRatesService;
            _locationService = locationService;
            _scheduledJobTetheringsService = scheduledJobTetheringsService;
            _ruleSetService = ruleSetService;
            _providerService = providerService;
            _scrappingServersService = scrappingServersService;
            _ftbScheduleJobService = ftbScheduleJobService;
            _userService = userService;
            _statusService = statusService;
            _searchResultsProcessedService = searchResultsProcessedService;
            _scheduledJobOpaqueValuesService = scheduledJobOpaqueValuesService;
        }

        public FirstTimeResult GetScheduledRuns(long scheduledJobId, bool firstPage, long searchSummaryId)
        {
            List<JobHistoryDTO> jobLog = new List<JobHistoryDTO>();
            //SearchSuggestedRateMaster suggestedRateMaster = new SearchSuggestedRateMaster();
            SearchResultDTO searchResults = new SearchResultDTO();
            FirstTimeResult resultsBundled = new FirstTimeResult();
            JobHistoryDTO firstRecord = new JobHistoryDTO();
            //filters data
            MarkResultFilters allFilters = new MarkResultFilters();
            List<ResultDays> days = new List<ResultDays>();
            List<RentalLengthsFilter> rentalLengthFilter = new List<RentalLengthsFilter>();
            List<LocationFilter> locationFilter = new List<LocationFilter>();
            List<ScrapperSourceFilter> sourcesFilter = new List<ScrapperSourceFilter>();
            List<CarClassFilter> carClassFilter = new List<CarClassFilter>();
            int noOfDaysOldData = Convert.ToInt32(ConfigurationManager.AppSettings["FilterDays"]);
            DateTime daysHistory = DateTime.Now.AddDays(noOfDaysOldData).Date;
            long[] rentalLengthIDs, locationBrandIds, scrapperSourceIds, carClassIds;
            long searchDeletedStatus = _statusService.GetStatusIDByName("Deleted");

            if (firstPage)
            {
                if (scheduledJobId > 0)
                {

                    jobLog = _searchSummaryService.GetAll().Where(summary => summary.ScheduledJobID == scheduledJobId && summary.StatusID != searchDeletedStatus && string.IsNullOrEmpty(summary.ShopType) &&
                        summary.CreatedDateTime.Date >= daysHistory)
                         .Select(summary => new JobHistoryDTO
                         {
                             CreatedDateTime = summary.CreatedDateTime,
                             SearchSummaryID = summary.ID,
                             RentalLengthIds = summary.RentalLengthIDs,
                             ScrapperSourceIds = summary.ScrapperSourceIDs,
                             LocationBrandIds = summary.LocationBrandIDs,
                             StartDate = summary.StartDate,
                             EndDate = summary.EndDate,
                             RunDate = summary.CreatedDateTime.ToString("MM/dd/yyyy"),
                             RunTime = summary.CreatedDateTime.ToString("hh:mm tt"),
                             IsReviewed = summary.IsReviewed.HasValue ? summary.IsReviewed.Value : false,
                             StatusId = summary.StatusID,
                             Response = summary.Response,
                             CarClassIds = summary.CarClassesIDs,
                             IsGOVShop = (summary.IsGov.HasValue ? summary.IsGov.Value : false),
                         }).OrderByDescending(log => log.CreatedDateTime).ToList<JobHistoryDTO>();
                }
            }
            else
            {
                if (searchSummaryId > 0)
                {
                    jobLog = _searchSummaryService.GetAll().Where(summary => summary.ID == searchSummaryId && summary.StatusID != searchDeletedStatus && string.IsNullOrEmpty(summary.ShopType))
                         .Select(summary => new JobHistoryDTO
                         {
                             CreatedDateTime = summary.CreatedDateTime,
                             SearchSummaryID = summary.ID,
                             RentalLengthIds = summary.RentalLengthIDs,
                             ScrapperSourceIds = summary.ScrapperSourceIDs,
                             LocationBrandIds = summary.LocationBrandIDs,
                             StartDate = summary.StartDate,
                             EndDate = summary.EndDate,
                             RunDate = summary.CreatedDateTime.ToString("MM/dd/yyyy"),
                             RunTime = summary.CreatedDateTime.ToString("hh:mm tt"),
                             IsReviewed = summary.IsReviewed.HasValue ? summary.IsReviewed.Value : false,
                             StatusId = summary.StatusID,
                             Response = summary.Response,
                             CarClassIds = summary.CarClassesIDs,
                             IsGOVShop = summary.IsGov.HasValue ? summary.IsGov.Value : false
                         }).OrderByDescending(log => log.CreatedDateTime).ToList<JobHistoryDTO>();
                }
            }
            if (jobLog.Count > 0)
            {
                firstRecord = jobLog.FirstOrDefault();
                if (firstRecord != null)
                {
                    firstRecord.BrandId = _locationBrandService.GetById(Convert.ToInt64(firstRecord.LocationBrandIds), false).BrandID;
                    firstRecord.CompanyLogo = _companyService.GetAll().Where(company => company.ID == firstRecord.BrandId).FirstOrDefault().Logo;

                    rentalLengthIDs = firstRecord.RentalLengthIds.Split(',').Select(a => Convert.ToInt64(a)).OrderBy(a => a).ToArray();

                    locationBrandIds = firstRecord.LocationBrandIds.Split(',').Select(a => Convert.ToInt64(a)).OrderBy(a => a).ToArray();

                    scrapperSourceIds = firstRecord.ScrapperSourceIds.Split(',').Select(a => Convert.ToInt64(a)).OrderBy(a => a).ToArray();

                    rentalLengthFilter = _rentalLengthService.GetAll().Where(length => rentalLengthIDs.Any(lengthsId => lengthsId.Equals(length.MappedID)))
                        .Select(length => new RentalLengthsFilter { RentalLength = length.Code, RentalLengthId = length.MappedID }).ToList<RentalLengthsFilter>();

                    locationFilter = _locationBrandService.GetAll().Where(location => locationBrandIds.Any(locationId => locationId.Equals(location.ID)))
                       .Select(location => new LocationFilter { LocationId = location.LocationID, Code = location.LocationBrandAlias, BrandId = location.BrandID }).ToList<LocationFilter>();

                    sourcesFilter = _scrapperService.GetAll().Where(source => scrapperSourceIds.Any(sourceID => sourceID.Equals(source.ID)))
                        .Select(source => new ScrapperSourceFilter { ScrapperSourceId = source.ID, Source = source.Name }).ToList<ScrapperSourceFilter>();

                    carClassIds = firstRecord.CarClassIds.Split(',').Select(car => Convert.ToInt64(car)).OrderBy(car => car).ToArray();

                    carClassFilter = _carClassService.GetAll().Where(car => carClassIds.Any(carId => carId.Equals(car.ID))).OrderBy(car=>car.DisplayOrder).Select(car => new CarClassFilter { ID = car.ID, Code = car.Code }).ToList();

                    DateTime StartDate = firstRecord.StartDate;
                    DateTime EndDate = firstRecord.EndDate;
                    while (StartDate <= EndDate)
                    {
                        days.Add(new ResultDays { ArrivalDate = StartDate, StartDate = StartDate.ToString("MM/dd/yyyy"), StartDateVal = StartDate.ToString("MMddyyyy") });
                        StartDate = StartDate.AddDays(1);
                    }
                    days = days.OrderBy(day => day.ArrivalDate).ToList();

                    //suggestedRateMaster = FilterSuggestedRates(firstRecord.SearchSummaryID, locationFilter.FirstOrDefault().LocationId, firstRecord.BrandId, rentalLengthIDs.FirstOrDefault(), days.FirstOrDefault().StartDateVal);

                    searchResults = GetSearchGridDailyView(firstRecord.SearchSummaryID, locationFilter.FirstOrDefault().LocationId, firstRecord.BrandId, rentalLengthIDs.FirstOrDefault(),
                                    days.FirstOrDefault().StartDateVal);
                }
            }
            else
            {
                jobLog = null;
            }
            resultsBundled.jobs = jobLog;
            //resultsBundled.suggestedRateMaster = suggestedRateMaster;
            resultsBundled.searchResults = searchResults;
            resultsBundled.resultFilters = firstRecord;
            allFilters.daysFilter = days;
            allFilters.lorFilters = rentalLengthFilter;
            allFilters.scrapperSources = sourcesFilter;
            allFilters.locations = locationFilter;
            allFilters.carClassFilters = carClassFilter;
            resultsBundled.allFilters = allFilters;

            return resultsBundled;
        }

        public SearchResultDTO FilterSuggestedRates(long searchSummaryId, long locationId, long brandId, long rentalLengthId, string selectedDate, long carClassId, bool isDailyView)
        {
            SearchResultDTO searchResults = null;

            if (searchSummaryId > 0 && locationId > 0 && rentalLengthId > 0)
            {

                if (isDailyView)
                {
                    searchResults = GetSearchGridDailyView(searchSummaryId, locationId, brandId, rentalLengthId, selectedDate);
                }
                else
                {
                    searchResults = GetSearchGridClassicViewData(searchSummaryId, locationId, brandId, rentalLengthId, carClassId);
                }
            }
            return searchResults;
        }

        public bool MarkResultsReviewed(long searchSummaryId, long UserId)
        {
            bool status = false;
            if (searchSummaryId > 0)
            {
                SearchSummary summary = _searchSummaryService.GetAll().Where(summaryObj => summaryObj.ID == searchSummaryId).FirstOrDefault();
                if (summary != null)
                {
                    summary.IsReviewed = true;
                    summary.UpdatedBy = UserId;
                    summary.UpdatedDateTime = DateTime.Now;
                }
                _searchSummaryService.Update(summary);
                status = true;
            }
            return status;
        }

        public string SaveScheduledJob(AutoConsoleJobEditDTO autoConsoleJobEditDTO)
        {
            if (autoConsoleJobEditDTO == null || autoConsoleJobEditDTO.DaysToRunEndDate == null)
            {
                return "Failed";
            }

            #region create request URL
            string StartDate = string.Empty, EndDate = string.Empty, pickUp = string.Empty, dropOff = string.Empty;

            if (autoConsoleJobEditDTO.IsStandardShop)
            {
                if (autoConsoleJobEditDTO.StartDate.HasValue && !string.IsNullOrEmpty(autoConsoleJobEditDTO.StartDate.ToString()))
                {
                    StartDate = autoConsoleJobEditDTO.StartDate.Value.ToString("yyyy-MM-dd");

                    StartDate += " " + autoConsoleJobEditDTO.PickUpTime;
                }
                if (autoConsoleJobEditDTO.EndDate.HasValue && !string.IsNullOrEmpty(autoConsoleJobEditDTO.EndDate.ToString()))
                {
                    EndDate = autoConsoleJobEditDTO.EndDate.Value.ToString("yyyy-MM-dd");
                    EndDate += " " + autoConsoleJobEditDTO.DropOffTime;
                }
                pickUp = DateTime.ParseExact(StartDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");
                dropOff = DateTime.ParseExact(EndDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                pickUp = "{{StartDateOffset}}";
                dropOff = "{{EndDateOffset}}";

            }

            //TODO make this step later for horizon shop


            List<string> companyCodes = _companyService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList();
            if (autoConsoleJobEditDTO.ScrapperSource == "CPR")
            {
                companyCodes.RemoveAll(d => d == "ZI" || d == "ZE");
            }

            //Sample URL
            //requestUrl = "http://172.27.61.10/cgi-bin/dot_net_service_call_to_scraper.cgi?datasource=ARC&loc=MCO&carclasses=ECAR,CCAR,LFAR,STAR,LCAR,FCAR,PCAR,ICAR,SCAR&lor=2&strtdt=2015-07-01T17:00:00Z&enddt=2015-07-03T17:00:00Z&searchid=7111&vndr=MW,ZA,ZI,EY,FX,ZD,ZE,EZ,AC,AD,SV,ET,SX,ZL,AL,FF,ZR,ZT";
            string requestUrl = string.Empty;
            RateHighwayDTO objRateHighwayDTO = null;
            if (autoConsoleJobEditDTO.ProviderCode == "RH")
            {
                //Create ratehighway dto
                SearchModelDTO objSearchModelDTO = new SearchModelDTO();

                objSearchModelDTO.CarClasses = autoConsoleJobEditDTO.CarClasses;
                objSearchModelDTO.ScrapperSource = autoConsoleJobEditDTO.ScrapperSource;
                if (!pickUp.Contains("{"))
                {
                    objSearchModelDTO.StartDate = Convert.ToDateTime(autoConsoleJobEditDTO.StartDate);
                    objSearchModelDTO.PickUpTime = autoConsoleJobEditDTO.PickUpTime;
                }
                if (!dropOff.Contains("{"))
                {
                    objSearchModelDTO.EndDate = Convert.ToDateTime(autoConsoleJobEditDTO.EndDate);
                    objSearchModelDTO.DropOffTime = autoConsoleJobEditDTO.DropOffTime;
                }
                objSearchModelDTO.location = autoConsoleJobEditDTO.location;
                objSearchModelDTO.RentalLengthIDs = autoConsoleJobEditDTO.RentalLengthIDs;
                objSearchModelDTO.VendorCodes = String.Join(",", companyCodes);

                objRateHighwayDTO = new RateHighwayDTO(objSearchModelDTO);
                if (string.IsNullOrEmpty(objRateHighwayDTO.PickupDateTime))
                {
                    objRateHighwayDTO.PickupDateTime = pickUp;
                }
                if (string.IsNullOrEmpty(objRateHighwayDTO.ReturnDateTime))
                {
                    objRateHighwayDTO.ReturnDateTime = dropOff;
                }

                requestUrl = objRateHighwayDTO.AdhocRequest;
            }
            else
            {
                //Select the scraping server based on TSD access rights
                User user = _userService.GetAll().Where(d => d.ID == autoConsoleJobEditDTO.LoggedInUserId).FirstOrDefault();
                string scrappingServerUrl = string.Empty;
                if (user.IsTSDUpdateAccess.HasValue && user.IsTSDUpdateAccess.Value)
                {
                    scrappingServerUrl = _scrappingServersService.GetScrappingUrl(EnumScrappingServers.AutomationShop);
                }
                else
                {
                    scrappingServerUrl = _scrappingServersService.GetScrappingUrl(EnumScrappingServers.ReadOnlyShop);
                }

                //Replace {{SEARCHSUMMARYID}} later
                requestUrl = scrappingServerUrl + "datasource=" + autoConsoleJobEditDTO.ScrapperSource + "&loc=" + autoConsoleJobEditDTO.location + "&carclasses=" + autoConsoleJobEditDTO.CarClasses
                    + "&lor=" + autoConsoleJobEditDTO.RentalLengthIDs + "&strtdt=" + pickUp + "&enddt=" + dropOff + "&searchid={{SEARCHSUMMARYID}}" +
                    "&vndr=" + String.Join(",", companyCodes) + "&user=" + autoConsoleJobEditDTO.UserName;
            }
            #endregion


            #region Save scheduled Job
            ScheduledJob scheduledJob;

            FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
            ftbAutomationScenarioDTO = CheckFTBJobStatus(autoConsoleJobEditDTO);

            if (autoConsoleJobEditDTO.JobId.HasValue)
            {
                //This is existing job and we need to modify it
                scheduledJob = _context.ScheduledJobs.Where(obj => obj.ID == autoConsoleJobEditDTO.JobId).FirstOrDefault();
                scheduledJob.UpdatedBy = autoConsoleJobEditDTO.LoggedInUserId;
                scheduledJob.UpdatedDateTime = DateTime.Now;
                _context.Entry(scheduledJob).State = System.Data.Entity.EntityState.Modified;
                _cacheManager.Remove(typeof(ScheduledJob).ToString() + "_" + autoConsoleJobEditDTO.JobId);
            }
            else
            {
                scheduledJob = new ScheduledJob();
                scheduledJob.CreatedBy = autoConsoleJobEditDTO.LoggedInUserId;
                scheduledJob.CreatedDateTime = DateTime.Now;



                _context.ScheduledJobs.Add(scheduledJob);

            }
            if (scheduledJob == null)
            {
                return "Failed";
            }
            if (objRateHighwayDTO != null)
            {
                scheduledJob.PostData = Newtonsoft.Json.JsonConvert.SerializeObject(objRateHighwayDTO);
            }
            scheduledJob.ProviderId = autoConsoleJobEditDTO.ProviderId;
            scheduledJob.DaysToRunEndDate = autoConsoleJobEditDTO.DaysToRunEndDate.AddDays(1).AddSeconds(-1);

            //only enable new jobs
            if (!autoConsoleJobEditDTO.JobId.HasValue && scheduledJob.DaysToRunEndDate.Date >= DateTime.Now.Date)
            {
                scheduledJob.IsEnabled = true;
            }

            scheduledJob.ScheduledJobFrequencyID = autoConsoleJobEditDTO.ScheduledJobFrequencyID;
            scheduledJob.CustomHoursToRun = autoConsoleJobEditDTO.CustomHoursToRun;
            scheduledJob.CustomMinutesToRun = autoConsoleJobEditDTO.CustomMinutesToRun;
            scheduledJob.RunTime = autoConsoleJobEditDTO.RunTime;
            scheduledJob.RunDay = autoConsoleJobEditDTO.RunDay;

            scheduledJob.JobScheduleWeekDays = autoConsoleJobEditDTO.JobScheduleWeekDays;
            scheduledJob.DaysToRunStartDate = autoConsoleJobEditDTO.DaysToRunStartDate;

            scheduledJob.IsStandardShop = autoConsoleJobEditDTO.IsStandardShop;
            scheduledJob.StartDate = autoConsoleJobEditDTO.StartDate;
            scheduledJob.EndDate = autoConsoleJobEditDTO.EndDate;
            scheduledJob.StartDateOffset = autoConsoleJobEditDTO.StartDateOffset;
            scheduledJob.EndDateOffset = autoConsoleJobEditDTO.EndDateOffset;
            scheduledJob.PickUpTime = autoConsoleJobEditDTO.PickUpTime;
            scheduledJob.DropOffTime = autoConsoleJobEditDTO.DropOffTime;

            scheduledJob.IsWideGapTemplate = autoConsoleJobEditDTO.IsWideGapTemplate;
            scheduledJob.IsActiveTethering = autoConsoleJobEditDTO.IsActiveTethering;
            scheduledJob.TSDUpdateWeekDay = autoConsoleJobEditDTO.TSDUpdateWeekDay;

            scheduledJob.ScrapperSourceIDs = autoConsoleJobEditDTO.ScrapperSourceIDs;
            scheduledJob.LocationBrandIDs = autoConsoleJobEditDTO.LocationBrandIDs;
            scheduledJob.CarClassesIDs = autoConsoleJobEditDTO.CarClassesIDs;
            scheduledJob.RentalLengthIDs = autoConsoleJobEditDTO.RentalLengthIDs;

            scheduledJob.UpdatedBy = autoConsoleJobEditDTO.LoggedInUserId;
            scheduledJob.UpdatedDateTime = DateTime.Now;

            //set GOV shop 
            scheduledJob.IsGov = Convert.ToBoolean(autoConsoleJobEditDTO.IsGov);
            scheduledJob.IsGovTemplate = autoConsoleJobEditDTO.IsGovTemplate;
            scheduledJob.IsStopByFTB = false;
            scheduledJob.CompeteOnBase = autoConsoleJobEditDTO.CompeteOnBase;
            scheduledJob.IsOpaqueActive = autoConsoleJobEditDTO.IsOpaqueActive;
            if (autoConsoleJobEditDTO.IsOpaqueActive)
            {
                scheduledJob.OpaqueRateCodes = autoConsoleJobEditDTO.OpaqueRateCodes;                
            }
            else
            {
                scheduledJob.OpaqueRateCodes = null;      
            }
            
            SetNextRunDateTime(scheduledJob);

            scheduledJob.RequestURL = requestUrl;

            if (scheduledJob.NextRunDateTime != null)
            {
                if (ftbAutomationScenarioDTO.IsReturnMsg)
                {
                    if (ftbAutomationScenarioDTO.ReturnMessage.ToLower() == "notconfiguredblackoutdates")
                    {
                        return ftbAutomationScenarioDTO.ReturnMessage;
                    }
                    else
                    {
                        return ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString();
                    }
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(ScheduledJob).ToString());
                _scheduledJobMinRatesService.SaveScheduledJobMinRates(autoConsoleJobEditDTO.ScheduledJobMinRates, scheduledJob.ID);
                _scheduledJobTetheringsService.SaveScheduledJobTethering(autoConsoleJobEditDTO.ScheduledJobTetherings, scheduledJob.ID);
                _ruleSetService.updateAutomationRuleSet(scheduledJob.ID, autoConsoleJobEditDTO.IntermediateID);//used for swipeid to scheduledjobid using for new job creation
                if (autoConsoleJobEditDTO.IsOpaqueActive)
                {
                    _scheduledJobOpaqueValuesService.SaveOpaqueValues(autoConsoleJobEditDTO.OpaqueRates, scheduledJob.ID);
                }
                else
                {
                    _scheduledJobOpaqueValuesService.RemoveAllOpaqueValues(scheduledJob.ID);
                }
            }
            else
            {
                return "NoMatchingDate";
            }

            #endregion
            return "Success";
        }
        public FTBAutomationScenarioDTO CheckFTBJobStatus(AutoConsoleJobEditDTO autoConsoleJobEditDTO)
        {
            FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
            ftbAutomationScenarioDTO.StartDate = Convert.ToDateTime(autoConsoleJobEditDTO.StartDate);
            ftbAutomationScenarioDTO.EndDate = Convert.ToDateTime(autoConsoleJobEditDTO.EndDate);
            ftbAutomationScenarioDTO.IsFTBJob = false;
            ftbAutomationScenarioDTO.LocationBrandID = Convert.ToInt64(autoConsoleJobEditDTO.LocationBrandIDs);
            ftbAutomationScenarioDTO.LoggedUserID = autoConsoleJobEditDTO.LoggedInUserId;
            ftbAutomationScenarioDTO = _ftbScheduleJobService.CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);
            return ftbAutomationScenarioDTO;
        }

        public void SetNextRunDateTime(ScheduledJob scheduledJob)
        {
            //get ScheduledJobFrequency for the job
            ScheduledJobFrequency scheduledJobFrequency = _scheduledJobFrequencyService.GetById(scheduledJob.ScheduledJobFrequencyID, false);

            if (scheduledJobFrequency != null && scheduledJobFrequency.ID > 0)
            {
                DateTime nextRunDateTime = scheduledJob.DaysToRunStartDate > DateTime.Now ? scheduledJob.DaysToRunStartDate : DateTime.Now;//.AddMinutes(-2);
                //Current time without sec;				
                DateTime nextRunDateTimeWithoutSec = nextRunDateTime.Date.AddHours(nextRunDateTime.Hour).AddMinutes(nextRunDateTime.Minute);
                IEnumerable<WeekDay> weekDays = _weekdayService.GetAll();
                long[] jobScheduledWeekDayIds = Common.StringToLongList(scheduledJob.JobScheduleWeekDays).ToArray();
                string[] jobScheduledWeekDays = new string[7];
                if (jobScheduledWeekDayIds != null && jobScheduledWeekDayIds.Count() > 0)
                {
                    Array.Sort(jobScheduledWeekDayIds);
                    jobScheduledWeekDays = weekDays.Where(obj => jobScheduledWeekDayIds.Contains(obj.ID)).Select(obj1 => obj1.Day).ToArray();
                }
                bool nextRunFound = false;
                bool endDayReached = false;
                if (scheduledJobFrequency.MinuteInterval.HasValue)
                {
                    #region Minute Interval
                    do
                    {
                        if (!string.IsNullOrEmpty(scheduledJob.JobScheduleWeekDays))
                        {
                            nextRunDateTime = GetNextRunUsingJobScheduleWeekDays(jobScheduledWeekDays, scheduledJob, nextRunDateTime, out nextRunFound, out endDayReached);
                        }
                        //If todays date is start date then calculate time
                        if (nextRunDateTime.Date == DateTime.Now.Date)
                        {
                            if (nextRunDateTime <= DateTime.Now)
                            {
                                nextRunDateTime = nextRunDateTimeWithoutSec.AddMinutes(scheduledJobFrequency.MinuteInterval.Value
                                    - (nextRunDateTimeWithoutSec.Minute % scheduledJobFrequency.MinuteInterval.Value));
                            }
                        }
                        else
                        {
                            nextRunDateTime = nextRunDateTime.Date;
                        }
                        //Also consider 2 min delay for saving the data and service trigger
                        if (nextRunDateTime != null && nextRunDateTime.AddMinutes(-2) < DateTime.Now)
                        {
                            //Run on next elapsed time
                            nextRunDateTime = nextRunDateTime.AddMinutes(scheduledJobFrequency.MinuteInterval.Value);
                            nextRunFound = false;
                        }
                    } while (!nextRunFound && !endDayReached);
                    scheduledJob.NextRunDateTime = (nextRunFound && scheduledJob.DaysToRunEndDate >= nextRunDateTime) ? nextRunDateTime : (DateTime?)null;
                    #endregion
                }
                else if (scheduledJobFrequency.DayInterval.HasValue)
                {
                    #region Day Interval
                    //daily, weekly
                    if (!string.IsNullOrEmpty(scheduledJob.JobScheduleWeekDays))
                    {
                        string runDate = nextRunDateTimeWithoutSec.ToString("yyyy-MM-dd") + " " + scheduledJob.RunTime;
                        nextRunDateTime = DateTime.ParseExact(runDate, "yyyy-MM-dd h:mm tt", CultureInfo.CurrentCulture);
                        do
                        {
                            nextRunDateTime = GetNextRunUsingJobScheduleWeekDays(jobScheduledWeekDays, scheduledJob, nextRunDateTime, out nextRunFound, out endDayReached);
                            //Also consider 2 min delay for saving the data and service trigger
                            if (nextRunDateTime != null && nextRunDateTime.AddMinutes(-2) < DateTime.Now)
                            {
                                //Run on next elapsed time
                                nextRunDateTime = nextRunDateTime.AddDays(scheduledJobFrequency.DayInterval.Value);
                                nextRunFound = false;
                            }
                        } while (!nextRunFound && !endDayReached);
                    }
                    //once a month
                    else if (scheduledJob.RunDay.HasValue)
                    {
                        string runDate = string.Empty;
                        int monthAdd = -1;
                        DateTime date = nextRunDateTime;
                        do
                        {
                            monthAdd++;
                            date = date.AddMonths(monthAdd);
                            runDate = date.Year + "-" + (date.Month) + "-" + scheduledJob.RunDay.Value + " " + scheduledJob.RunTime;

                        }
                        while (!(DateTime.TryParseExact(runDate, "yyyy-M-d hh:mm tt", CultureInfo.CurrentCulture, DateTimeStyles.None, out nextRunDateTime)
                        && nextRunDateTime.AddMinutes(-2) > DateTime.Now));

                    }
                    scheduledJob.NextRunDateTime = (scheduledJob.DaysToRunEndDate >= nextRunDateTime) ? nextRunDateTime : (DateTime?)null;
                    #endregion
                }
                else if (scheduledJobFrequency.Name.Contains("Custom")
                    && !string.IsNullOrEmpty(scheduledJob.CustomHoursToRun) && !string.IsNullOrEmpty(scheduledJob.CustomMinutesToRun))
                {
                    #region Custom
                    long[] jobScheduleHrs = Common.StringToLongList(scheduledJob.CustomHoursToRun).ToArray();
                    long[] jobScheduleMin = Common.StringToLongList(scheduledJob.CustomMinutesToRun).ToArray();
                    if (jobScheduleHrs != null && jobScheduleHrs.Count() > 0 && jobScheduleMin != null && jobScheduleMin.Count() > 0)
                    {
                        Array.Sort(jobScheduleHrs);
                        Array.Sort(jobScheduleMin);
                        do
                        {
                            bool breakParent = false;
                            foreach (long hr in jobScheduleHrs)
                            {
                                foreach (long min in jobScheduleMin)
                                {
                                    DateTime dt = DateTime.ParseExact(nextRunDateTimeWithoutSec.ToString("yyyy-MM-dd") + " " + hr + ":" + min, "yyyy-MM-dd H:m", CultureInfo.CurrentCulture);
                                    nextRunDateTime = GetNextRunUsingJobScheduleWeekDays(jobScheduledWeekDays, scheduledJob, dt, out nextRunFound, out endDayReached);
                                    if (nextRunDateTime.AddMinutes(-2) >= DateTime.Now)
                                    {
                                        breakParent = true;
                                        break;
                                    }
                                }
                                if (breakParent)
                                {
                                    break;
                                }
                            }
                            if (nextRunDateTime.AddMinutes(-2) < DateTime.Now)
                            {
                                nextRunFound = false;
                            }
                            nextRunDateTimeWithoutSec = nextRunDateTimeWithoutSec.AddDays(1);
                        } while (!nextRunFound && !endDayReached);
                        scheduledJob.NextRunDateTime = (nextRunFound && scheduledJob.DaysToRunEndDate >= nextRunDateTime) ? nextRunDateTime : (DateTime?)null;
                    }
                    #endregion
                }
            }
        }

        private DateTime GetNextRunUsingJobScheduleWeekDays(string[] jobScheduledWeekDays, ScheduledJob scheduledJob, DateTime nextRunDateTime, out bool nextRunFound, out bool endDayReached)
        {
            nextRunFound = false;
            endDayReached = false;
            for (; nextRunDateTime <= scheduledJob.DaysToRunEndDate; nextRunDateTime = nextRunDateTime.AddDays(1))
            {
                if (jobScheduledWeekDays.Contains(nextRunDateTime.DayOfWeek.ToString()))
                {
                    nextRunFound = true;
                    break;
                }
            }
            if (!nextRunFound && nextRunDateTime >= scheduledJob.DaysToRunEndDate)
            {
                endDayReached = true;
            }
            return nextRunDateTime;
        }

        public List<AutomationConsoleViewJobsDTO> GetAllScheduledJobs(string userLocationBrandIds)
        {
            _cacheManager.Remove(typeof(SearchSummary).ToString());
            List<AutomationConsoleViewJobsDTO> automationJobs = new List<AutomationConsoleViewJobsDTO>();
            List<long> UserLocationIds = !string.IsNullOrEmpty(userLocationBrandIds) ? userLocationBrandIds.Split(',').Select(a => Convert.ToInt64(a)).ToList() : new List<long>();

            if (UserLocationIds == null || UserLocationIds.Count() == 0)
            {
                return automationJobs;
            }


            Dictionary<long, string> weekDays = _context.WeekDays.ToDictionary(a => a.ID, a => a.Day);
            Dictionary<long, string> rentalLengths = _context.RentalLengths.ToDictionary(a => a.ID, a => a.Code);
            Dictionary<long, string> carClasses = _context.CarClasses.Where(a => !a.IsDeleted).ToDictionary(a => a.ID, a => a.Code);
            Dictionary<long, string> sources = _context.ScrapperSources.ToDictionary(a => a.ID, a => a.Name);

            var getAllautomationJobs = (from scheduledJob in _context.ScheduledJobs
                                        join user in _context.Users on scheduledJob.CreatedBy equals user.ID
                                        let searchSummary = _context.SearchSummaries.Where(x => x.ScheduledJobID == scheduledJob.ID &&
                                                                                                x.ShopType.Equals(ShopTypes.SummaryShop) &&
                                                                                                x.StatusID < (long)Enumerations.ShopStatus.Deleted).OrderByDescending(x => x.ID).FirstOrDefault()
                                        let provider = _context.Providers.Where(x => x.ID == scheduledJob.ProviderId).FirstOrDefault()
                                        where !scheduledJob.IsDeleted && !user.IsDeleted
                                        select new
                                        {
                                            ID = scheduledJob.ID,
                                            CarClassesIDs = scheduledJob.CarClassesIDs,
                                            RentalLengthIDs = scheduledJob.RentalLengthIDs,
                                            CreatedDateTime = scheduledJob.CreatedDateTime,
                                            DaysToRunStartDate = scheduledJob.DaysToRunStartDate,
                                            DaysToRunEndDate = scheduledJob.DaysToRunEndDate,
                                            IsWideGapTemplate = scheduledJob.IsWideGapTemplate,
                                            IsEnabled = scheduledJob.IsEnabled,
                                            IsDeleted = scheduledJob.IsDeleted,
                                            IsStandardShop = scheduledJob.IsStandardShop,
                                            StartDate = scheduledJob.StartDate,
                                            EndDate = scheduledJob.EndDate,
                                            StartDateOffset = scheduledJob.StartDateOffset,
                                            EndDateOffset = scheduledJob.EndDateOffset,
                                            ExecutionInProgress = scheduledJob.ExecutionInProgress,
                                            CreatedByFirstName = user.FirstName,
                                            CreatedByLastName = user.LastName,
                                            //IsTSDUpdateAccess = user.IsTSDUpdateAccess.Value,
                                            IsActiveTethering = scheduledJob.IsActiveTethering,
                                            LocationBrandIDs = scheduledJob.LocationBrandIDs,
                                            IsPresentNextRunDateTime = scheduledJob.NextRunDateTime,
                                            CreatedByID = scheduledJob.CreatedBy,
                                            LastRunDateTime = scheduledJob.LastRunDateTime,
                                            ScrapperSourceIDs = scheduledJob.ScrapperSourceIDs,
                                            IsGov = scheduledJob.IsGov,
                                            IsStopByFTB = scheduledJob.IsStopByFTB,
                                            ProviderId = provider.ID,
                                            CanInititateShop = !provider.Code.Equals(ProviderConstants.RateHighway),
                                            ShopCreatedBy = searchSummary != null ? searchSummary.CreatedBy : default(long),
                                            SearchSummaryId = searchSummary != null ? searchSummary.ID : default(long),
                                            IsInitiateShopEnable = searchSummary == null || searchSummary.StatusID >= (long)Enumerations.ShopStatus.ProcessComplete,
                                            IsShopComplete = searchSummary != null && searchSummary.StatusID == (long)Enumerations.ShopStatus.ProcessComplete,
                                            SearchSummaryStatus = searchSummary != null ? searchSummary.StatusID : default(long),
                                            TSDupdateWeekDays = string.IsNullOrEmpty(scheduledJob.TSDUpdateWeekDay) ? false : true
                                        }).ToList();


            if (getAllautomationJobs == null || getAllautomationJobs.Count == 0)
            {
                return null;
            }

            var searchSummaryData = _context.SearchSummaries.Where(x => x.ScheduledJobID != null).ToList();

            automationJobs = getAllautomationJobs.OrderBy(a => a.ID).Select(a => new AutomationConsoleViewJobsDTO
            {
                Id = a.ID,
                //  CarClasses = string.Join(", ", a.CarClassesIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Where(b => carClasses.ContainsKey(Convert.ToInt64(b))).Select(b => carClasses[Convert.ToInt64(b)])),
                CarClasses = a.CarClassesIDs,
                RentalLengths = string.Join(", ", a.RentalLengthIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => rentalLengths[Convert.ToInt64(b)])),
                CreatedDate = a.CreatedDateTime,
                CreatedDateAsString = a.CreatedDateTime.ToString(DateFormats.MMDDYYYY),
                RunDates = a.DaysToRunStartDate.ToString(DateFormats.MMDDYYYY) + "..." + a.DaysToRunEndDate.ToString(DateFormats.MMDDYYYY),
                IsWideGap = a.IsWideGapTemplate,
                IsEnabled = a.IsEnabled,
                IsDeleted = a.IsDeleted,
                IsStandardShop = a.IsStandardShop,
                ShopDates = a.IsStandardShop ? ((a.StartDate.HasValue ? a.StartDate.Value.ToString(DateFormats.MMDDYYYY) : string.Empty) + "..." + (a.EndDate.HasValue ? a.EndDate.Value.ToString(DateFormats.MMDDYYYY) : string.Empty)) : ("+" + a.StartDateOffset + "...+" + a.EndDateOffset + " days"),
                ExecutionInProgress = a.ExecutionInProgress,
                CreatedBy = a.CreatedByFirstName + " " + a.CreatedByLastName,
                IsReadOnly = a.TSDupdateWeekDays,
                CreatedByID = a.CreatedByID,
                IsActiveTethering = a.IsActiveTethering,
                LocationBrands = a.LocationBrandIDs,
                IsPresentNextRunDateTime = (a.IsPresentNextRunDateTime != null) ? true : false,
                BrandLocationsLists = !string.IsNullOrEmpty(a.LocationBrandIDs) ? a.LocationBrandIDs.Split(',').Select(c => Convert.ToInt64(c)).ToList() : null,
                IsReviewPending = searchSummaryData == null ? false : searchSummaryData.Where(objSearchSummary => objSearchSummary.ScheduledJobID == a.ID && objSearchSummary.IsReviewed.HasValue && !objSearchSummary.IsReviewed.Value).Count() > 0,
                AreReviewButtonsRequired = searchSummaryData == null ? false : searchSummaryData.Where(objSearchSummary => objSearchSummary.ScheduledJobID == a.ID).Count() != searchSummaryData.Where(objSearchSummary => objSearchSummary.ScheduledJobID == a.ID && !objSearchSummary.IsReviewed.HasValue).Count(),
                Status = a.ExecutionInProgress ? ShopStatusToDisplay.InProgress : (a.IsPresentNextRunDateTime != null ? (a.IsEnabled ? ShopStatusToDisplay.Scheduled : ShopStatusToDisplay.Stopped) : ShopStatusToDisplay.Finished),
                Source = sources[Convert.ToInt64(a.ScrapperSourceIDs)],
                IsGov = a.IsGov.HasValue ? a.IsGov.Value : false,
                IsStopByFTB = a.IsStopByFTB.HasValue ? a.IsStopByFTB.Value : false,
                ProviderId = a.ProviderId,
                SearchSummaryId = a.SearchSummaryId,
                CanInititateShop = a.CanInititateShop,
                IsInitiateShopEnable = a.IsInitiateShopEnable && (a.IsStandardShop ? a.EndDate > DateTime.Now : (a.EndDateOffset.HasValue ? DateTime.Now.AddDays(a.EndDateOffset.Value) > DateTime.Now : false)),
                IsShopComplete = a.IsShopComplete,
                SearchSummaryStatus = GetStatusName(a.SearchSummaryStatus),
                RentalLengthIDs = a.RentalLengthIDs,
                ScrapperSourceIDs = a.ScrapperSourceIDs,
                ShopCreatedBy = a.ShopCreatedBy,
                StartDate = a.IsStandardShop ? a.StartDate.Value.ToString(DateFormats.MMDDYYYY) : (a.StartDateOffset.HasValue ? DateTime.Now.AddDays(a.StartDateOffset.Value).ToString(DateFormats.MMDDYYYY) : DateTime.Now.ToString(DateFormats.MMDDYYYY)),
                EndDate = a.IsStandardShop ? a.EndDate.Value.ToString(DateFormats.MMDDYYYY) : (a.StartDateOffset.HasValue ? DateTime.Now.AddDays(a.EndDateOffset.Value).ToString(DateFormats.MMDDYYYY) : DateTime.Now.ToString(DateFormats.MMDDYYYY)),
                LastRunDate = a.LastRunDateTime.HasValue ? a.LastRunDateTime.Value.ToString(DateFormats.MMDDYYYY + " HH:mm") : "-",
                NextRunDate = a.IsPresentNextRunDateTime.HasValue ? a.IsPresentNextRunDateTime.Value.ToString(DateFormats.MMDDYYYY + " HH:mm") : "-",
            }).Where(a => a.BrandLocationsLists != null && a.BrandLocationsLists.Intersect(UserLocationIds).Any()).ToList();

            return automationJobs;
        }

        private string GetStatusName(long statusId)
        {
            string status = String.Empty;

            switch (statusId)
            {
                case 1:
                    status = ShopStatusToDisplay.Scheduled;
                    break;
                case 2:
                    status = ShopStatusToDisplay.InProgress;
                    break;
                case 3:
                    status = ShopStatusToDisplay.InProgress;
                    break;
                case 4:
                    status = ShopStatusToDisplay.Complete;
                    break;
                case 5:
                    status = ShopStatusToDisplay.Failed;
                    break;
                default:
                    status = "-";
                    break;
            }

            return status;
        }

        #region Automation Shops Methods
        public void RunAutomationShops()
        {
            //Step1: Get all scheduled job with NextRunTime == this time and isEnabled == true and IsDeleted == false
            DateTime thisTime = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            DateTime thisTimeWithNextMin = thisTime.AddMinutes(1);
            List<ScheduledJob> scheduledJobsToRun = _context.ScheduledJobs.Where(obj => obj.IsEnabled && !obj.IsDeleted && obj.NextRunDateTime.HasValue
                && obj.NextRunDateTime <= thisTimeWithNextMin && obj.DaysToRunEndDate >= thisTime).ToList();

            Dictionary<long, string> dicProviders = _providerService.GetAll().ToDictionary(d => d.ID, d => d.Code);

            if (scheduledJobsToRun != null && scheduledJobsToRun.Count() > 0)
            {

                long[] allLocationBrands = _locationBrandService.GetAll().Where(obj => !obj.IsDeleted).ToList().Select(obj => obj.ID).ToArray();

                #region Set LastRuntime, Calculate and set NextRunTime, set ExecutionInProgress
                DateTime DateWithTimeNoSeconds = thisTime;
                foreach (ScheduledJob job in scheduledJobsToRun)
                {
                    //For scheduled Job, check location-Brands is not deleted, If yes then delete such job	
                    long[] locationBrandsForJob = Common.StringToLongList(job.LocationBrandIDs).ToArray();
                    if (!locationBrandsForJob.Any(id => allLocationBrands.Contains(id)))
                    {
                        _context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                        _cacheManager.Remove(typeof(ScheduledJob).ToString() + "_" + job.ID);
                        job.IsDeleted = true;
                        continue;
                    }

                    _context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    _cacheManager.Remove(typeof(ScheduledJob).ToString() + "_" + job.ID);
                    UpdateLastRunNextRunAndExecutionInProgress(job, DateWithTimeNoSeconds);
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(ScheduledJob).ToString());
                #endregion
            }
            else
            {
                return;
            }

            #region Generate URL and call scraper API
            foreach (ScheduledJob scheduledJob in scheduledJobsToRun)
            {
                string requestURL = scheduledJob.RequestURL;
                DateTime startDate = scheduledJob.StartDate.HasValue ? scheduledJob.StartDate.Value : DateTime.Now;
                DateTime endDate = scheduledJob.EndDate.HasValue ? scheduledJob.EndDate.Value : DateTime.Now;
                if (!scheduledJob.IsStandardShop)
                {
                    requestURL = CreateRequestURLForHorizonShop(scheduledJob, out startDate, out endDate);
                }
                else if (scheduledJob.StartDate.Value < DateTime.Now.Date)
                {
                    DateTime todaysDate = DateTime.Now.Date;
                    todaysDate = DateTime.Parse(todaysDate.ToString("yyyy-MM-dd") + " " + scheduledJob.PickUpTime);
                    var replaceRequestURL = HttpUtility.ParseQueryString(requestURL.Substring(requestURL.IndexOf('?')), System.Text.Encoding.UTF8);
                    replaceRequestURL.Set("strtdt", todaysDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    //Actual assign request url with changed startdate
                    requestURL = HttpUtility.UrlDecode((requestURL.Substring(0, requestURL.IndexOf('?'))).ToString() + '?' + replaceRequestURL.ToString());

                    startDate = DateTime.Parse(todaysDate.ToString("yyyy-MM-dd"));
                }
                bool searchStatus = false;
                if (scheduledJob.ProviderId.HasValue)
                {
                    string selectedAPI;
                    dicProviders.TryGetValue(scheduledJob.ProviderId.Value, out selectedAPI);
                    switch (selectedAPI)
                    {
                        case "RH":
                            searchStatus = _searchSummaryService.InitializeRateHighwayAutomationSearch(scheduledJob, requestURL, startDate, endDate, "RH");
                            break;
                        default:
                        case "SS":
                            searchStatus = _searchSummaryService.InitializeAutomationSearch(scheduledJob, requestURL, startDate, endDate);
                            break;
                    }
                }
                else
                {
                    searchStatus = _searchSummaryService.InitializeAutomationSearch(scheduledJob, requestURL, startDate, endDate);
                }

                if (string.IsNullOrEmpty(requestURL) || !searchStatus)
                {
                    ScheduledJob job = this.GetById(scheduledJob.ID, false);
                    job.ExecutionInProgress = false;
                    this.Update(job);
                }
            }
            #endregion
        }

        private string CreateRequestURLForHorizonShop(ScheduledJob scheduledJob, out DateTime startDate, out DateTime endDate)
        {
            string requestURL = string.Empty;
            string StartDate = string.Empty, EndDate = string.Empty, pickUp = string.Empty, dropOff = string.Empty;
            startDate = scheduledJob.DaysToRunStartDate; endDate = scheduledJob.DaysToRunEndDate;
            DateTime todaysDate = DateTime.Now.Date;
            if (scheduledJob.StartDateOffset.HasValue && scheduledJob.EndDateOffset.HasValue)
            {
                StartDate = todaysDate.AddDays(scheduledJob.StartDateOffset.Value).ToString("yyyy-MM-dd");
                StartDate += " " + scheduledJob.PickUpTime;


                EndDate = todaysDate.AddDays(scheduledJob.EndDateOffset.Value).ToString("yyyy-MM-dd");
                EndDate += " " + scheduledJob.DropOffTime;

                startDate = DateTime.ParseExact(StartDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture);
                endDate = DateTime.ParseExact(EndDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture);

                pickUp = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                dropOff = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

                requestURL = scheduledJob.RequestURL.Replace("{{StartDateOffset}}", pickUp);
                requestURL = requestURL.Replace("{{EndDateOffset}}", dropOff);
                return requestURL;
            }
            return string.Empty;

        }

        private void UpdateLastRunNextRunAndExecutionInProgress(ScheduledJob job, DateTime DateWithTimeNoSeconds)
        {
            job.LastRunDateTime = DateWithTimeNoSeconds;
            SetNextRunDateTime(job);
            job.ExecutionInProgress = true;
        }
        #endregion

        private SearchResultDTO GetSearchGridDailyView(long searchSummaryId, long locationId, long brandId, long rentalLengthId, string selectedDate)
        {
            DateTime _arrivalDate = DateTime.ParseExact(selectedDate, "MMddyyyy", CultureInfo.InvariantCulture);
            SearchResultProcessedData searchResultProcessedData = _searchResultsProcessedService.GetBySearchSummaryId(searchSummaryId).Where(obj => obj.ClassicViewJSONResult.Equals("Daily View Record", StringComparison.OrdinalIgnoreCase)
                 && obj.LocationID == locationId && obj.RentalLengthID == rentalLengthId && obj.DateFilter == _arrivalDate).FirstOrDefault();
            if (searchResultProcessedData != null && !string.IsNullOrEmpty(searchResultProcessedData.DailyViewJSONResult))
            {
                List<SearchResultSuggestedRateDTO> suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryId
                    && obj.LocationID == locationId && obj.BrandID == brandId && obj.RentalLengthID == rentalLengthId && obj.Date == _arrivalDate)
                    .Select(a => new SearchResultSuggestedRateDTO
                    {
                        ID = a.ID,
                        SearchSummaryID = a.SearchSummaryID,
                        RuleSetName = a.RuleSetName,
                        RuleSetID = a.RuleSetID,
                        BaseRate = a.BaseRate,
                        BrandID = a.BrandID,
                        CarClassID = a.CarClassID,
                        CompanyBaseRate = a.CompanyBaseRate,
                        CompanyTotalRate = a.CompanyTotalRate,
                        TotalRate = a.TotalRate,
                        Date = a.Date,
                        RentalLengthID = a.RentalLengthID,
                        LocationID = a.LocationID,
                        MaxBaseRate = a.MaxBaseRate,
                        MinBaseRate = a.MinBaseRate
                    }).ToList();
                return new SearchResultDTO
                {
                    finalData = searchResultProcessedData.DailyViewJSONResult,
                    suggestedRate = suggestedRates,
                    lastTSDUpdated = string.Empty
                };
            }
            return null;
        }

        private SearchResultDTO GetSearchGridClassicViewData(long searchSummaryId, long locationId, long brandId, long rentalLengthId, long carClassId)
        {
            SearchResultProcessedData searchResultProcessedData = _searchResultsProcessedService.GetBySearchSummaryId(searchSummaryId).Where(obj => obj.DailyViewJSONResult.Equals("Classic View Record", StringComparison.OrdinalIgnoreCase) &&
                obj.LocationID == locationId && obj.RentalLengthID == rentalLengthId && obj.CarClassID == carClassId).FirstOrDefault();

            if (searchResultProcessedData != null && !string.IsNullOrEmpty(searchResultProcessedData.ClassicViewJSONResult))
            {
                List<SearchResultSuggestedRateDTO> suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryId
                    && obj.LocationID == locationId && obj.BrandID == brandId && obj.RentalLengthID == rentalLengthId && obj.CarClassID == carClassId)
                    .Select(a => new SearchResultSuggestedRateDTO
                    {
                        ID = a.ID,
                        SearchSummaryID = a.SearchSummaryID,
                        RuleSetName = a.RuleSetName,
                        RuleSetID = a.RuleSetID,
                        BaseRate = a.BaseRate,
                        BrandID = a.BrandID,
                        CarClassID = a.CarClassID,
                        CompanyBaseRate = a.CompanyBaseRate,
                        CompanyTotalRate = a.CompanyTotalRate,
                        TotalRate = a.TotalRate,
                        Date = a.Date,
                        RentalLengthID = a.RentalLengthID,
                        LocationID = a.LocationID,
                        MaxBaseRate = a.MaxBaseRate,
                        MinBaseRate = a.MinBaseRate
                    }).ToList();

                return new SearchResultDTO
                {
                    finalData = searchResultProcessedData.ClassicViewJSONResult,
                    suggestedRate = suggestedRates,
                    lastTSDUpdated = string.Empty
                };
            }
            return null;
        }

        public IEnumerable<ScheduledJobOpaqueValuesDTO> getScheduledJobOpaqueRates(long scheduledJobId)
        {
            IEnumerable<ScheduledJobOpaqueValuesDTO> scheduledJobOpaqueValuesDTO = (from opaqueRates in _context.ScheduledJobOpaqueValues
                                                                                    where opaqueRates.ScheduledJobId == scheduledJobId
                                                                                    select new ScheduledJobOpaqueValuesDTO()
                                                                                    {
                                                                                        ScheduledJobId = opaqueRates.ScheduledJobId,
                                                                                        PercenValue = opaqueRates.PercentValue,
                                                                                        CarClassId = opaqueRates.CarClassId,
                                                                                        Id = opaqueRates.ID
                                                                                    }).ToList();
            return scheduledJobOpaqueValuesDTO;
        }        
    }
}
