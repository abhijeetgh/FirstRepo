using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using RateShopper.Data.Mapping;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RateShopper.Services;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;
using System.Globalization;
using RateShopper.Compression;
using System.Configuration;

namespace RateShopper.Controllers
{
    [Authorize]
    [HandleError]
    public class TSDAuditController : Controller
    {
        private ITSDTransactionsService _TSDTransactionService;
        private ILocationBrandService _LocationBrandService;
        private ICompanyService _companyService;
        private ISearchSummaryService _searchSummaryService;
        private ISearchResultsService _searchResultService;
        private IQuickViewService _quickViewService;
        private IUserService _userService;
        public TSDAuditController(ITSDTransactionsService TSDTransactionService, ILocationBrandService LocationBrandService, ICompanyService companyService,
            ISearchResultsService searchResultService, IQuickViewService quickViewService, ISearchSummaryService searchSummaryService, IUserService userService)
        {
            _TSDTransactionService = TSDTransactionService;
            _LocationBrandService = LocationBrandService;
            _companyService = companyService;
            _searchResultService = searchResultService;
            _quickViewService = quickViewService;
            _searchSummaryService = searchSummaryService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<JsonResult> TSDUpdate(ICollection<TSDModel> tsdModels, string UserName, string RateSystem,
            long LocationBrandID, long UserId, long SearchSummaryID, bool IsTetheredUpdate, string BrandLocation, string Location, OpaqueRatesConfiguration opaqueRatesConfiguration)
        {
            string response = string.Empty;
            if (tsdModels != null && tsdModels.Count > 0)
            {
                List<TSDModel> lstTSDModels = tsdModels.ToList();
                string[] opaqueRentalLengths = ConfigurationManager.AppSettings["OpaqueLORs"].Split(',');

                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                if (identity != null)
                {
                    //Verify whether user is having access to Update the rates to TSD
                    var isTSDAccess = identity.Claims.Where(c => c.Type == "IsTSDUpdateAccess").Select(c => c.Value).FirstOrDefault();
                    if (lstTSDModels != null && lstTSDModels.Count > 0 && isTSDAccess != null && isTSDAccess == "True")
                    {
                        if (IsTetheredUpdate)
                        {
                            LocationBrand location = _LocationBrandService.GetAll().Where(a => a.LocationBrandAlias.StartsWith(Location) && !a.IsDeleted && a.ID != LocationBrandID).FirstOrDefault();
                            if (location != null)
                            {
                                LocationBrandID = location.ID;
                            }
                        }
                        if (LocationBrandID > 0)
                        {
                            if (opaqueRatesConfiguration != null && lstTSDModels.Count > 0 && lstTSDModels[0].RentalLength.Split(',').Intersect(opaqueRentalLengths).Any())
                            {
                                await _TSDTransactionService.PushOpaqueRates(opaqueRatesConfiguration, lstTSDModels, UserName, RateSystem, LocationBrandID, UserId, SearchSummaryID, IsTetheredUpdate, BrandLocation);
                            }
                            response = await _TSDTransactionService.ProcessRateSelection(lstTSDModels, UserName, RateSystem, LocationBrandID, UserId, SearchSummaryID
                                , IsTetheredUpdate, BrandLocation);
                        }
                    }
                }
            }
            return Json(response);
        }

        [HttpGet]
        public JsonResult GetLocationBrand(long locationId, long BrandId, string rentalLength)
        {
            long result = 0;
            if (locationId > 0 && BrandId > 0 && !string.IsNullOrEmpty(rentalLength))
            {
                result = _LocationBrandService.GetLocationBrandId(locationId, BrandId, rentalLength);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public decimal GetExtraDayRate(long LocationBrandId, string rentalLength)
        {
            decimal extraDayRate = 0;
            if (LocationBrandId > 0)
            {
                extraDayRate = _LocationBrandService.FetchExtraDailyRate(LocationBrandId, rentalLength);
            }
            return extraDayRate;
        }
        [CompressFilter]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<CompanyDTO> companies = _companyService.GetAll().Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            ViewBag.Companies = companies;
            ViewBag.AuditLogs = _TSDTransactionService.GetTSDAuditLogs(0);
            return View();
        }

        /// <summary>
        /// Fetch specific audit details
        /// </summary>
        /// <param name="auditID"></param>
        /// <returns></returns>
        [CompressFilter]
        [HttpPost]
        public JsonResult GetAuditDetails(long auditID)
        {
            return Json(_TSDTransactionService.GetLogDetail(auditID));
        }
        [HttpGet]
        public ContentResult GetLastUpdateOnTSD(long searchSummaryId)
        {
            if (searchSummaryId > 0)
            {
                return Content(_TSDTransactionService.GetLastTSDUpdate(searchSummaryId));
            }
            else
                return null;
        }

        /// <summary>
        /// Get TSD audit logs
        /// </summary>
        /// <param name="brandID"></param>
        /// <returns></returns>
        [CompressFilter]
        [HttpGet]
        public JsonResult GetAuditLogs(long brandID)
        {
            //return Json(_TSDTransactionService.GetTSDAuditLogs(brandID));
            var jsonResult = Json(_TSDTransactionService.GetTSDAuditLogs(brandID), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAllTSD(TSDUpdateAll tsdList, bool isGOV, OpaqueRatesConfiguration opaqueRatesConfiguration)
        {
            if (tsdList != null && tsdList.UserId > 0)
            {
                //_userService.GetById(tsdList.UserId, false).IsTSDUpdateAccess;
                RateShopper.Domain.Entities.User user = _userService.GetById(tsdList.UserId, false);
                if (user == null || (user != null && !user.IsTSDUpdateAccess.HasValue) || (user != null && user.IsTSDUpdateAccess.HasValue && !user.IsTSDUpdateAccess.Value))
                {
                    return Json(new { status = false, message = "You do not have TSD access" });
                }
            }


            if (tsdList != null && tsdList.RentalLenghts != null && tsdList.RentalLenghts.Count > 0 && (tsdList.CarClassesFirst != null || tsdList.CarClassesSecond != null))
            {
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                string[] opaqueRentalLengths = ConfigurationManager.AppSettings["OpaqueLORs"].Split(',');
                bool sendOpaqueRates = false;
                if (!string.IsNullOrEmpty(tsdList.StartDate))
                {
                    StartDate = DateTime.ParseExact(tsdList.StartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(tsdList.EndDate))
                {
                    EndDate = DateTime.ParseExact(tsdList.EndDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }

                int summaryIdCounter = 0;
                string response = string.Empty;
                bool isTetherActive = false;
                List<TSDModel> dominentBrandtList = new List<TSDModel>();
                List<TSDModel> tetherBrandtList = new List<TSDModel>();

                //List<DateTime> StartDates = new List<DateTime>();
                DateTime[] arrivalDates = Enumerable.Range(0, 1 + EndDate.Date.Subtract(StartDate.Date).Days)
                        .Select(offset => StartDate.AddDays(offset)).ToArray();

                List<DateTime> timeStampUpdateList = new List<DateTime>();
                string dominentBrand = !string.IsNullOrEmpty(tsdList.DominentBrandName) ? tsdList.DominentBrandName : string.Empty;
                string dependentBrand = !string.IsNullOrEmpty(tsdList.DependentBrandName) ? tsdList.DependentBrandName : string.Empty;

                //List<long> carClassIds = tsdList.CarClasses.Select(car => car.ID).ToList();

                List<long> carClassIds = new List<long>();

                isTetherActive = tsdList.IsTetherActive;
                long tetherBrandId = 0;
                if (tsdList.LocationID > 0 && tsdList.TetherBrandId > 0)
                {
                    tetherBrandId = _LocationBrandService.GetLocationBrandId(tsdList.LocationID, tsdList.TetherBrandId, "D");
                }


                //fetch Global Limit details
                decimal minBaseValue = 0;
                decimal maxBaseValue = decimal.MaxValue;
                decimal tetherMinBaseValue = 0;
                decimal tetherMaxBaseValue = decimal.MaxValue;
                List<GlobalLimitDetailsDTO> globalLimitDetails = new List<GlobalLimitDetailsDTO>();
                List<GlobalLimitDetailsDTO> globalLimitDetailsTetherBrand = new List<GlobalLimitDetailsDTO>();

                globalLimitDetails = _TSDTransactionService.getGlobalLimits(tsdList.LocationBrandId, StartDate, EndDate);
                Formula objFormula = new Formula();
                if (isTetherActive && tetherBrandId > 0)
                {
                    objFormula = _TSDTransactionService.getLocationFormula(tetherBrandId);
                    globalLimitDetailsTetherBrand = _TSDTransactionService.getGlobalLimits(tetherBrandId, StartDate, EndDate);
                }
                //

                foreach (var rentalLength in tsdList.RentalLenghts)
                {
                    sendOpaqueRates = false;
                    if (opaqueRentalLengths.Contains(rentalLength.LOR) && opaqueRatesConfiguration != null)
                    {
                        sendOpaqueRates = true;
                    }
                    dominentBrandtList = new List<TSDModel>();
                    tetherBrandtList = new List<TSDModel>();
                    foreach (DateTime TSDStartDate in arrivalDates)
                    {
                        List<UpdateCars> CarClasses = new List<UpdateCars>();
                        if (tsdList.CarClassesFirst.CalendarDays.Where(day => day.ToLower() == "all").Count() > 0)
                        {
                            CarClasses = tsdList.CarClassesFirst.CarClasses.ToList();
                            carClassIds = tsdList.CarClassesFirst.CarClasses.Select(car => car.ID).ToList();
                            timeStampUpdateList = arrivalDates.ToList();
                        }
                        else
                        {
                            if (tsdList.CarClassesFirst.CarClasses != null && tsdList.CarClassesFirst.CalendarDays.Any(day => day.Equals(TSDStartDate.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase)))
                            {
                                CarClasses = tsdList.CarClassesFirst.CarClasses.ToList();
                                carClassIds = tsdList.CarClassesFirst.CarClasses.Select(car => car.ID).ToList();
                                timeStampUpdateList.Add(TSDStartDate);
                            }
                            else if (tsdList.CarClassesSecond !=null && tsdList.CarClassesSecond.CarClasses != null && tsdList.CarClassesSecond.CalendarDays.Any(day => day.Equals(TSDStartDate.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase)))
                            {
                                CarClasses = tsdList.CarClassesSecond.CarClasses.ToList();
                                carClassIds = tsdList.CarClassesSecond.CarClasses.Select(car => car.ID).ToList();
                                timeStampUpdateList.Add(TSDStartDate);
                            }
                        }

                        foreach (var car in CarClasses)
                        {
                            TSDModel tsdModel = new TSDModel();
                            summaryIdCounter = summaryIdCounter + 1;
                            tsdModel.RemoteID = Convert.ToString(tsdList.SearchSummaryId) + '-' + summaryIdCounter;
                            tsdModel.Branch = tsdList.BrandLocation;
                            tsdModel.CarClass = car.Code;
                            tsdModel.RentalLength = rentalLength.LOR;
                            tsdModel.RentalLengthIDs = Convert.ToString(rentalLength.ID);
                            tsdModel.StartDate = TSDStartDate.ToString("yyyyMMd");

                            if (globalLimitDetails != null && globalLimitDetails.Count > 0)
                            {
                                if (rentalLength.LOR.ToUpper().IndexOf("D") > -1)
                                {
                                    minBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate).
                                        Select(globaldet => globaldet.DayMin.HasValue ? globaldet.DayMin.Value : 0).FirstOrDefault();
                                    maxBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                        .Select(globaldet => globaldet.DayMax.HasValue ? globaldet.DayMax.Value : decimal.MaxValue).FirstOrDefault();
                                }
                                else if (rentalLength.LOR.ToUpper().IndexOf("W") > -1)
                                {
                                    minBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                        .Select(globaldet => globaldet.WeekMin.HasValue ? globaldet.WeekMin.Value : 0).FirstOrDefault();
                                    maxBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                        .Select(globaldet => globaldet.WeekMax.HasValue ? globaldet.WeekMax.Value : decimal.MaxValue).FirstOrDefault();
                                }
                                else if (rentalLength.LOR.ToUpper().IndexOf("M") > -1)
                                {
                                    minBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                        .Select(globaldet => globaldet.MonthlyMin.HasValue ? globaldet.MonthlyMin.Value : 0).FirstOrDefault();
                                    maxBaseValue = globalLimitDetails.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                        .Select(globaldet => globaldet.MonthlyMax.HasValue ? globaldet.MonthlyMax.Value : decimal.MaxValue).FirstOrDefault();
                                }
                            }

                            //check if not zero and not max value means no global limit set for this
                            if (minBaseValue != 0 && maxBaseValue != decimal.MaxValue)
                            {
                                if (car.BaseValue < minBaseValue || car.BaseValue > maxBaseValue)
                                {
                                    return Json(new { status = false, message = "Global Limit Violation occured for " + car.Code + " Date :" + TSDStartDate.ToString("MM/dd/yyyy") });
                                }
                            }

                            tsdModel.DailyRate = car.BaseValue;
                            //tsdModel.ExtraDayRateFactor = GetExtraDayRate(tsdList.LocationBrandId, Convert.ToString(rentalLength.LOR));

                            tsdModel.ExtraDayRateFactor = tsdList.ExtraDayRateFactor;

                            if (tsdModel.DailyRate > 0)
                            {
                                dominentBrandtList.Add(tsdModel);
                            }
                            if (isTetherActive)
                            {

                                decimal diffAmount = 0;
                                TSDModel tetherTSDModel = new TSDModel();
                                summaryIdCounter = summaryIdCounter + 1;
                                tetherTSDModel.RemoteID = Convert.ToString(tsdList.SearchSummaryId) + '-' + summaryIdCounter;
                                tetherTSDModel.Branch = tsdList.BrandLocation;
                                tetherTSDModel.CarClass = car.Code;
                                tetherTSDModel.RentalLength = rentalLength.LOR;
                                tetherTSDModel.RentalLengthIDs = Convert.ToString(rentalLength.ID);
                                tetherTSDModel.StartDate = TSDStartDate.ToString("yyyyMMd");

                                if (globalLimitDetailsTetherBrand != null && globalLimitDetailsTetherBrand.Count > 0)
                                {
                                    if (rentalLength.LOR.ToUpper().IndexOf("D") > -1)
                                    {
                                        tetherMinBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate).
                                            Select(globaldet => globaldet.DayMin.HasValue ? globaldet.DayMin.Value : 0).FirstOrDefault();
                                        tetherMaxBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                            .Select(globaldet => globaldet.DayMax.HasValue ? globaldet.DayMax.Value : decimal.MaxValue).FirstOrDefault();
                                    }
                                    else if (rentalLength.LOR.ToUpper().IndexOf("W") > -1)
                                    {
                                        tetherMinBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                            .Select(globaldet => globaldet.WeekMin.HasValue ? globaldet.WeekMin.Value : 0).FirstOrDefault();
                                        tetherMaxBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                            .Select(globaldet => globaldet.WeekMax.HasValue ? globaldet.WeekMax.Value : decimal.MaxValue).FirstOrDefault();
                                    }
                                    else if (rentalLength.LOR.ToUpper().IndexOf("M") > -1)
                                    {
                                        tetherMinBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                            .Select(globaldet => globaldet.MonthlyMin.HasValue ? globaldet.MonthlyMin.Value : 0).FirstOrDefault();
                                        tetherMaxBaseValue = globalLimitDetailsTetherBrand.Where(globalDet => globalDet.CarClassID == car.ID && globalDet.StartDate <= TSDStartDate && globalDet.EndDate >= TSDStartDate)
                                            .Select(globaldet => globaldet.MonthlyMax.HasValue ? globaldet.MonthlyMax.Value : decimal.MaxValue).FirstOrDefault();
                                    }
                                }

                                //calculate tether value 
                                bool noTetherGapForGov = Convert.ToBoolean(ConfigurationManager.AppSettings["GOVNoTetherGap"]);
                                if (car.IsDollar)
                                {
                                    diffAmount = car.TetherValue;
                                }
                                else
                                {
                                    if (!isGOV)
                                    {
                                        diffAmount = car.TotalValue * car.TetherValue / 100;
                                    }
                                    else
                                    {
                                        diffAmount = car.BaseValue * car.TetherValue / 100;
                                    }
                                }
                                //tetherTSDModel.DailyRate = car.BaseValue + diffAmount;
                                decimal totalValue = 0;
                                if (!isGOV)
                                {
                                    totalValue = car.TotalValue + diffAmount;
                                    //assign default calculated basevalue to daily rate
                                    tetherTSDModel.DailyRate = _searchResultService.CalculateSuggestedBaseRate(tetherBrandId, objFormula, totalValue, tsdList.BaseRentalLength, tsdList.BaseRentalLength.Substring(0, 1), isGOV);
                                }
                                else
                                {
                                    //if GOVNoTetherGap is true then Dominant Brand and Dependant brand rates will be same 
                                    //set noTetherGapForGov to false if tether gap setting to be used tether gap setting will be applied on Base Rate for GOV shop
                                    if (!noTetherGapForGov)
                                    {
                                        totalValue = car.BaseValue + diffAmount;
                                    }
                                    else
                                    {
                                        totalValue = car.BaseValue;
                                    }
                                    tetherTSDModel.DailyRate = totalValue;
                                }

                                //check global limits and replace values
                                if (tetherMinBaseValue != 0)
                                {
                                    //base is less than global limit
                                    if (tetherTSDModel.DailyRate < tetherMinBaseValue)
                                    {
                                        tetherTSDModel.DailyRate = tetherMinBaseValue;
                                    }
                                }
                                if (tetherMaxBaseValue != 0 && tetherMaxBaseValue != decimal.MaxValue)
                                {
                                    //base is greater than global limit
                                    if (tetherTSDModel.DailyRate > tetherMaxBaseValue)
                                    {
                                        tetherTSDModel.DailyRate = tetherMaxBaseValue;
                                    }
                                }
                                if (isGOV)
                                {
                                    tetherTSDModel.DailyRate = Math.Truncate(tetherTSDModel.DailyRate);
                                }

                                // tetherTSDModel.ExtraDayRateFactor = tsdList.ExtraDayRateFactor;
                                //tetherTSDModel.ExtraDayRateFactor = GetExtraDayRate(tetherBrandId, Convert.ToString(rentalLength.LOR));

                                tetherTSDModel.ExtraDayRateFactor = tsdList.ExtraDayRateFactor;

                                if (tetherTSDModel.DailyRate > 0)
                                {
                                    tetherBrandtList.Add(tetherTSDModel);
                                }
                            }
                        }

                    }
                    //
                    if (dominentBrand != null && dominentBrandtList.Count > 0)
                    {
                        response = await _TSDTransactionService.ProcessRateSelection(dominentBrandtList, tsdList.UserName, "Weblink", tsdList.LocationBrandId, tsdList.UserId, tsdList.SearchSummaryId, false, dominentBrand, false);
                        // Task.Run(() => _TSDTransactionService.ProcessRateSelection(dominentBrandtList, tsdList.UserName, "Weblink", tsdList.LocationBrandId, tsdList.UserId, tsdList.SearchSummaryId, false, dominentBrand, false));

                        if (sendOpaqueRates)
                        {
                            foreach (var opaquerental in opaqueRentalLengths)
                            {
                                if (dominentBrandtList.Where(d => d.RentalLength == opaquerental).Count() > 0)
                                {
                                    await _TSDTransactionService.PushOpaqueRates(opaqueRatesConfiguration, dominentBrandtList.Where(d => d.RentalLength.Equals(opaquerental)).ToList(), tsdList.UserName, "Weblink", tsdList.LocationBrandId, tsdList.UserId, tsdList.SearchSummaryId, false, dominentBrand);

                                }
                            }
                        }

                        //dominentBrandtList.Where(d => d.RentalLength == "D3");                        
                    }
                    DateTime[] timeStampUpdate = new DateTime[] { };

                    if (timeStampUpdateList != null && timeStampUpdateList.Count > 0)
                    {
                        timeStampUpdate = timeStampUpdateList.ToArray();
                        response = _TSDTransactionService.updateDateTimeStamp(timeStampUpdate, Convert.ToInt64(rentalLength.ID), tsdList.SearchSummaryId, carClassIds, tsdList.UserId);
                        //check if current shop is quickview 
                        if (tsdList.SearchSummaryId > 0)
                        {
                            SearchSummary searchShop = _searchSummaryService.GetById(tsdList.SearchSummaryId, false);
                            if (searchShop != null)
                            {
                                if (searchShop.IsQuickView.HasValue)
                                {
                                    if (searchShop.IsQuickView.Value)
                                    {
                                        QuickViewDTO quickView = _quickViewService.GetQuickViewDetails(tsdList.SearchSummaryId);
                                        if (quickView != null)
                                        {
                                            foreach (DateTime searchDate in timeStampUpdate)
                                            {
                                                _quickViewService.SetQuickViewReview(searchDate, Convert.ToInt64(rentalLength.ID), tsdList.SearchSummaryId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //tether rate update 
                    if (isTetherActive)
                    {
                        response = await _TSDTransactionService.ProcessRateSelection(tetherBrandtList, tsdList.UserName, "WSPAN", tetherBrandId, tsdList.UserId, tsdList.SearchSummaryId, true, dependentBrand, false);
                        //Task.Run(() => _TSDTransactionService.ProcessRateSelection(tetherBrandtList, tsdList.UserName, "WSPAN", tetherBrandId, tsdList.UserId, tsdList.SearchSummaryId, true, dependentBrand, false));

                        if (sendOpaqueRates)
                        {
                            foreach (var opaquerental in opaqueRentalLengths)
                            {
                                if (dominentBrandtList.Where(d => d.RentalLength == opaquerental).Count() > 0)
                                {
                                    await _TSDTransactionService.PushOpaqueRates(opaqueRatesConfiguration, tetherBrandtList.Where(d => d.RentalLength == opaquerental).ToList(), tsdList.UserName, "Weblink", tetherBrandId, tsdList.UserId, tsdList.SearchSummaryId, false, dependentBrand);

                                }
                            }
                        }
                    }

                }
                return Json(new { status = true, message = "success" });
            }
            return Json(new { status = false, message = "No Data sent for TSD update" });
        }
    }
}