using RateShopper.Compression;
using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize]
    [HandleError]
    [CompressFilter]
    public class RateCodeController : Controller
    {
        IRateCodeService _rateCodeService;
        ICompanyService _companyService;
        //IRateCodeDateRangeService _rateCodeDateRangeService;

        public RateCodeController(IRateCodeService rateCodeService, ICompanyService _companyService)
        {
            this._companyService = _companyService;
            this._rateCodeService = rateCodeService;
        }

        // GET: /RateCode/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.Companies = _companyService.GetAll(false).Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            return View();
        }

        [HttpGet]
        public JsonResult GetRateCodes()
        {
            List<RateCodeDTO> rateCodes = _rateCodeService.GetAllRateCodes();
            if (rateCodes != null)
            {
                return Json(new { status = true, result = rateCodes }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveRateCode(RateCodeDTO objRateCodeDTO)
        {
            if (objRateCodeDTO != null)
            {
                objRateCodeDTO.ID = _rateCodeService.SaveRateCode(objRateCodeDTO);
            }
            return Json(objRateCodeDTO);
        }

        //[HttpPost]
        //public JsonResult SaveRateCodeDateRange(RateCodeDateRangeDTO objRateCodeDateRangeDTO)
        //{
        //    if (objRateCodeDateRangeDTO != null)
        //    {
        //        objRateCodeDateRangeDTO.ID = _rateCodeDateRangeService.SaveRateCodeDateRange(objRateCodeDateRangeDTO);
                
        //    }
        //    return Json(objRateCodeDateRangeDTO);
        //}

        [HttpPost]
        public JsonResult GetRateCode(long rateCodeId)
        {
            RateCodeDTO rateCode = _rateCodeService.GetRateCodeDetails(rateCodeId);
            if (rateCode != null)
            {
                return Json(new { status = true, result = rateCode });
            }
            else
            {
                return Json(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteRateCode(long rateCodeId, long userId)
        {
            bool isDeleted = _rateCodeService.DeleteRateCode(rateCodeId, userId);
            return Json(new { status = isDeleted });
        }

    }
}