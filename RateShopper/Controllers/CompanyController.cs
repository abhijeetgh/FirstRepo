using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System.IO;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class CompanyController : Controller
    {
        ILocationCompanyService _locationCompanyService;
        ICompanyService _companyService;
        string _companyLogoInitialPath = "Media/Companies/";
        string _defaultLogoPath = "images/default_logo.png";

        public CompanyController(ILocationCompanyService locationCompanyService, ICompanyService companyService)
        {
            _locationCompanyService = locationCompanyService;
            _companyService = companyService;
        }

        public ActionResult Index()
        {
            ViewBag.CompanyLocations = _locationCompanyService.GetLocations();
            return View();
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllCompanies()
        {
            return Json(_companyService.GetAllCompanies(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCompany(long companyID)
        {
            CompanyDTO objCompanyDTO = _companyService.GetCompanyDetails(companyID);
            if (objCompanyDTO != null && objCompanyDTO.ID > 0)
            {
                objCompanyDTO.lstLocations = _locationCompanyService.GetCompanyLocations(objCompanyDTO.ID);
            }
            return Json(objCompanyDTO);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification="Method overloading is not allowed in controller with same name and same http type"), HttpPost]
        public JsonResult SaveCompany(CompanyDTO objCompanyDTO, bool isCompanyLocationsModified = false)
        {
            bool setDefaultLogo = false;
            if (objCompanyDTO != null)
            {
                long companyID = objCompanyDTO.ID;                
                if (objCompanyDTO.ID > 0 && !string.IsNullOrEmpty(objCompanyDTO.Logo))
                {
                    //comment the following code which delete the previous logo
                    //if (!string.IsNullOrEmpty(oldImage) && System.Convert.ToString(oldImage).ToUpper() != System.Convert.ToString(objCompanyDTO.Logo).ToUpper())
                    //{
                    //    string oldImagePath = Path.Combine(Server.MapPath("~/" + _companyLogoInitialPath + companyID + "/"), oldImage);
                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}
                    objCompanyDTO.Logo = _companyLogoInitialPath + objCompanyDTO.ID.ToString() + "/" + objCompanyDTO.Logo;
                }
                else if (companyID == 0 && string.IsNullOrEmpty(objCompanyDTO.Logo))
                {
                    objCompanyDTO.Logo = _defaultLogoPath;
                    setDefaultLogo = true;                    
                }

                //Save company details
                objCompanyDTO.ID = _companyService.SaveCompany(objCompanyDTO);
                if (objCompanyDTO.ID > 0 && isCompanyLocationsModified)
                {
                    //save company locations
                    _locationCompanyService.SaveCompanyLocations(objCompanyDTO.lstLocations, objCompanyDTO.ID);
                }

                if (!setDefaultLogo && companyID == 0 && objCompanyDTO.ID > 0 && !string.IsNullOrEmpty(objCompanyDTO.Logo))
                {
                    string oldPath = Path.Combine(Server.MapPath("~/" + _companyLogoInitialPath + "temp/"), objCompanyDTO.Logo);
                    string newPath = Path.Combine(Server.MapPath("~/" + _companyLogoInitialPath + objCompanyDTO.ID.ToString() + "/"));
                    
                    if (System.IO.File.Exists(oldPath))
                    {
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        newPath = newPath + objCompanyDTO.Logo;
                        System.IO.File.Copy(oldPath, newPath, true);
                        System.IO.File.Delete(oldPath);
                        newPath = _companyLogoInitialPath + objCompanyDTO.ID.ToString() + "/" + objCompanyDTO.Logo;
                        _companyService.SaveCompanyLogo(objCompanyDTO.ID, newPath);
                    }
                }                
                return Json(objCompanyDTO);
            }
            return null;
        }

        [HttpPost]
        public string Upload()
        {
            string companyID = Request.Form["CompanyID"];
            var file = Request.Files[0];
            var imagePath = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var path = string.Empty;
            string oldImage = Request.Form["OldImageName"];
            
            if (companyID == "0")
            {
                path = Server.MapPath("~/" + _companyLogoInitialPath + "temp/");
                imagePath = _companyLogoInitialPath + "temp/" + fileName;                
            }
            else
            {
                path = Server.MapPath("~/" + _companyLogoInitialPath + companyID + "/");
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                {
                    string[] filestringarray =  fileName.Split('.');
                    if (filestringarray.Length > 1)
                    {
                        filestringarray[filestringarray.Length - 2] = filestringarray[filestringarray.Length - 2] + System.Guid.NewGuid().ToString();
                        fileName = string.Join(".", filestringarray);
                    }
                }
                imagePath = _companyLogoInitialPath + companyID + "/" + fileName;                
            }
            if (!string.IsNullOrEmpty(oldImage))
            {
                var oldImagePath = Path.Combine(path, oldImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, fileName);
            file.SaveAs(path);

            return imagePath;
        }

        [HttpPost]
        public JsonResult DeleteCompany(long companyID, long userID)
        {
            bool result = _companyService.DeleteCompany(companyID, userID);
            return Json(result);
        }

        [HttpGet]
        public void DeleteUploadedImage(string oldImage, long companyID)
        {
            if (!string.IsNullOrEmpty(oldImage))
            {
                string path = string.Empty;
                if (companyID > 0)
                {
                    path = Server.MapPath("~/" + _companyLogoInitialPath + companyID + "/");
                }
                else
                {
                    path = Server.MapPath("~/" + _companyLogoInitialPath + "temp/");
                }


                var oldImagePath = Path.Combine(path, oldImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
        }
    }
}
