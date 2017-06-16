using RateShopper.Compression;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [CompressFilter]
    [Authorize]
    [HandleError]
    public class SummaryController : Controller
    {
        private ISearchResultsService _searchResultsService;
        private IUserService _userService;
        private Services.Data.IProvidersService _dataProviderService;
        private ICompanyService _companyService;
        private Providers.Interface.IProviderService _providerService;
        private ISearchSummaryService _searchSummaryService;
        private ISearchResultsService _searchResultService;
        private IRentalLengthService _rentalLengthService;

        public SummaryController(ISearchResultsService searchResultsService, IUserService userService, Services.Data.IProvidersService dataProviderService, ICompanyService companyService, Providers.Interface.IProviderService providerService,
            ISearchSummaryService searchSummaryService, ISearchResultsService searchResultService, IRentalLengthService rentalLengthService)
        {
            _searchResultsService = searchResultsService;
            _userService = userService;
            _dataProviderService = dataProviderService;
            _companyService = companyService;
            _providerService = providerService;
            _searchSummaryService = searchSummaryService;
            _searchResultService = searchResultService;
            _rentalLengthService = rentalLengthService;
        }


        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            ViewBag.ScrapperSources = _searchResultsService.GetScrapperSource(Convert.ToInt64(UserID)).Where(d => d.ProviderCode == ProviderConstants.ScrappingService).OrderBy(d => d.Name);
            ViewBag.RentalLengths = _rentalLengthService.GetAll();
            ViewBag.Providers = _dataProviderService.GetAllProviders();
            if (long.TryParse(UserID, out userId))
            {
                ViewBag.LocationBrands = _userService.GetBrandLocationsByLoggedInUserId(userId).ToList();
            }

            List<ScheduleJobUserDTO> user = _userService.GetAll(true).Where(a => !(a.IsDeleted)).Select(a => new ScheduleJobUserDTO { Id = a.ID, FirstName = a.FirstName, LastName = a.LastName }).OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
            if (user != null)
            {
                ViewBag.Users = user;
            }
            return View();
        }

        public JsonResult GetCarClasses()
        {
            return Json(_searchResultsService.GetCarClass(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocations(long UserId)
        {
            return Json(_searchResultsService.GetBrandLocation(UserId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocationCarClasses(long locationBrandId)
        {
            List<long> carClassIds = _searchResultsService.GetLocationCarClasses(locationBrandId);
            return Json(carClassIds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> InitiateSummarySearch(SearchModelDTO searchModel)
        {
            searchModel.VendorCodes = String.Join(",", _companyService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList());
            string response = await _providerService.SendRequestForSearchedShop(searchModel);
            if (response.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }

        public JsonResult DeleteSummaryShop(long searchSummaryId, long userId)
        {
            SearchSummary searchSummary = new SearchSummary();
            searchSummary = _searchSummaryService.GetById(searchSummaryId);
            if (searchSummary != null)
            {
                searchSummary.StatusID = 6;
                searchSummary.UpdatedDateTime = DateTime.Now;
                searchSummary.UpdatedBy = userId;
                _searchSummaryService.Update(searchSummary);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> FTBGetSearchSummary(string objLastModifieddate, string strClientTimezoneOffset, long LoggedInUserID, string userSystemDate, bool isAdminUser, long selectedUserId)
        {
            int statusID = Convert.ToInt32(ConfigurationManager.AppSettings["StatusID"]);
            int lastDaysRecord = Convert.ToInt32(ConfigurationManager.AppSettings["FTBSearchSummaryLastDays"]);
            int clientTimezoneOffset = 0;
            SearchSummaryResult searchSummaryResults = new SearchSummaryResult();
            if (!string.IsNullOrEmpty(strClientTimezoneOffset))
            {
                clientTimezoneOffset = int.Parse(strClientTimezoneOffset.ToString());
            }
            DateTime UserSystemDate = DateTime.Now;
            if (!string.IsNullOrEmpty(userSystemDate))
            {
                UserSystemDate = Convert.ToDateTime(userSystemDate);
            }
            int callProcedure = Convert.ToInt32(ConfigurationManager.AppSettings["CallProcedure"]);
            if (callProcedure < 1)
                searchSummaryResults = await _searchSummaryService.GetFTBSearchSummaryData(objLastModifieddate, clientTimezoneOffset, LoggedInUserID, statusID, lastDaysRecord, UserSystemDate, isAdminUser, selectedUserId);
            else
                searchSummaryResults = await _searchSummaryService.GetFTBSearchSummaryDataFromSP(objLastModifieddate, clientTimezoneOffset, LoggedInUserID, statusID, lastDaysRecord, UserSystemDate, isAdminUser, selectedUserId);
            return Json(searchSummaryResults);
        }

        public async Task<ActionResult> GetFTBSearchGridDailyViewDataDefault(long LoggedInUserID, bool isAdmin)
        {
            SearchResultDTO searchResult = await _searchResultsService.GetFTBSummaryReportDefault(LoggedInUserID, isAdmin);
            if (searchResult != null)
            {
                var jsonResult = Json(
                      searchResult,
                      JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return new EmptyResult();
        }

        public async Task<ActionResult> GetFTBSummaryData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID)
        {
            SearchResultDTO searchResult = await _searchResultService.GetFTBSummaryReport(searchSummaryID, scrapperSourceID, locationBrandID, locationID, brandID, rentallengthID);
            if (searchResult != null)
            {
                var jsonResult = Json(searchResult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return new EmptyResult();
        }

    }
}