using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using RateShopper.Domain.Entities;
using RateShopper.Data.Mapping;
using System.Threading.Tasks;
using RateShopper.Services.Helper;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using RateShopper.Domain.DTOs;
using RateShopper.Helper;
using System.Text;
using System.Net.Http.Headers;
using System.Data;
using System.Configuration;
using System.Security.Claims;
using System.Globalization;
using RateShopper.Providers.Interface;
using RateShopper.Compression;

namespace RateShopper.Controllers
{
    [CompressFilter]
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class FTBRatesController : Controller
    {
        private IUserService _userService;
        private ISearchResultsService _searchResultsService;
        private IFTBTargetService ftbTargetService;
        private IWeekDayService weekDayService;
        private IFTBRatesService _ftbRatesService;
        private IRentalLengthService _rentalLengthService;
        
        public FTBRatesController(IUserService userService, ISearchResultsService searchResultsService, IWeekDayService weekDayService, IFTBTargetService ftbTargetService, IFTBRatesService ftbRatesService, IRentalLengthService rentalLengthService)
        {
            _userService=userService;
            _searchResultsService = searchResultsService;
            this.weekDayService = weekDayService;
            this.ftbTargetService = ftbTargetService;
            _ftbRatesService = ftbRatesService;
            _rentalLengthService = rentalLengthService;
        }


        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            if (long.TryParse(UserID, out userId))
            {
                ViewBag.LocationBrands = _userService.GetFTBBrandLocationsByLoggedInUserId(userId).ToList();
            }
            ViewBag.RentalLengths = _rentalLengthService.GetAll().Where(d => d.Code != "M30");
            ViewBag.Years = _ftbRatesService.GetYears();
            ViewBag.ServerDate = DateTime.Now.ToString("MM-dd-yyyy");
            return View();
        }

        [HttpPost]
        public ActionResult SaveTargetDetails(ICollection<FTBTargetDTO> ftbTargetDTO)
        {
            bool flag = false;
            if (ftbTargetDTO != null && ftbTargetDTO.Count > 0)
            {
                List<FTBTargetDTO> lstFTBTargetDTO = ftbTargetDTO.ToList();

                if (lstFTBTargetDTO[0].IsUpdate)
                {
                    flag = ftbTargetService.UpdateTargetDetails(lstFTBTargetDTO);
                }
                else
                {
                    flag = ftbTargetService.AddTargetDetails(lstFTBTargetDTO);
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> FetchTargetDetails(long locationBrandId, long year, long month, bool isCopyFrom)
        {
            List<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();
            if (locationBrandId != 0 && year != 0 && month != 0)
            {
                FTBTargetDTOs = await ftbTargetService.GetTargetDetails(locationBrandId, year, month, isCopyFrom);
                if (FTBTargetDTOs.Count() == 0)
                {
                    FTBTargetDTOs = getPreparedTemplate().ToList();
                }
            }
            return Json(FTBTargetDTOs, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTargetTemplate()
        {
            List<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();
            FTBTargetDTOs = getPreparedTemplate().ToList();
            return Json(FTBTargetDTOs, JsonRequestBehavior.AllowGet);
        }

        public ICollection<FTBTargetDTO> getPreparedTemplate()
        {

            //List<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();
            ICollection<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();
            //var result =Enumerable.Range(1, 5).Select(a => new FTBTargetsDetailDTO { SlotOrder = a }).ToList();

            FTBTargetDTOs = weekDayService.GetAll().Select(wd => new FTBTargetDTO
            {
                DayOfWeekId = wd.ID,
                Day = wd.Day,
                Target = null,
                IsUpdate = false,
                FTBTargetsDetailDTOs = Enumerable.Range(1, 5).Select(a => new FTBTargetsDetailDTO { SlotOrder = a }).ToList()
            }).OrderBy(x => x.DayOfWeekId).ToList();

            return FTBTargetDTOs;
        }

        [HttpGet]
        public ActionResult GetFTBRate(long brandLocationID, long rentalLengthId, int year, int month, int selectedFTBRateId)
        {
            return Json(_ftbRatesService.GetFTBRates(brandLocationID, rentalLengthId, year, month, selectedFTBRateId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFTBRates(FTBRatesDTO objFTBRatesDTO)
        {
            if (objFTBRatesDTO != null)
            {
                return Json(_ftbRatesService.SaveFTBRates(objFTBRatesDTO));
            }
            return Json("");
        }

        [HttpPost]
        public async Task<JsonResult> CopyFTBRates(FTBCopyMonthsDTO objFTBCopyMonthsDTO)
        {
            int response = await _ftbRatesService.CopyFTBRates(objFTBCopyMonthsDTO, false);
            if (response > 0)
            {
                return Json(new { status = 1, Rates = _ftbRatesService.GetFTBRates(objFTBCopyMonthsDTO.LocationBrandId, 0, objFTBCopyMonthsDTO.Year, objFTBCopyMonthsDTO.Month, 0) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 0 });
            }
        }
    }
}