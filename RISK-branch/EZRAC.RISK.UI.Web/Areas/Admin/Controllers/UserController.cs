using EZRAC.Core.Caching;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
//using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class UserController : Controller
    {
        #region  Private Members
        private IUserService _userService;
        private ILookUpService _lookupService;
        #endregion
        //
        // GET: /Admin/User/
        #region Constructors
        public UserController(IUserService userService, ILookUpService lookupService)
        {
            _userService = userService;
            _lookupService = lookupService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {

            IEnumerable<ViewModels.Admin.UserViewModel> userviewModel = await GetAllUsers();

            var userRoleList = await _userService.GetUserRoles();

            ViewBag.UserRoleList = LookUpHelpers.GetUserRoleItems(userRoleList, 0).ToList();

            var locationDto = await _lookupService.GetAllLocationsAsync();
            ViewBag.Locations = GetAssignedLocationViewModel(locationDto);
            //var selectedLocationDto = _userService.GetUserLocationsByUserId();
            return View(userviewModel);
        }


        public async Task<ActionResult> GetUserbyIdAsync(long userId)
        {
            ViewModels.Admin.UserViewModel userviewModel = new ViewModels.Admin.UserViewModel();
            var result = await _userService.GetUserbyIdAsync(userId);
            var userRoleList = await _userService.GetUserRoles();

            ViewBag.UserRoleList = LookUpHelpers.GetUserRoleItems(userRoleList, userId).ToList();
            userviewModel = AdminHelpers.GetUserProfileViewModel(result);
            userviewModel.AssignedLocations = await GetAssignedLocationViewModel(userId);
            //return RedirectToAction("Index");
            return PartialView("_UserEditProfile", userviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUpdateUser(ViewModels.Admin.UserViewModel userViewModel)
        {

            UserDto userDto = new UserDto();
            userDto = AdminHelpers.GetUserProfileDto(userViewModel);

            await _userService.AddUpdatedUserAsync(userDto);

            RemoveCache();

            IEnumerable<ViewModels.Admin.UserViewModel> userviewModel = await GetAllUsers();

            return PartialView("_UserGridList", userviewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSelectedUser(long userId)
        {
            await _userService.DeleteUser(userId);

            RemoveCache();

            IEnumerable<ViewModels.Admin.UserViewModel> userviewModel = await GetAllUsers();

            return PartialView("_UserGridList", userviewModel);

        }

        public async Task<ActionResult> IsUserNameExist(string username)
        {
            bool flag = await _userService.IsUserNameExist(username);
            return Json(flag.ToString(), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> IsUserMapped(long userId)
        {
            bool flag = await _userService.IsUserMapped(userId);
            return Json(flag.ToString(), JsonRequestBehavior.AllowGet);
        }

        #region Private methods
        private async Task<IEnumerable<ViewModels.Admin.UserViewModel>> GetAllUsers()
        {
            IEnumerable<ViewModels.Admin.UserViewModel> userviewModel = null;
            var result = await _userService.GetAllUsersAsync();
            if (result.Any())
            {
                userviewModel = MappedUserViewModel(result);
            }
            return userviewModel;
        }

        private static IEnumerable<ViewModels.Admin.UserViewModel> MappedUserViewModel(IEnumerable<UserDto> userlist)
        {
            IEnumerable<ViewModels.Admin.UserViewModel> userviewModel = null;
            userviewModel = userlist.Select(x => new ViewModels.Admin.UserViewModel
                        {
                            Id = x.Id,
                            UserName = x.UserName,
                            Password = x.Password,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            UserRoleId = x.UserRoleID,
                            UserRole = x.UserRole,
                            IsActive = x.IsActive
                        }).ToList<ViewModels.Admin.UserViewModel>();
            return userviewModel;
        }

        private async Task<IEnumerable<AssignedLocationViewModel>> GetAssignedLocationViewModel(long userId)
        {
            IEnumerable<LocationDto> locationDto = await _lookupService.GetAllLocationsAsync();
            IEnumerable<LocationDto> selectedLocationDto = await _userService.GetUserLocationsByUserId(userId);

            var viewModelList = new List<AssignedLocationViewModel>();
            AssignedLocationViewModel viewModel = null;

            foreach (var item in locationDto)
            {
                viewModel = new AssignedLocationViewModel();
                viewModel.LocationId = item.Id;
                viewModel.LocationCode = item.Code;
                viewModel.IsLocationAssigned = selectedLocationDto.Any(x => x.Id == item.Id);
                viewModel.CompanyAbbreviation = item.CompanyAbbr;
                viewModelList.Add(viewModel);
            }
            return viewModelList.OrderBy(x => x.LocationCode);
        }

        private static IEnumerable<AssignedLocationViewModel> GetAssignedLocationViewModel(IEnumerable<LocationDto> locationDto)
        {
            var viewModelList = new List<AssignedLocationViewModel>();
            AssignedLocationViewModel viewModel = null;

            foreach (var item in locationDto)
            {
                viewModel = new AssignedLocationViewModel();
                viewModel.LocationId = item.Id;
                viewModel.LocationCode = item.Code;
                viewModel.IsLocationAssigned = false;
                viewModel.CompanyAbbreviation = item.CompanyAbbr;
                viewModelList.Add(viewModel);
            }
            return viewModelList.OrderBy(x => x.LocationCode);
        }

        private static void RemoveCache()
        {
            Cache.Remove(Constants.CacheConstants.AllUsersEmails);
            Cache.Remove(Constants.CacheConstants.Users);
            Cache.Remove(Constants.CacheConstants.AssignedUsers);
        }
        #endregion
    }
}