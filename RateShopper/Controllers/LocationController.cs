using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RateShopper.Controllers
{
    [Authorize]
    [HandleError]
    public class LocationController : Controller
    {
        ILocationService _locationService;
        ILocationBrandRentalLengthService _locationBrandRentalLengthService;
        ILocationBrandCarClassService _locationBrandCarClassService;
        ILocationBrandService _locationBrandService;
        ICompanyService _companyService;
        IRentalLengthService _rentalLengthService;
        ICarClassService _carClassService;

        public LocationController(ILocationService _locationService, ILocationBrandRentalLengthService _locationBrandRentalLengthService,
            ILocationBrandCarClassService _locationBrandCarClassService, ILocationBrandService _locationBrandService, ICompanyService companyService, IRentalLengthService _rentalLengthService,
            ICarClassService _carClassService)
        {
            this._locationService = _locationService;
            this._locationBrandCarClassService = _locationBrandCarClassService;
            this._locationBrandRentalLengthService = _locationBrandRentalLengthService;
            this._locationBrandService = _locationBrandService;
            this._companyService = companyService;
            this._rentalLengthService = _rentalLengthService;
            this._carClassService = _carClassService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.Companies = _companyService.GetAll(false).Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            ViewBag.RentalLengths = _rentalLengthService.GetRentalLength(false).OrderBy(x => x.DisplayOrder);
            ViewBag.CarClasses = _carClassService.GetAll(false).Where(o => !o.IsDeleted)
                .Select(obj => new CarClassDTO { ID = obj.ID, Code = obj.Code, CarClassOrder = obj.DisplayOrder }).OrderBy(car => car.CarClassOrder).ToList();

            return View();
        }

        /// <summary>
        /// Save brand location details
        /// </summary>
        /// <param name="objLocationDTO"></param>
        /// <param name="isCarClassesModified"></param>
        /// <param name="isRentalLengthsModified"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveLocation(LocationDTO objLocationDTO, bool isCarClassesModified, bool isRentalLengthsModified)
        {
            if (objLocationDTO != null)
            {
                objLocationDTO.ID = _locationService.SaveLocation(objLocationDTO);
                if (objLocationDTO.ID > 0)
                {
                    objLocationDTO.LocationBrandID = _locationBrandService.SaveLocationBrand(objLocationDTO);
                    if (objLocationDTO.LocationBrandID > 0)
                    {
                        if (isCarClassesModified && objLocationDTO.CarClasses.Count > 0)
                        {
                            _locationBrandCarClassService.SaveLocationBrandCarClasses(objLocationDTO.CarClasses, objLocationDTO.LocationBrandID);
                        }
                        if (isRentalLengthsModified)
                        {
                            _locationBrandRentalLengthService.SaveLocationBrandRentalLengths(objLocationDTO);
                        }
                    }
                }
            }
            return Json(objLocationDTO);
        }

        /// <summary>
        /// Get all brand locations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetLocations()
        {
            return Json(_locationService.GetLocations(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete brand location and if no further brand exists then delete location
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="locationBrandID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteLocation(long locationID, long locationBrandID, long userID)
        {
            bool isBrandDeleted = _locationBrandService.DeleteLocationBrand(locationBrandID, userID);
            if (isBrandDeleted)
            {
                _locationService.DeleteLocation(locationID, userID);
            }
            return Json(isBrandDeleted);
        }

        /// <summary>
        /// Get location using location id
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="locationBrandID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetLocation(long locationID, long locationBrandID)
        {
            return Json(_locationService.GetLocation(locationID, locationBrandID));
        }
        public JsonResult GetCompanies()
        {
            List<CompanyMasterDTO> companiesList = new List<CompanyMasterDTO>();
            companiesList = _companyService.GetAll().Where(company => !company.IsDeleted).Select(company => new CompanyMasterDTO { ID = company.ID, Name = company.Name, Code = company.Code, IsBrand = company.IsBrand }).OrderBy(company => company.Name).ToList();
            if (companiesList != null && companiesList.Count > 0)
            {
                return Json(new { status = true, companies = companiesList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCompetitors(long locationBrandId)
        {
            string directCompetitorIds = string.Empty;
            if (locationBrandId > 0)
            {
                directCompetitorIds = _locationBrandService.GetCompetitors(locationBrandId);
            }
            if (!string.IsNullOrEmpty(directCompetitorIds))
            {
                return Json(new { status = true, directCompetitors = directCompetitorIds }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}