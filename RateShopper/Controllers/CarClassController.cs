using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class CarClassController : Controller
    {
        ICarClassService _carClassService;
        string _carClassLogoInitialPath = "Media/CarClasses/";
        string _defaultLogoPath = "images/default_logo.png";

        public CarClassController(ICarClassService carClassService)
        {
            _carClassService = carClassService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get all car classes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllCarClasses()
        {
            return Json(_carClassService.GetAllCarClasses(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCarClass(long carClassID)
        {
            CarClassDTO objCarClassDTO = _carClassService.GetCarClassDetails(carClassID);
            return Json(objCarClassDTO);
        }

        [HttpPost]
        public JsonResult SaveCarClass(CarClassDTO objCarClassDTO)
        {
            bool setDefaultLogo = false;
            if (objCarClassDTO != null)
            {
                long carClassID = objCarClassDTO.ID;
                if (objCarClassDTO.ID > 0 && !string.IsNullOrEmpty(objCarClassDTO.Logo))
                {
                    objCarClassDTO.Logo = _carClassLogoInitialPath + objCarClassDTO.ID.ToString() + "/" + objCarClassDTO.Logo;
                }
                else if (carClassID == 0 && string.IsNullOrEmpty(objCarClassDTO.Logo))
                {
                    objCarClassDTO.Logo = _defaultLogoPath;
                    setDefaultLogo = true;
                }

                //Save car class details
                objCarClassDTO.ID = _carClassService.SaveCarClass(objCarClassDTO);

                if (!setDefaultLogo && carClassID == 0 && objCarClassDTO.ID > 0 && !string.IsNullOrEmpty(objCarClassDTO.Logo))
                {
                    string oldPath = Path.Combine(Server.MapPath("~/" + _carClassLogoInitialPath + "temp/"), objCarClassDTO.Logo);
                    string newPath = Path.Combine(Server.MapPath("~/" + _carClassLogoInitialPath + objCarClassDTO.ID.ToString() + "/"));

                    if (System.IO.File.Exists(oldPath))
                    {
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        newPath = newPath + objCarClassDTO.Logo;
                        System.IO.File.Copy(oldPath, newPath, true);
                        System.IO.File.Delete(oldPath);
                        newPath = _carClassLogoInitialPath + objCarClassDTO.ID.ToString() + "/" + objCarClassDTO.Logo;
                        _carClassService.SaveCarClassLogo(objCarClassDTO.ID, newPath);
                    }
                }
                //return Json(objCarClassDTO);
                return Json(new { status = true, objCarClass = objCarClassDTO });
            }
            return null;
        }

        [HttpPost]
        public string Upload()
        {
            string carClassID = Request.Form["CarClassID"];
            var file = Request.Files[0];
            var imagePath = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var path = string.Empty;
            string oldImage = Request.Form["OldImageName"];

            if (carClassID == "0")
            {
                path = Server.MapPath("~/" + _carClassLogoInitialPath + "temp/");
                imagePath = _carClassLogoInitialPath + "temp/" + fileName;
            }
            else
            {
                path = Server.MapPath("~/" + _carClassLogoInitialPath + carClassID + "/");
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                {
                    string[] filestringarray = fileName.Split('.');
                    if (filestringarray.Length > 1)
                    {
                        filestringarray[filestringarray.Length - 2] = filestringarray[filestringarray.Length - 2] + System.Guid.NewGuid().ToString();
                        fileName = string.Join(".", filestringarray);
                    }
                }
                imagePath = _carClassLogoInitialPath + carClassID + "/" + fileName;
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
        public JsonResult DeleteCarClass(long carClassID, long userID)
        {
            bool result = _carClassService.DeleteCarClass(carClassID, userID);
            return Json(result);
        }

        [HttpGet]
        public void DeleteUploadedImage(string oldImage, long carClassID)
        {
            if (!string.IsNullOrEmpty(oldImage))
            {
                string path = string.Empty;
                if (carClassID > 0)
                {
                    path = Server.MapPath("~/" + _carClassLogoInitialPath + carClassID + "/");
                }
                else
                {
                    path = Server.MapPath("~/" + _carClassLogoInitialPath + "temp/");
                }

                var oldImagePath = Path.Combine(path, oldImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
        }
        [HttpGet]
        public JsonResult CheckCarClassOrder(string CarClassOrder,string carClassCode)
        {
            string carClassName = _carClassService.CarClassOrderCount(Convert.ToInt16(CarClassOrder),carClassCode);
            if (!string.IsNullOrEmpty(carClassName))
            {
                return Json(new { status = true, carclass = carClassName }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
    }
}