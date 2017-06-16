using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using System.Threading.Tasks;
using RateShopper.Services.Helper;
using RateShopper.Domain.DTOs;
using RateShopper.Helper;
using System.Data;
using System.Configuration;
using System.Security.Claims;
using RateShopper.Compression;

namespace RateShopper.Controllers
{
    [CompressFilter]
    [Authorize]
    [HandleError]
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        private IGlobalTetherSettingService _globalTetherService;
        private IFormulaService _formulaService;

        private ICompanyService _companyService;


        ISearchResultsService _searchResultsService;
        IRentalLengthService _rentalLengthService;
        //public SearchController(IGlobalTetherSettingService _globalTetherService, IFormulaService _formulaService,
        //    ILocationBrandService locationBrandService, ICarClassService carClassService, IRentalLengthService rentalLengthService, ISearchResultsService searchResultsService, ISearchSummaryService searchSummaryService)
        private IUserService _userService;
        private IStatusesService _statusesService;
        private ISearchSummaryService _searchSummaryService;
        private ILocationBrandRentalLengthService _locationBrandRentalLengthService;
        private ILocationBrandService _locationBrandService;
        private RateShopper.Providers.Interface.IProviderService _providerService;
        private Services.Data.IProvidersService _dataProviderService;
        private IQuickViewService _quickViewService;
        private ISearchJobUpdateAllService _searchJobUpdateAllService;
        private IRateCodeService _rateCodeService;

        public SearchController(IGlobalTetherSettingService globalTetherService, IFormulaService formulaService, IUserService userService,
           IStatusesService statusesService, ISearchSummaryService searchSummaryService, ISearchResultsService searchResultsService,
            ICompanyService companyService, ILocationBrandRentalLengthService locationBrandRentalLengthService, ILocationBrandService locationBrandService,
             Services.Data.IProvidersService dataProviderService, RateShopper.Providers.Interface.IProviderService providerService, IQuickViewService quickViewService, ISearchJobUpdateAllService searchJobUpdateAllService, IRateCodeService rateCodeService, IRentalLengthService rentalLengthService)
        {
            this._globalTetherService = globalTetherService;
            this._formulaService = formulaService;
            this._userService = userService;
            this._statusesService = statusesService;
            this._searchSummaryService = searchSummaryService;
            this._searchResultsService = searchResultsService;
            this._companyService = companyService;
            this._locationBrandRentalLengthService = locationBrandRentalLengthService;
            this._locationBrandService = locationBrandService;
            this._providerService = providerService;
            this._dataProviderService = dataProviderService;
            this._quickViewService = quickViewService;
            this._searchJobUpdateAllService = searchJobUpdateAllService;
            this._rateCodeService = rateCodeService;
            this._rentalLengthService = rentalLengthService;
        }
        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            ViewBag.ScrapperSources = _searchResultsService.GetScrapperSource(Convert.ToInt64(UserID)).OrderBy(d => d.Name);
            ViewBag.RentalLengths = _rentalLengthService.GetAll();
            ViewBag.Providers = _dataProviderService.GetAllProviders();
            ViewBag.RateCodes = _rateCodeService.GetAllRateCodes();
            if (long.TryParse(UserID, out userId))
            {
                ViewBag.LocationBrands = _userService.GetBrandLocationsByLoggedInUserId(userId).ToList();
            }
            //List<User> user = _userService.GetAll(true).Where(a => !(a.IsDeleted)).ToList();
            List<ScheduleJobUserDTO> user = _userService.GetAll(true).Where(a => !(a.IsDeleted)).Select(a => new ScheduleJobUserDTO { Id = a.ID, FirstName = a.FirstName, LastName = a.LastName }).OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
            if (user != null)
            {
                ViewBag.Users = user;
            }
            return View();
        }
        public ActionResult GetFormula(long locationID)
        {

            FormulaDTO oFormula = new FormulaDTO();
            oFormula = _formulaService.GetFormulaByLocation(locationID);
            //oFormula = _formulaService.GetById(Convert.ToInt64(locationID), true, "");

            return Json(oFormula, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFormulaTether(long locationID, long DependantBrandID)
        {

            FormulaDTO oFormula = new FormulaDTO();
            oFormula = _formulaService.GetTetherFormula(locationID, DependantBrandID);
            //oFormula = _formulaService.GetById(Convert.ToInt64(locationID), true, "");

            return Json(oFormula, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScrapperSources(long UserId)
        {
            return Json(_searchResultsService.GetScrapperSource(UserId), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetScrapperSourcess(long userId)
        {
            //return Json(_searchResultsService.GetScrapperSource(userId, providerId), JsonRequestBehavior.AllowGet);
            return Json(_searchResultsService.GetScrapperSource(userId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocations(long UserId)
        {
            return Json(_searchResultsService.GetBrandLocation(UserId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCarClasses()
        {
            return Json(_searchResultsService.GetCarClass(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRentalLengths()
        {
            return Json(_searchResultsService.GetRentalLength(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationCarClasses(long locationBrandId)
        {
            List<long> carClassIds = _searchResultsService.GetLocationCarClasses(locationBrandId);
            return Json(carClassIds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> InitiateSearch(SearchModelDTO searchModel)
        {
            //string response = await _searchSummaryService.SaveSearchSummary(searchModel);
            //Following lines are commented temporary as api dev is paused.
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
        //public ActionResult GetSearchSummaryUserList()
        //{
        //    List<User> user = new List<User>();
        //    user=_userService.GetAll(true).Where(a => !(a.IsDeleted)).ToList();
        //    if (user != null)
        //    {
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        return Json(serializer.Serialize(user), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }


        //}
        public ActionResult GetStatus()
        {
            List<Statuses> lstStatuses = new List<Statuses>();
            foreach (var item in _statusesService.GetAll(false).ToList())
            {
                Statuses status = new Statuses();
                if (item.ID == 1 || item.ID == 2 || item.ID == 3)
                {
                    status.ID = 1;
                    status.Status = "In Progress";
                    lstStatuses.Add(status);
                }
                else
                {
                    status.ID = item.ID;
                    status.Status = item.Status;
                    lstStatuses.Add(status);
                }
            }
            return Json(lstStatuses.GroupBy(obj => obj.ID).Select(grp => grp.First()).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGlobalTetherSettings()
        {
            List<GlobalTetherSetting> lstGlobalTetherSetting = _globalTetherService.GetAll().ToList();
            return Json(lstGlobalTetherSetting, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGlobalTetherSettingsLocationSpecific(long locationId)
        {
            List<GlobalTetherSetting> lstGlobalTetherSetting = _globalTetherService.GetGlobalTetherSettingsLocationSpecific(locationId);
            return Json(lstGlobalTetherSetting, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetSearchSummary(string objLastModifieddate, string strClientTimezoneOffset, long LoggedInUserID, string userSystemDate, bool isAdminUser, long selectedUserId)
        {
            int statusID = Convert.ToInt32(ConfigurationManager.AppSettings["StatusID"]);
            int lastDaysRecord = Convert.ToInt32(ConfigurationManager.AppSettings["SearchSummaryLastDays"]);
            int clientTimezoneOffset = 0;
            SearchSummaryResult searchSummaryResults = new SearchSummaryResult();
            if (!string.IsNullOrEmpty(strClientTimezoneOffset))
            {
                clientTimezoneOffset = int.Parse(strClientTimezoneOffset.ToString());
            }
            //if (!string.IsNullOrEmpty(currentDateTime))
            //{
            //    CurrentDateTimeServer = _quickViewService.GetESTDate(currentDateTime).Date;
            //}
            DateTime UserSystemDate = DateTime.Now;
            if (!string.IsNullOrEmpty(userSystemDate))
            {
                UserSystemDate = Convert.ToDateTime(userSystemDate);
            }
            int callProcedure = Convert.ToInt32(ConfigurationManager.AppSettings["CallProcedure"]);
            if (callProcedure < 1)
                searchSummaryResults = await _searchSummaryService.GetSearchSummaryData(objLastModifieddate, clientTimezoneOffset, LoggedInUserID, statusID, lastDaysRecord, UserSystemDate, isAdminUser, selectedUserId);
            else
                searchSummaryResults = await _searchSummaryService.GetSearchSummaryDataFromSP(objLastModifieddate, clientTimezoneOffset, LoggedInUserID, statusID, lastDaysRecord, UserSystemDate, isAdminUser, selectedUserId);
            return Json(searchSummaryResults);
        }
        [HttpGet]
        public ActionResult DeleteSelectedSummary(long id)
        {
            SearchSummary searchSummary = new SearchSummary();
            searchSummary = _searchSummaryService.GetById(id);
            searchSummary.StatusID = 6;
            searchSummary.UpdatedDateTime = DateTime.Now;
            _searchSummaryService.Update(searchSummary);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return latest search result on page load
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetSearchGridDailyViewDataDefault(long LoggedInUserID, bool isAdminUser)
        {
            SearchResultDTO searchResult = await _searchResultsService.GetSearchGridDailyViewDataDefault(LoggedInUserID, isAdminUser);
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

        public async Task<ActionResult> GetSearchGridDailyViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string arrivalDate)
        {
            SearchResultDTO searchResult = await _searchResultsService.GetSearchGridDailyViewData(searchSummaryID, scrapperSourceID, locationBrandID, locationID, brandID, rentallengthID, arrivalDate);
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

        public async Task<ActionResult> GetSearchGridClassicViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string carClassId)
        {
            SearchResultDTO searchResult = await _searchResultsService.GetSearchGridClassicViewData(searchSummaryID, scrapperSourceID, locationBrandID, locationID, brandID, rentallengthID, carClassId);
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

        //Get Last udpated date for TSD based on Filter selection in search screen
        public ActionResult GetLastUpdateOnTSD(long searchSummaryID, long scrapperSourceID, long locationBrandID, long locationID, long brandID, long rentallengthID, long carClassId, string arrivalDate, string view)
        {
            return Content(_searchResultsService.GetLastUpdateOnTSD(searchSummaryID, scrapperSourceID, locationBrandID, locationID, brandID, rentallengthID, carClassId, arrivalDate, view));
        }

        public ActionResult SearchViewAppliedRuleSet(long ID)
        {
            return Json(_searchResultsService.SearchViewAppliedRuleSet(ID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompanyBrands()
        {
            //IQueryable<Company> queryableCompanies = _companyService.GetAll();
            List<CompanyMasterDTO> Companies = new List<CompanyMasterDTO>();
            Companies = _companyService.GetAll(false).Where(a => a.IsBrand && !(a.IsDeleted)).Select(obj => new CompanyMasterDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code, Logo = obj.Logo, IsBrand = obj.IsBrand, IsDeleted = obj.IsDeleted, CreatedBy = obj.CreatedBy, UpdatedBy = obj.UpdatedBy }).ToList();
            return Json(Companies, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLOR(long locationBrandID, string locationBrandName)
        {
            bool? useLorRateCode = null;
            var locationBrandRentalLengths = _locationBrandRentalLengthService.GetAll().Where(obj => obj.LocationBrandID == locationBrandID);
            var locationBrand = _locationBrandService.GetById(locationBrandID, false);
            string rateSystem = string.Empty;
            if (!string.IsNullOrEmpty(locationBrandName))
            {
                if (locationBrandName.Split('-').Length > 0)
                {
                    if (locationBrandName.Split('-')[1].ToUpper() == "EZ")
                    {
                        rateSystem = Convert.ToString(ConfigurationManager.AppSettings["EZRateSystem"]);
                    }
                    else if (locationBrandName.Split('-')[1].ToUpper() == "AD")
                    {
                        rateSystem = Convert.ToString(ConfigurationManager.AppSettings["ADRateSystem"]);
                    }
                }
            }
            if (locationBrand != null)
            {
                useLorRateCode = locationBrand.UseLORRateCode;
            }
            //List<RentalLength> rentalLengths = _rentalLengthService.GetAll().ToList();
            IEnumerable<RentalLength> rentalLengths = _rentalLengthService.GetRentalLength();
            var rentalLength = (from tblLocationBrandRentalLengths in locationBrandRentalLengths
                                          join tblRentalLength in rentalLengths on tblLocationBrandRentalLengths.RentalLengthID equals tblRentalLength.MappedID
                                          orderby tblRentalLength.DisplayOrder
                                          select new RentalLengthDTO { MappedID = tblRentalLength.MappedID, Code = tblRentalLength.Code });
            
            if (locationBrandRentalLengths != null && locationBrandRentalLengths.Count() > 0)
            {
                //var x = query.ToArray();
                return Json(new { lors = rentalLength, useLorRtCode = useLorRateCode.Value, rateSystemName = rateSystem }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public FileResult GetSearchShopCSV(long searchSummaryID, string shopDate)
        {
            Dictionary<string, string> searchShopSummary = _searchSummaryService.GetSearchShopSummaryForCSV(searchSummaryID);
            if (searchShopSummary.Count > 0 && !string.IsNullOrEmpty(shopDate))
            {
                using (System.Data.DataTable dt = new System.Data.DataTable("ShopDetails"))
                {
                    dt.Columns.Add("Label");
                    dt.Columns.Add("Value");

                    dt.Rows.Add("Time of:", shopDate);
                    dt.Rows.Add("Source:", searchShopSummary["Sources"]);
                    dt.Rows.Add("Location:", searchShopSummary["LocationBrand"]);
                    dt.Rows.Add("Start Date:", searchShopSummary["Startdate"]);
                    dt.Rows.Add("End Date:", searchShopSummary["Enddate"]);
                    dt.Rows.Add("Car Classes:", searchShopSummary["CarClasses"]);
                    dt.Rows.Add("Lengths:", searchShopSummary["Lengths"]);

                    DataTable search = _searchResultsService.GetSearchShopDetailsForCSV(searchSummaryID);
                    search.Columns["Suggested_Total_Rate"].ColumnName = "Suggested/Updated Total Rate";
                    search.Columns["Suggested_Base_Rate"].ColumnName = "Suggested/Updated Base Rate";
                    search.Columns.Remove("isGOV");
                    search.Columns.Remove("LocationBrandId");

                    if (!search.AsEnumerable().Any(row => "W8" == Convert.ToString(row["Length"]).ToUpper() || "W9" == Convert.ToString(row["Length"]).ToUpper() || "W10" == Convert.ToString(row["Length"]).ToUpper() || "W11" == Convert.ToString(row["Length"]).ToUpper()))
                    {
                        search.Columns.Remove("Additional_Base");
                    }

                    using (DataSet ds = new System.Data.DataSet())
                    {
                        ds.Tables.Add(dt);
                        ds.Tables.Add(search);

                        return new CsvActionResult(ds, searchShopSummary["Startdate"] + " to " + searchShopSummary["Enddate"] + "_" + searchShopSummary["LocationBrand"] + "_" + shopDate.Replace(":", "_") + ".csv", true);
                    }
                }
            }
            return null;
        }

        public JsonResult GetGlobalLimitTetherShop(long SearchSummaryID, long DependantBrandID, long LocationID)
        {
            List<GlobalLimitDetailsDTO> globalllimitdto = _searchSummaryService.GetGlobalLimitTetherShop(SearchSummaryID, DependantBrandID, LocationID);
            return Json(globalllimitdto, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuickViewCompetitors(long locationBrandId, long searchSummaryId)
        {
            string competitorsIds = string.Empty;
            string carClassIds = string.Empty;
            QuickViewDTO objQuickViewDTO = null;
            //List<QuickViewGroup> lstQuickViewGroup = new List<QuickViewGroup>();
            if (searchSummaryId > 0)
            {
                objQuickViewDTO = _quickViewService.GetQuickViewDetails(searchSummaryId);
                if (objQuickViewDTO == null)
                {
                    competitorsIds = _locationBrandService.GetQuickViewCompetitorsIds(locationBrandId);
                }
                else
                {
                    competitorsIds = objQuickViewDTO.CompetitorCompanyIds;
                    carClassIds = objQuickViewDTO.CarClassIds;
                }
            }
            else
            {
                competitorsIds = _locationBrandService.GetQuickViewCompetitorsIds(locationBrandId);
            }
            List<CompanyDTO> allCompanies = _companyService.GetAllCompanies().OrderBy(company => company.Name).ToList();
            IEnumerable<long> selectedCompetitors = Common.StringToLongList(competitorsIds);
            allCompanies.ForEach(d => d.SelectedForQuickView = selectedCompetitors.Contains(d.ID));
            var jsonResult = Json(new { status = true, selectedCompetitors = competitorsIds, companies = allCompanies, quickviewobject = objQuickViewDTO, quickviewgroupobject = (objQuickViewDTO != null) ? objQuickViewDTO.lstQuickViewGroupItem.ToList() : null, selectedCarClasses = carClassIds }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public async Task<JsonResult> SaveQuickViewSchedule(long searchSummaryId, string scheduleTime, string quickViewCompetitors, string userId, string clientDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData)
        {
            if (searchSummaryId > 0 && !string.IsNullOrEmpty(scheduleTime) && !string.IsNullOrEmpty(quickViewCompetitors) && !string.IsNullOrEmpty(carClassIds))
            {
                QuickViewDTO objQuickViewDTO = _quickViewService.SaveQuickView(searchSummaryId, scheduleTime, quickViewCompetitors, Convert.ToInt64(userId), clientDate, scheduleControlId, carClassIds, quickViewGroupData);
                if (objQuickViewDTO != null && objQuickViewDTO.ID > 0)
                {
                    //Update HasQuickView flag of current shop
                    _searchSummaryService.UpdateQuickViewStatus(searchSummaryId, Convert.ToInt64(userId));
                    //if run now option is selected
                    if (scheduleTime == "0")
                    {
                        string response = await _searchSummaryService.InitiateQuickViewSearch(searchSummaryId, Convert.ToInt64(userId));
                    }
                    return Json(new { status = true, quickViewId = objQuickViewDTO.ID });
                }
            }
            return Json(new { status = false });
        }


        public JsonResult GetQuickViewGridData(string locationBrandIds, long userId, string clientDate)
        {
            if (!string.IsNullOrEmpty(locationBrandIds) && userId > -1 && !string.IsNullOrEmpty(clientDate))
            {
                var jsonresult = Json(new { status = true, gridData = _quickViewService.GetQuickViews(userId, locationBrandIds, clientDate) }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            return Json(new { status = false });
        }

        [HttpPost]
        public async Task<JsonResult> SaveQuickView(long quickViewId, string userId, string scheduleTime, string quickViewCompetitors, string clientDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData)
        {
            if (quickViewId > 0 && !string.IsNullOrEmpty(scheduleTime))
            {
                QuickViewGridDTO objQuickViewGridDTO = _quickViewService.UpdateQuickView(quickViewId, scheduleTime, quickViewCompetitors, Convert.ToInt64(userId), clientDate, scheduleControlId, carClassIds, quickViewGroupData);
                if (objQuickViewGridDTO != null)
                {
                    if (objQuickViewGridDTO.ChildSummaryId.HasValue)
                    {
                        //Update HasQuickView flag of current shop
                        _searchSummaryService.UpdateQuickViewStatus(objQuickViewGridDTO.ChildSummaryId.Value, Convert.ToInt64(userId));
                    }
                    if (scheduleTime == "0")
                    {
                        string response = await _searchSummaryService.InitiateQuickViewSearch(objQuickViewGridDTO.SearchSummaryId, Convert.ToInt64(userId));
                    }
                    return Json(new { status = true, quickViewRow = objQuickViewGridDTO });
                }
            }
            return Json(new { status = false });
        }

        [HttpPost]
        public JsonResult DeleteQuickView(long quickViewId, long userId, string clientDate)
        {
            bool status = _quickViewService.DeleteQuickView(quickViewId, userId, clientDate);
            return Json(new { status = status });
        }

        [HttpGet]
        public JsonResult GetPreloaBaseRate(long SearchSummaryId, bool isDailyPreLoad)
        {
            List<SearchJobUpdateAll> searchJobUpdateAll = _searchJobUpdateAllService.GetPreloaBaseRate(SearchSummaryId, isDailyPreLoad);
            return Json(searchJobUpdateAll, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertUpdateTSDUpdateAll(long SearchSummaryId, ICollection<SearchJobUpdateAllDTO> searchJobUpdateAllDTO)
        {
            List<SearchJobUpdateAllDTO> lstSearchJobUpdateAllDTO = searchJobUpdateAllDTO.ToList();
            string result = _searchJobUpdateAllService.InsertUpdateTSDUpdateAll(SearchSummaryId, lstSearchJobUpdateAllDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetApplicableRateCodes(string strStartDate, string strEndDate)
        {
            DateTime startDate = DateTime.ParseExact(strStartDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(strEndDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            List<RateCodeDTO> lstRateCodes = _rateCodeService.GetApplicableRateCodesBetweenDateRange(startDate, endDate);
            return Json(lstRateCodes);
        }


    }
}