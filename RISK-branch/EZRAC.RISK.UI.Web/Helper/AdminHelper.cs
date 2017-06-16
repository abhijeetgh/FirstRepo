using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class AdminHelper
    {

        internal static IEnumerable<ClaimStatusViewModel> GetClaimListViewModel(IEnumerable<ClaimStatusDto> claimStatusList)
        {
            var viewModelList = new List<ClaimStatusViewModel>();
            ClaimStatusViewModel viewModel = null;

            foreach (var item in claimStatusList)
            {
                viewModel = new ClaimStatusViewModel();
                viewModel.Id = item.Id;
                viewModel.ClaimStatus = item.Description;
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }

        internal static ClaimStatusViewModel GetClaimViewModel(ClaimStatusDto claimStatus)
        {
            ClaimStatusViewModel viewModel = viewModel = new ClaimStatusViewModel();
            viewModel.Id = claimStatus.Id;
            viewModel.ClaimStatus = claimStatus.Description;
            return viewModel;
        }

        internal static IEnumerable<LossTypeViewModel> GetClaimListViewModel(IEnumerable<LossTypesDto> lossTypeList)
        {
            var viewModelList = new List<LossTypeViewModel>();
            LossTypeViewModel viewModel = null;

            foreach (var item in lossTypeList)
            {
                viewModel = new LossTypeViewModel();
                viewModel.Id = item.Id;
                viewModel.LossType = item.Type;
                viewModel.Description = item.Description;
                viewModel.Status = item.Status;
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }

        internal static LossTypeViewModel GetClaimViewModel(LossTypesDto lossType)
        {
            LossTypeViewModel viewModel = viewModel = new LossTypeViewModel();
            viewModel.Id = lossType.Id;
            viewModel.LossType = lossType.Type;
            viewModel.Description = lossType.Description;
            viewModel.Status = lossType.Status;
            return viewModel;
        }

        internal static IEnumerable<VehicleSectionViewModel> GetVehicleSectionListVideModel(IEnumerable<DamageTypesDto> damageTypeList)
        {
            var viewModelList = new List<VehicleSectionViewModel>();
            VehicleSectionViewModel viewModel = null;

            foreach (var item in damageTypeList)
            {
                viewModel = new VehicleSectionViewModel();
                viewModel.Id = item.Id;
                viewModel.Section = item.Section;
                viewModelList.Add(viewModel);
            }
            return viewModelList;
        }

        internal static VehicleSectionViewModel GetVehicleSectionViewModel(DamageTypesDto damage)
        {
            VehicleSectionViewModel viewModel = new VehicleSectionViewModel();
            viewModel.Id = damage.Id;
            viewModel.Section = damage.Section;
            return viewModel;
        }

        internal static IEnumerable<InsuranceCompanyViewModel> GetInsuranceCompanyListVideModel(IEnumerable<InsuranceDto> insuranceCompanyList)
        {
            var viewModelList = new List<InsuranceCompanyViewModel>();
            InsuranceCompanyViewModel viewModel = null;

            foreach (var item in insuranceCompanyList)
            {
                viewModel = new InsuranceCompanyViewModel();
                viewModel.Id = item.Id;
                viewModel.Address = item.Address;
                viewModel.City = item.City;
                viewModel.CompanyName = item.CompanyName;
                viewModel.CompanyNotes = item.Notes;
                viewModel.Email = item.Email;
                viewModel.Fax = item.Fax;
                viewModel.PhoneNumber = item.Phone;
                viewModel.State = item.State;
                viewModel.Zip = item.Zip;
                viewModel.Contact = item.Contact;
                viewModelList.Add(viewModel);

            }
            return viewModelList;
        }

        internal static InsuranceCompanyViewModel GetInsuranceCompanyViewModel(InsuranceDto insuranceCompany)
        {
            InsuranceCompanyViewModel viewModel = new InsuranceCompanyViewModel();
            viewModel.Id = insuranceCompany.Id;
            viewModel.Address = insuranceCompany.Address;
            viewModel.City = insuranceCompany.City;
            viewModel.CompanyName = insuranceCompany.CompanyName;
            viewModel.CompanyNotes = insuranceCompany.Notes;
            viewModel.Email = insuranceCompany.Email;
            viewModel.Fax = insuranceCompany.Fax;
            viewModel.PhoneNumber = insuranceCompany.Phone;
            viewModel.State = insuranceCompany.State;
            viewModel.Zip = insuranceCompany.Zip;
            viewModel.Contact = insuranceCompany.Contact;
            return viewModel;
        }

        internal static InsuranceDto GetInsuranceDto(InsuranceCompanyViewModel viewModel)
        {
            var insuranceDto = new InsuranceDto();
            insuranceDto.Id = (int)viewModel.Id;
            insuranceDto.Address = viewModel.Address;
            insuranceDto.City = viewModel.City;
            insuranceDto.CompanyName = viewModel.CompanyName;
            insuranceDto.Notes = viewModel.CompanyNotes;
            insuranceDto.Email = viewModel.Email;
            insuranceDto.Fax = viewModel.Fax;
            insuranceDto.Phone = viewModel.PhoneNumber;
            insuranceDto.State = viewModel.State;
            insuranceDto.Zip = viewModel.Zip;
            insuranceDto.Contact = viewModel.Contact;
            return insuranceDto;
        }


        internal static LocationMasterViewModel GetLocationMasterViewModel(IEnumerable<LocationDto> locations)
        {
            LocationMasterViewModel locationModel = new LocationMasterViewModel();

            if (locations != null && locations.Any())
            {
                locationModel.Locations = locations.Select(x =>
                    new LocationViewModel
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Name,
                        State = x.State,
                        CompanyAbbr = x.CompanyAbbr != null ? x.CompanyAbbr.ToUpper() : String.Empty
                    }).ToList();

            }

            locationModel.LocationViewModel = new LocationViewModel();
            locationModel.LocationViewModel.Companies = LookUpHelpers.GetCompanyListItem();

            return locationModel;
        }

        internal static IEnumerable<PoliceAgencyViewModel> GetPoliceAgencyListVideModel(IEnumerable<PoliceAgencyDto> policeAgencyList)
        {
            var viewModelList = new List<PoliceAgencyViewModel>();
            PoliceAgencyViewModel viewModel = null;

            foreach (var item in policeAgencyList)
            {
                viewModel = new PoliceAgencyViewModel();
                viewModel.Id = item.Id;
                viewModel.Address = item.Address;
                viewModel.City = item.City;
                viewModel.AgencyName = item.AgencyName;
                viewModel.Email = item.Email;
                viewModel.Fax = item.Fax;
                viewModel.Phone = item.Phone;
                viewModel.State = item.State;
                viewModel.Zip = item.ZIP;
                viewModel.Contact = item.Contact;
                viewModelList.Add(viewModel);

            }
            return viewModelList;
        }

        internal static PoliceAgencyViewModel GetPoliceAgencyViewModel(PoliceAgencyDto policeAgency)
        {
            PoliceAgencyViewModel viewModel = new PoliceAgencyViewModel();
            viewModel.Id = policeAgency.Id;
            viewModel.Address = policeAgency.Address;
            viewModel.City = policeAgency.City;
            viewModel.AgencyName = policeAgency.AgencyName;
            viewModel.Email = policeAgency.Email;
            viewModel.Fax = policeAgency.Fax;
            viewModel.Phone = policeAgency.Phone;
            viewModel.State = policeAgency.State;
            viewModel.Zip = policeAgency.ZIP;
            viewModel.Contact = policeAgency.Contact;
            return viewModel;
        }


        internal static PoliceAgencyDto GetPoliceAgencyDto(PoliceAgencyViewModel viewModel)
        {
            var agencyDto = new PoliceAgencyDto();
            agencyDto.Id = (int)viewModel.Id;
            agencyDto.Address = viewModel.Address;
            agencyDto.City = viewModel.City;
            agencyDto.AgencyName = viewModel.AgencyName;
            agencyDto.Email = viewModel.Email;
            agencyDto.Fax = viewModel.Fax;
            agencyDto.Phone = viewModel.Phone;
            agencyDto.State = viewModel.State;
            agencyDto.ZIP = viewModel.Zip;
            agencyDto.Contact = viewModel.Contact;
            return agencyDto;
        }




        internal static LocationViewModel GetLocationViewModel(LocationDto locationDto)
        {
            LocationViewModel locationViewModel = new LocationViewModel();
            if (locationDto != null)
            {
                locationViewModel.Id = locationDto.Id;
                locationViewModel.Code = locationDto.Code;
                locationViewModel.Description = locationDto.Name;
                locationViewModel.CompanyAbbr = locationDto.CompanyAbbr;
                locationViewModel.SelectedCompany = locationDto.CompanyId;
                locationViewModel.State = locationDto.State;
                locationViewModel.Companies = LookUpHelpers.GetCompanyListItem();
                locationViewModel.IsUpdate = true;

            }

            return locationViewModel;
        }

        internal static LocationViewModel GetEmptyLocationViewModel()
        {
            LocationViewModel locationViewModel = new LocationViewModel();

            locationViewModel.Companies = LookUpHelpers.GetCompanyListItem();

            return locationViewModel;
        }

        internal static LocationDto GetLocationDto(LocationViewModel locationViewModel, long userId)
        {
            LocationDto locationDto = null;
            if (locationViewModel != null)
            {
                locationDto = new LocationDto();
                locationDto.Id = locationViewModel.Id;
                locationDto.Code = locationViewModel.Code;
                locationDto.Name = locationViewModel.Description;
                locationDto.State = locationViewModel.State;
                locationDto.CompanyId = locationViewModel.SelectedCompany;
                locationDto.AddedOrUpdatedUserId = userId;
            }
            return locationDto;
        }
        internal static LocationDto GetLocationDto(LocationViewModel locationViewModel)
        {
            LocationDto locationDto = null;
            if (locationViewModel != null)
            {
                locationDto = new LocationDto();
                locationDto.Id = locationViewModel.Id;
                locationDto.Code = locationViewModel.Code;
                locationDto.Name = locationViewModel.Description;
                locationDto.State = locationViewModel.State;
                locationDto.CompanyId = locationViewModel.SelectedCompany;
            }
            return locationDto;
        }

        internal static IEnumerable<WriteOffTypeViewModel> GetWriteOffListViewModel(IEnumerable<WriteOffTypeDTO> writeOffTypeList)
        {
            var viewModelList = new List<WriteOffTypeViewModel>();
            WriteOffTypeViewModel viewModel = null;

            foreach (var item in writeOffTypeList)
            {
                viewModel = new WriteOffTypeViewModel();
                viewModel.Id = item.Id;
                viewModel.WriteOffType = item.Type;
                viewModel.Description = item.Description;
                viewModel.Status = item.Status;
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }

        internal static WriteOffTypeViewModel GetClaimViewModel(WriteOffTypeDTO writeOffType)
        {
            WriteOffTypeViewModel viewModel = viewModel = new WriteOffTypeViewModel();
            viewModel.Id = writeOffType.Id;
            viewModel.WriteOffType = writeOffType.Type;
            viewModel.Description = writeOffType.Description;
            viewModel.Status = writeOffType.Status;
            return viewModel;
        }
    }
}