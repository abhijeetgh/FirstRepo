using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RateShopper.Services.Data;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using RateShopper.Compression;
using System.Security.Claims;
using RateShopper.Services.Helper;
using System.Threading.Tasks;


namespace RateShopper.Controllers
{
    [CompressFilter]
    [Authorize]
    [HandleError]
    public class RezCentralController : Controller
    {
        private IUserService userService;
        private IRateCodeService rateCodeService;

        private ILocationBrandCarClassService locationBrandCarClassService;
        private IWeekDayService weekDayService;
        private ICarClassService carClass;
        private ITSDTransactionsService tSDTransactionService;
        //
        // GET: /RezCentral/

        public RezCentralController(IUserService userService, ILocationBrandCarClassService locationBrandCarClassService, IWeekDayService weekDayService, ICarClassService carClass, ITSDTransactionsService tSDTransactionService, IRateCodeService rateCodeService)        
        {
            this.userService = userService;
            this.rateCodeService = rateCodeService;
            this.locationBrandCarClassService = locationBrandCarClassService;
            this.weekDayService = weekDayService;
            this.carClass = carClass;
            this.tSDTransactionService = tSDTransactionService;
        }
        public ActionResult Index()
        {
            long userId = 0;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string UserID = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();
            if (long.TryParse(UserID, out userId))
            {
                ViewBag.RateCodes = rateCodeService.GetAllRateCodes().Where(d => d.IsActive);
                ViewBag.WeekDays = weekDayService.GetAll().ToList();
                ViewBag.CarClasses = carClass.GetAllCarClasses();
            }
            return View();
        }

        public JsonResult GetLocationCarClasses(string locationBrandId)
        {
            List<long> locationBrandIds = Common.StringToLongList(locationBrandId).ToList();
            string carClassIds = locationBrandCarClassService.GetLocationCarClassesData(locationBrandIds);
            return Json(carClassIds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> PushRates(RezCentralDTO objRezCentralDTO)
        {
            if (objRezCentralDTO != null)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                var isTSDAccess = identity.Claims.Where(c => c.Type == "IsTSDUpdateAccess").Select(c => c.Value).FirstOrDefault();
                if (isTSDAccess != null && isTSDAccess == "True")
                {
                    string response = await tSDTransactionService.PushRezCentralRates(objRezCentralDTO);
                    if (!string.IsNullOrEmpty(response))
                    {
                        return Json(new { IsPushSuccess = true, message = response });
                    }                    
                }
            }
            return Json(new { IsPushSuccess = false });
        }

        [HttpPost]
        public async Task<JsonResult> PullRates(RezPreloadPayloadDTO objPayload)
        {
            IEnumerable<RezPreloadRateDTO> tsdRateList= await tSDTransactionService.GetRatesFromPreloadAPI(objPayload);
            return Json(tsdRateList,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocations(long userId)
        {
            return Json(userService.GetBrandLocationsByLoggedInUserId(userId).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}