using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.Risk.UI.Web.ViewModels.Admin;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class AdminHelpers
    {
        internal static UserViewModel GetUserProfileViewModel(UserDto userDto)
        {
            UserViewModel userviewModel = new UserViewModel();
            userviewModel.Id = userDto.Id;
            userviewModel.FirstName = userDto.FirstName;
            userviewModel.LastName = userDto.LastName;
            userviewModel.UserName = userDto.UserName;
            userviewModel.Email = userDto.Email;
            userviewModel.UserRoleId = userDto.UserRoleID;
            userviewModel.UserRole = userDto.UserRole;
            userviewModel.IsActive = userDto.IsActive;
            return userviewModel;
        }
        internal static UserDto GetUserProfileDto(UserViewModel userViewModel)
        {
            UserDto userDto = new UserDto();
            userDto.Id = userViewModel.Id;
            userDto.FirstName = userViewModel.FirstName;
            userDto.LastName = userViewModel.LastName;
            userDto.UserName = userViewModel.UserName != null ? userViewModel.UserName.Trim() : userViewModel.UserName;
            userDto.Email = userViewModel.Email;
            userDto.Password = userViewModel.Password;
            userDto.UserRoleID = userViewModel.UserRoleId;
            userDto.UserRole = userViewModel.UserRole;
            userDto.IsActive = userViewModel.IsActive;
            userDto.CurrentUserId = SecurityHelper.GetUserIdFromContext();
            userDto.LocationIds = userViewModel.AssignedLocationsId != null ? userViewModel.AssignedLocationsId.ToList() :  new List<long>();

            return userDto;
        }
        internal static CompanyViewModel GetCompanyViewModel(CompanyDto companyDto)
        {
            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel.Id = companyDto.Id;
            companyViewModel.Abbr = companyDto.Abbr;
            companyViewModel.Name = companyDto.Name;
            companyViewModel.Address = companyDto.Address;
            companyViewModel.City = companyDto.City;
            companyViewModel.State = companyDto.State;
            companyViewModel.Zip = companyDto.Zip;
            companyViewModel.Phone = companyDto.Phone;
            companyViewModel.Fax = companyDto.Fax;
            companyViewModel.Website = companyDto.Website;
            //companyViewModel.Zurich = companyDto.Zurich;
            return companyViewModel;
        }
        internal static CompanyDto GetCompanyDto(CompanyViewModel companyViewModel)
        {
            CompanyDto companyDto = new CompanyDto();
            companyDto.Id = companyViewModel.Id;
            companyDto.Abbr = companyViewModel.Abbr;
            companyDto.Name = companyViewModel.Name;
            companyDto.Address = companyViewModel.Address;
            companyDto.City = companyViewModel.City;
            companyDto.State = companyViewModel.State;
            companyDto.Zip = companyViewModel.Zip;
            companyDto.Phone = companyViewModel.Phone;
            companyDto.Fax = companyViewModel.Fax;
            companyDto.Website = companyViewModel.Website;
            //companyDto.Zurich = companyViewModel.Zurich;
            companyDto.CurrentUserId = SecurityHelper.GetUserIdFromContext();
            return companyDto;
        }
    }
}