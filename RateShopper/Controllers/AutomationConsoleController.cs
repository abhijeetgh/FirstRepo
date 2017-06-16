using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;
using System.Security.Claims;
using RateShopper.Compression;
using System.Threading.Tasks;

namespace RateShopper.Controllers
{
    [CompressFilter]
    [Authorize]
    [HandleError]
    public class AutomationConsoleController : Controller
    {
        private ILocationBrandService locationBrandService;
        private IUserService userService;
        private IScheduledJobService scheduledJobService;
        private IRentalLengthService rentalLengthService;
        private IWeekDayService weekDayService;
        private ICarClassService carClassService;
        private IScheduledJobMinRatesService scheduledJobMinRateService;
        private ILocationBrandCarClassService locationBrandCarClassService;
        private IGlobalTetherSettingService globalTetherService;
        private IScheduledJobTetheringsService scheduledJobTetheringsService;
        private IRuleSetService ruleSetService;
        private IJobTypeMapperService jobTypeMapperService;
        private Services.Data.IProvidersService dataProviderService;
        private IFTBScheduleJobService ftbScheduleJobService;
        private ICompanyService _companyService;
        private Providers.Interface.IProviderService _providerService;
        private IScrapperSourceService _scrapperSourceService;
        private IRateCodeService rateCodeService;

        public AutomationConsoleController(ILocationBrandService locationBrandService, IUserService userService, IScheduledJobService ScheduledJobService,
            IWeekDayService weekDayService, ICarClassService carClassService, IRentalLengthService rentalLengthService, IScheduledJobMinRatesService scheduledJobMinRateService,
            ILocationBrandCarClassService locationBrandCarClassService, IGlobalTetherSettingService globalTetherService,
            IScheduledJobTetheringsService scheduledJobTetheringsService, IRuleSetService ruleSetService, Services.Data.IProvidersService _dataProviderService, IJobTypeMapperService _jobTypeMapperService,
            IFTBScheduleJobService ftbScheduleJobService,
            ICompanyService companyService, Providers.Interface.IProviderService providerService,
            IScrapperSourceService scrapperSourceService, IRateCodeService rateCodeService)
        {
            this.locationBrandService = locationBrandService;
            this.userService = userService;
            this.scheduledJobService = ScheduledJobService;
            this.weekDayService = weekDayService;
            this.carClassService = carClassService;
            this.rentalLengthService = rentalLengthService;
            this.scheduledJobMinRateService = scheduledJobMinRateService;
            this.locationBrandCarClassService = locationBrandCarClassService;
            this.globalTetherService = globalTetherService;
            this.scheduledJobTetheringsService = scheduledJobTetheringsService;
            this.ruleSetService = ruleSetService;
            this.dataProviderService = _dataProviderService;
            this.jobTypeMapperService = _jobTypeMapperService;
            this.ftbScheduleJobService = ftbScheduleJobService;
            this._companyService = companyService;
            this._providerService = providerService;
            this._scrapperSourceService = scrapperSourceService;
            this.rateCodeService = rateCodeService;
        }

        //
        // GET: /AutomationConsole/

        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            bool isAutomation = Convert.ToBoolean(identity.Claims.Where(c => c.Type == "IsAutomation").Select(c => c.Value).FirstOrDefault());

            if (!isAutomation)
            {
                return RedirectToAction("RemoveAuthoriseUserData", "Account");
            }

            if (long.TryParse(UserID, out userId))
            {
                ViewBag.LocationBrands = userService.GetBrandLocationsByLoggedInUserId(userId).ToList();
                ViewBag.ScrapperSources = userService.GetScrapperSource(userId).OrderBy(d => d.Name);
            }
            else
            {
                ViewBag.LocationBrands = locationBrandService.GetAll(false).Where(a => !a.IsDeleted).Select(a => new ScheduleJobLocationBrandDTO { BrandLocationId = a.ID, Alias = a.LocationBrandAlias, BrandId = a.BrandID }).OrderBy(a => a.Alias).ToList();
            }

            ViewBag.Users = userService.GetAll(false).Where(a => !a.IsDeleted).Select(a => new ScheduleJobUserDTO { Id = a.ID, FirstName = a.FirstName, LastName = a.LastName }).OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();

            ViewBag.WeekDays = weekDayService.GetAll(false);
            ViewBag.RentalLengths = rentalLengthService.GetAll(false);
            // ViewBag.Frequencies = scheduledJobFrequencyService.GetAll(false);
            ViewBag.Frequencies = jobTypeMapperService.GetJobFrequencyTypes("Automation Console");
            ViewBag.Providers = dataProviderService.GetAllProviders();
            ViewBag.RateCodes = rateCodeService.GetAllRateCodes();
            return View();
        }


        [HttpGet]
        public JsonResult GetJobOccurences(long scheduledJobID, bool firstPage, long searchSummaryId)
        {
            FirstTimeResult result = scheduledJobService.GetScheduledRuns(scheduledJobID, firstPage, searchSummaryId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MarkResults(long searchSummaryId, long userId)
        {
            bool status = scheduledJobService.MarkResultsReviewed(searchSummaryId, userId);
            var jsonResult = Json(status, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetHistoryDetails(long summaryId, long locationId, long brandId, long rentalLengthId, string selectedDate, long carClassId, bool isDailyView)
        {
            SearchResultDTO searchResults = scheduledJobService.FilterSuggestedRates(summaryId, locationId, brandId, rentalLengthId, selectedDate, carClassId, isDailyView);
            if (searchResults != null)
            {
                var jsonResult = Json(searchResults, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return new EmptyResult();
        }

        public JsonResult GetLocations(long UserId)
        {
            return Json(userService.GetBrandLocation(UserId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCarClasses()
        {
            return Json(carClassService.GetAllCarClasses(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationCarClasses(long locationBrandId)
        {
            List<long> carClassIds = locationBrandCarClassService.GetLocationCarClasses(locationBrandId);
            return Json(carClassIds, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGlobalTetherSettings()
        {
            List<GlobalTetherSetting> lstGlobalTetherSetting = globalTetherService.GetAll(false).Select(obj => new GlobalTetherSetting { DominentBrandID = obj.DominentBrandID, LocationID = obj.LocationID }).Distinct().ToList();
            return Json(lstGlobalTetherSetting, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveJob(AutoConsoleJobEditDTO autoConsoleJobEditDTO)
        {
            return Json(scheduledJobService.SaveScheduledJob(autoConsoleJobEditDTO));
        }

        public JsonResult GetJobDetails(long JobId)
        {
            return Json(scheduledJobService.GetById(JobId, false), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllScheduledJobs(string userLocationBrandIds)
        {
            List<AutomationConsoleViewJobsDTO> automationJobs = scheduledJobService.GetAllScheduledJobs(userLocationBrandIds);
            return Json(automationJobs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PreLoadMinRates(long[] CarClassId, string locationBrandIds, long ScheduledJobId)
        {

            if (CarClassId != null && !string.IsNullOrEmpty(locationBrandIds))
            {
                List<MinRatesDTO> minRatesList = scheduledJobMinRateService.preLoadMinRates(CarClassId, locationBrandIds, ScheduledJobId);
                if (minRatesList != null)
                {
                    return Json(new { status = true, rateList = minRatesList });
                }
                else
                {
                    return Json(new { status = false });
                }
            }
            else
                return Json(new { status = false });
        }

        [HttpGet]
        public JsonResult GetSelectedMinRate(long ScheduleJobID)
        {
            return Json(scheduledJobMinRateService.GetSelectedMinRate(ScheduleJobID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StartStopScheduledJob(string jobId, string loggedInUserId, bool stop)
        {
            long JobId = 0, LoggedInUserId = 0;
            if (long.TryParse(jobId, out JobId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                ScheduledJob job = scheduledJobService.GetAll(false).Where(a => a.ID == JobId).FirstOrDefault();

                if (job != null)
                {
                    //If user has to go for start for scheduled job

                    if (!stop)
                    {
                        scheduledJobService.SetNextRunDateTime(job);
                    }
                    job.IsEnabled = !stop;
                    job.UpdatedBy = LoggedInUserId;
                    job.UpdatedDateTime = DateTime.Now;

                    if (!stop && !job.NextRunDateTime.HasValue)
                    {
                        job.IsEnabled = false;
                        job.IsStopByFTB = false;
                        scheduledJobService.Update(job);
                        return Json(new { Status = "NO_NEXT_RUN_FOUND" }, JsonRequestBehavior.AllowGet);
                    }
                    if (!stop)
                    {
                        //FTB Scheduled job scenario check on start/stop operation
                        FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
                        ftbAutomationScenarioDTO.StartDate = job.StartDate.Value;
                        ftbAutomationScenarioDTO.EndDate = job.EndDate.Value;
                        ftbAutomationScenarioDTO.IsFTBJob = false;
                        ftbAutomationScenarioDTO.IsMenualChangeJobStatus = true;
                        ftbAutomationScenarioDTO.AutomationJobID = JobId;
                        ftbAutomationScenarioDTO.LocationBrandID = Convert.ToInt64(job.LocationBrandIDs);
                        ftbAutomationScenarioDTO.LoggedUserID = LoggedInUserId;
                        ftbAutomationScenarioDTO = ftbScheduleJobService.CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);
                        if (ftbAutomationScenarioDTO.IsReturnMsg)
                        {
                            if (ftbAutomationScenarioDTO.ReturnMessage.ToLower() == "notconfiguredblackoutdates")
                            {
                                return Json(new { Status = ftbAutomationScenarioDTO.ReturnMessage }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { Status = ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString() }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        job.IsStopByFTB = false;//If user should menually stop/start job on time we should disable this flag and remove color code on the front if exist and change flag if is there or not. 
                    }
                    scheduledJobService.Update(job);
                }
                return Json(new { Status = "success", NextRunTime = job.NextRunDateTime.HasValue ? job.NextRunDateTime.Value.ToString("MM/dd/yyyy HH:mm") : "-" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteScheduledJob(string jobId, string loggedInUserId)
        {
            long JobId = 0, LoggedInUserId = 0;
            if (long.TryParse(jobId, out JobId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                ScheduledJob job = scheduledJobService.GetAll(false).Where(a => a.ID == JobId).FirstOrDefault();

                if (job != null)
                {
                    job.IsDeleted = true;
                    job.IsEnabled = false;
                    job.IsCustomRuleSet = false;
                    job.UpdatedBy = LoggedInUserId;
                    job.UpdatedDateTime = DateTime.Now;
                    scheduledJobService.Update(job);
                    //Delete tether item in scheduledJobTethering
                    scheduledJobTetheringsService.DeleteScheduleJobTetheringData(JobId);
                    ruleSetService.DeleteAutomationRuleSet(JobId);//hard Delete ruleset hierarchy
                }

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult GetJobSelectedTetherSettings(long jobId)
        {
            ScheduledJobTetheringsDTO scheduledJobTetherings = scheduledJobTetheringsService.GetSelectedJobTetheringData(jobId);
            return Json(scheduledJobTetherings, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEditJobOpaqueRates(long jobId)
        {
            IEnumerable<ScheduledJobOpaqueValuesDTO> scheduledJobOpaqueValuesDTO = scheduledJobService.getScheduledJobOpaqueRates(jobId);
            return Json(scheduledJobOpaqueValuesDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> InitiateShopForScheduledJob(SearchModelDTO searchModel)
        {
            string response = String.Empty;

            if (searchModel != null)
            {
                searchModel.VendorCodes = String.Join(",", _companyService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList());
                var provider = dataProviderService.GetAll().Where(x => x.ID == searchModel.ProviderId).FirstOrDefault();
                var scrapingServiceSource = _scrapperSourceService.GetAll().Where(x => x.ID == Int64.Parse(searchModel.ScrapperSourceIDs.Split(',')[0])).FirstOrDefault();
                searchModel.SelectedAPI = provider != null ? provider.Code : String.Empty;
                searchModel.ScrapperSource = scrapingServiceSource != null ? scrapingServiceSource.Code : String.Empty;
                searchModel.StartDate = searchModel.StartDate < DateTime.Now ? DateTime.Now : searchModel.StartDate;
                searchModel.ShopType = ShopTypes.SummaryShop;
                response = await _providerService.SendRequestForSearchedShop(searchModel);
            }

            if (response.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }

        }

        [HttpPost]
        public JsonResult GetApplicableRateCodes(string strStartDate, string strEndDate)
        {
            DateTime startDate = DateTime.ParseExact(strStartDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(strEndDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            List<RateCodeDTO> lstRateCodes = rateCodeService.GetApplicableRateCodesBetweenDateRange(startDate, endDate);
            return Json(lstRateCodes);
        }
    }
}