using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using System.Threading.Tasks;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;
using System.Security.Claims;
using RateShopper.Compression;

namespace RateShopper.Controllers
{
    [HandleError]
    [Authorize]
    [CompressFilter]
    public class FTBAutomationController : Controller
    {
        private IUserService _userService;
        private IJobTypeMapperService _jobTypeMapperService;
        private IWeekDayService _weekDayService;
        private IFTBScheduleJobService _ftbScheduleJobService;
        private Services.Data.IProvidersService _dataProviderService;
        private ICompanyService _companyService;
        private Providers.Interface.IProviderService _providerService;
        private ISearchResultsService _searchResultsService;
        private ISearchSummaryService _searchSummaryService;

        public FTBAutomationController(IUserService userService, IJobTypeMapperService jobTypeMapperService, IWeekDayService weekDayService, IFTBScheduleJobService ftbScheduleJobService, Services.Data.IProvidersService dataProviderService, ICompanyService companyService,
            Providers.Interface.IProviderService providerService, ISearchResultsService searchResultsService, ISearchSummaryService searchSummaryService)
        {
            this._userService = userService;
            this._jobTypeMapperService = jobTypeMapperService;
            this._weekDayService = weekDayService;
            this._ftbScheduleJobService = ftbScheduleJobService;
            this._dataProviderService = dataProviderService;
            this._companyService = companyService;
            this._providerService = providerService;
            this._searchResultsService = searchResultsService;
            this._searchSummaryService = searchSummaryService;
        }

        //
        // GET: /FTBAutomation/
        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            if (long.TryParse(UserID, out userId))
            {
                bool isFTPAutomation = Convert.ToBoolean(identity.Claims.Where(c => c.Type == "ftbautomation").Select(c => c.Value).FirstOrDefault());

                if (!isFTPAutomation)
                {
                    return RedirectToAction("RemoveAuthoriseUserData", "Account");
                }

                ViewBag.Frequencies = _jobTypeMapperService.GetJobFrequencyTypes("FTB Automation").OrderBy(freq => freq.ID);
                ViewBag.WeekDays = _weekDayService.GetAll(false);
                ViewBag.NextMonths = GetNextMonths();
                ViewBag.ScrapperSources = _searchResultsService.GetScrapperSource(Convert.ToInt64(UserID)).Where(d => d.ProviderCode == ProviderConstants.ScrappingService).OrderBy(d => d.Name);
                ViewBag.LocationBrands = _userService.GetFTBBrandLocationsByLoggedInUserId(userId).ToList();
                ViewBag.Users = _userService.GetAll(false).Where(a => !a.IsDeleted).Select(a => new ScheduleJobUserDTO { Id = a.ID, FirstName = a.FirstName, LastName = a.LastName }).OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
            }
            return View();
        }

        public async Task<JsonResult> GetLocations(long UserId)
        {
            return Json(await _userService.GetBrandLocationWithTether(UserId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveJob(FTBJobEditDTO ftbJobEditDTO)
        {
            return Json(_ftbScheduleJobService.SaveScheduledJob(ftbJobEditDTO));
        }

        public static ICollection<FTBMonthsDTO> GetNextMonths()
        {
            List<FTBMonthsDTO> ftbMonths = new List<FTBMonthsDTO>();
            DateTime nextMonth;

            for (int i = 1; i < 12; i++)
            {
                nextMonth = DateTime.Now.AddMonths(i);
                ftbMonths.Add(new FTBMonthsDTO { Year = nextMonth.Year, Month = nextMonth.Month, MonthYear = nextMonth.ToString("MMM") + "-" + nextMonth.Year });
            }

            return ftbMonths;
        }

        public JsonResult GetFTBJob(long jobId)
        {
            if (jobId > 0)
            {
                return Json(new { status = true, result = _ftbScheduleJobService.GetById(jobId, false) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, message = "No Job found" }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetScheduledJobMonths(long locationBrandId)
        {
            if (locationBrandId > 0)
            {
                return Json(new { status = true, result = await _ftbScheduleJobService.GetLocationBrandJobMonths(locationBrandId) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, message = "No Jobs found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetFTBAutomationJobList(long loggedUserId, long locationBrandId,bool isAdminUser)
        {
            IEnumerable<FTBAutomationJobsDTO> FTBAutomationJobsDTO = await _ftbScheduleJobService.GetFTBAutomationJobList(loggedUserId, locationBrandId,isAdminUser);
            return Json(FTBAutomationJobsDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StartStopFTBScheduledJob(string jobId, string loggedInUserId, bool stop)
        {
            long JobId = 0, LoggedInUserId = 0;
            if (long.TryParse(jobId, out JobId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                FTBScheduledJob job = _ftbScheduleJobService.GetAll(false).Where(a => a.ID == JobId).FirstOrDefault();

                if (job != null)
                {
                    if (!stop)
                    {
                        _ftbScheduleJobService.SetNextRunDateTime(job);
                    }
                    job.IsEnabled = !stop;
                    job.UpdatedBy = LoggedInUserId;
                    job.UpdatedDateTime = DateTime.Now;

                    if (!stop && !job.NextRunDateTime.HasValue)
                    {
                        job.IsEnabled = false;
                        _ftbScheduleJobService.Update(job);
                        return Json("NO_NEXT_RUN_FOUND", JsonRequestBehavior.AllowGet);
                    }
                    if (!stop)
                    {
                        //FTB Scheduled job scenario check on start/stop operation
                        FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
                        ftbAutomationScenarioDTO.StartDate = job.StartDate;
                        ftbAutomationScenarioDTO.IsFTBJob = true;
                        ftbAutomationScenarioDTO.LocationBrandID = job.LocationBrandID;
                        ftbAutomationScenarioDTO.LoggedUserID = LoggedInUserId;
                        ftbAutomationScenarioDTO = _ftbScheduleJobService.CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);
                        if (ftbAutomationScenarioDTO.IsReturnMsg)
                        {
                            _ftbScheduleJobService.Update(job);
                            if (ftbAutomationScenarioDTO.ReturnMessage == "notconfiguredblackoutdates")
                            {
                                return Json("Successnotconfiguredblackoutdates", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("Success" + ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString(), JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    _ftbScheduleJobService.Update(job);
                }
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteFTBScheduledJob(string jobId, string loggedInUserId)
        {
            long JobId = 0, LoggedInUserId = 0;
            if (long.TryParse(jobId, out JobId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                FTBScheduledJob job = _ftbScheduleJobService.GetAll(false).Where(a => a.ID == JobId).FirstOrDefault();

                if (job != null)
                {
                    job.IsDeleted = true;
                    job.IsEnabled = false;
                    job.UpdatedBy = LoggedInUserId;
                    job.UpdatedDateTime = DateTime.Now;
                    _ftbScheduleJobService.Update(job);
                }

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> InitiateSummaryShop(SearchModelDTO searchModel)
        {
            string response = String.Empty;
            if (searchModel != null)
            {
                searchModel.VendorCodes = String.Join(",", _companyService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList());
                if (string.IsNullOrEmpty(searchModel.SelectedAPI))
                {
                    var provider = _dataProviderService.GetAll().Where(x => x.ID == searchModel.ProviderId).FirstOrDefault();
                    searchModel.SelectedAPI = provider != null ? provider.Code : String.Empty;
                }
                //searchModel.StartDate = 
                searchModel.ShopType = ShopTypes.SummaryShop;

                response = await _providerService.SendRequestForSearchedShop(searchModel);
            }
            if (response.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {                
                return Json(await _ftbScheduleJobService.GetSearchDetails(searchModel.FTBScheduledJobID), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed");
            }

        }

        public async Task<JsonResult> DeleteSummaryShop(long searchSummaryId, long loggedUserId, long locationBrandId, long jobId)
        {
            SearchSummary searchSummary = new SearchSummary();
            searchSummary = _searchSummaryService.GetById(searchSummaryId);
            if (searchSummary != null)
            {
                searchSummary.StatusID = 6;
                searchSummary.UpdatedDateTime = DateTime.Now;
                searchSummary.UpdatedBy = loggedUserId;
                _searchSummaryService.Update(searchSummary);
            }            
            return Json(await _ftbScheduleJobService.GetSearchDetails(jobId), JsonRequestBehavior.AllowGet);
        }
    }

}