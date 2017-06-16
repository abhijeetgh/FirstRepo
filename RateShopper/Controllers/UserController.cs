using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class UserController : Controller
    {
        //
        // GET: /User/
        ILocationBrandService locationBrandService;
        IScrapperSourceService scrapperSourceService;
        IUserService userService;
        IUserPermissionService userPermissionService;
        public UserController(ILocationBrandService locationBrandService, IScrapperSourceService scrapperSourceService, IUserService userService, IUserPermissionService userPermissionService)
        {
            this.locationBrandService = locationBrandService;
            this.scrapperSourceService = scrapperSourceService;
            this.userService = userService;
            this.userPermissionService = userPermissionService;
        }
        public ActionResult Index()
        {
            ViewBag.ScrapperSource = scrapperSourceService.GetAll().ToList();
            ViewBag.LocationBrand = locationBrandService.GetAll().Where(obj => !(obj.IsDeleted)).ToList();
            ViewBag.UserPermission = userPermissionService.GetAll().Where(obj=>!(obj.IsDeleted)).ToList();
            return View();
        }
        public JsonResult GetUserList()
        {
             //List<User> lstUser = userService.GetAll().Where(obj => !(obj.IsDeleted)).ToList();

             return Json(userService.GetUserList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult selectedUserData(long userID,long userRoleID)
        {
            return Json(userService.selectedUserData(userID,userRoleID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertUpdateUser(User user, UserDTO userDTO)
        {
            long userID = 0;
            if (user != null && user.ID != 0)
            {
                userID = userService.UpdateUser(user, userDTO);
            }
            else
            {
                userID = userService.InsertUser(user, userDTO);
            }
            return Json(userID, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUser(long UserID, long LoggedInUserId)
        {
            string Msg = string.Empty;
            int statusID = Convert.ToInt32(ConfigurationManager.AppSettings["StatusID"]);
            int lastDaysRecord = Convert.ToInt32(ConfigurationManager.AppSettings["SearchSummaryLastDays"]);

            Msg = userService.DeleteUser(UserID, statusID, lastDaysRecord, LoggedInUserId);

            return Json(Msg,JsonRequestBehavior.AllowGet);
        }
	}
}