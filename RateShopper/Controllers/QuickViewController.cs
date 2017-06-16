using System;
using System.Web.Mvc;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;
using System.Globalization;
using RateShopper.Compression;

namespace RateShopper.Controllers
{
    [Authorize]
    [HandleError]
    [CompressFilter]
    public class QuickViewController : Controller
    {
        //
        // GET: /QuickView/
        public ActionResult Index()
        {
            return View();
        }
        private IQuickViewResultsService _quickViewResultService;
        private IQuickViewService _quickViewService;
        public QuickViewController(IQuickViewResultsService quickViewResultService, IQuickViewService quickViewService)
        {
            _quickViewResultService = quickViewResultService;
            _quickViewService = quickViewService;
        }
        public JsonResult FetchQuickViewResult(long quickViewId, long searchSummaryId)
        {
            QuickViewReportDTO quickViewReport = null;
            if (quickViewId > 0 && searchSummaryId > 0)
            {
                quickViewReport = _quickViewResultService.GetQuickViewResult(quickViewId, searchSummaryId);
            }
            if (quickViewReport != null && quickViewReport.Dates != null && quickViewReport.LORs != null)
            {
                return Json(new { Status = true, reportData = quickViewReport }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetlengthDateCombination(long SearchSummaryID)
        {
            return Json(_quickViewResultService.GetlengthDateCombination(SearchSummaryID), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult IgnoreAndNext(string searchDate, long RentalLengthId, long searchSummaryId)
        {
            DateTime SearchDateParse = DateTime.ParseExact(searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            _quickViewService.SetQuickViewReview(SearchDateParse, RentalLengthId, searchSummaryId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}