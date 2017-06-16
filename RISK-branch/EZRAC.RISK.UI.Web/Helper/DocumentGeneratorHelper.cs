using EZRAC.Risk.UI.Web.ViewModels.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using EZRAC.Risk.UI.Web.DocumentGenerator;
using System.Web.Hosting;
using EZRAC.RISK.Util.Common;


namespace EZRAC.Risk.UI.Web.Helper
{
    public static class DocumentGeneratorHelper
    {

        internal static IEnumerable<DocumentCategoriesViewModel> GetDocumentCategoriesViewModel(IEnumerable<DocumentCategoryDto> documentTypeDto)
        {

            var listViewModel = new List<DocumentCategoriesViewModel>();
            DocumentCategoriesViewModel viewModel = null;
            foreach (var item in documentTypeDto)
            {
                viewModel = new DocumentCategoriesViewModel();
                viewModel.Id = item.Id;
                viewModel.Category = item.Category;
                viewModel.DocumentTypes = GetDocumentTypeViewModel(item.DocumentTypes);
                listViewModel.Add(viewModel);
            }
            return listViewModel;
        }

        internal static IEnumerable<DocumentTypeViewModel> GetDocumentTypeViewModel(IEnumerable<DocumentTypeDto> documentTypeDto)
        {
            var listDocumentViewModel = new List<DocumentTypeViewModel>();
            DocumentTypeViewModel viewModel = null;

            foreach (var item in documentTypeDto)
            {
                viewModel = new DocumentTypeViewModel();
                viewModel.Category = item.Category;
                viewModel.Description = item.Description;
                viewModel.Id = item.Id;
                listDocumentViewModel.Add(viewModel);
            }
            return listDocumentViewModel;
        }



        internal static DocumentTemplateViewModel GetDocumentTemplateViewModel(DocumentHeaderDto documentTemplateDto)
        {
            DocumentTemplateViewModel documentTemplateViewModel = null;

            if (documentTemplateDto != null)
            {
                documentTemplateViewModel = new DocumentTemplateViewModel();
                documentTemplateViewModel.ClaimId = documentTemplateDto.ClaimId;

                documentTemplateViewModel.Company = GetCompanyViewModel(documentTemplateDto);

                documentTemplateViewModel.Contract = MapContractViewModel(documentTemplateDto.Contarct);

                documentTemplateViewModel.Location = MapLocationViewModel(documentTemplateDto.Location);

                documentTemplateViewModel.VehicleViewModel = GetVehicleViewModel(documentTemplateDto.VehicleDto);

                documentTemplateViewModel.RiskUserFullName = documentTemplateDto.AssignedUserFullName;
                documentTemplateViewModel.RiskUserEmail = documentTemplateDto.AssignedUserEmail;
                documentTemplateViewModel.RiskUserPhone = documentTemplateDto.AssignedUserPhoneNumber;

                documentTemplateViewModel.DriverViewModel = GetDriverViewModel(documentTemplateDto);


            }
            return documentTemplateViewModel;
        }

        private static LocationViewModel MapLocationViewModel(LocationDto locationDto)
        {
            LocationViewModel locationViewModel = null;
            if (locationDto != null)
            {
                locationViewModel = new LocationViewModel();
                locationViewModel.Id = locationDto.Id;
                locationViewModel.Code = locationDto.Code;
                locationViewModel.Name = locationDto.Name;
                locationViewModel.CompanyAbbr = locationDto.CompanyAbbr;
                locationViewModel.State = locationDto.State;
            }
            return locationViewModel;
        }

        private static CompanyViewModel GetCompanyViewModel(DocumentHeaderDto documentTemplateDto)
        {
            CompanyViewModel company = new CompanyViewModel();

            if (documentTemplateDto != null)
            {
                company.Name = documentTemplateDto.Company.Name;
                company.Abbr = documentTemplateDto.Company.Abbr;
                company.Address = documentTemplateDto.Company.Address;

                if ((ClaimsConstant.LossTypes)documentTemplateDto.LossTypeId == ClaimsConstant.LossTypes.TrafficParkingTicket)
                {
                    company.Phone = ConfigSettingsReader.GetAppSettingValue(AppSettings.TrafficParkingTicketPhone);
                    company.Fax = ConfigSettingsReader.GetAppSettingValue(AppSettings.TrafficParkingTicketFax);
                }
                else if ((ClaimsConstant.LossTypes)documentTemplateDto.LossTypeId == ClaimsConstant.LossTypes.ViolationTicket)
                {
                    company.Phone = ConfigSettingsReader.GetAppSettingValue(AppSettings.ViolationTicketPhone);
                    company.Fax = ConfigSettingsReader.GetAppSettingValue(AppSettings.ViolationTicketFax);
                }
                else
                {
                    company.Phone = documentTemplateDto.Company.Phone;
                    company.Fax = documentTemplateDto.Company.Fax;
                }
                company.State = documentTemplateDto.Company.State;
                company.City = documentTemplateDto.Company.City;
                company.Zip = documentTemplateDto.Company.Zip;
                company.Website = documentTemplateDto.Company.Website;
                company.Logopath = HostingEnvironment.MapPath(String.Format("~/Images/upgrade_{0}_logo.png", documentTemplateDto.Company.Abbr.ToLower()));
                company.AdLogoPath = HostingEnvironment.MapPath("~/Images/adlogo.png");
                company.EzLogoPath = HostingEnvironment.MapPath("~/Images/ezlogo.png");
            }
            return company;
        }

        private static DriverViewModel GetDriverViewModel(DocumentHeaderDto documentTemplateDto)
        {
            DriverViewModel driverViewModel = new DriverViewModel();
            if (documentTemplateDto.DriverInfo != null)
            {
                driverViewModel.Name = String.Format("{0} {1}", documentTemplateDto.DriverInfo.FirstName, documentTemplateDto.DriverInfo.LastName);
                driverViewModel.Address = documentTemplateDto.DriverInfo.Address;
                driverViewModel.Address2 = documentTemplateDto.DriverInfo.Address2;
                driverViewModel.City = documentTemplateDto.DriverInfo.City;
                driverViewModel.State = documentTemplateDto.DriverInfo.State;
                driverViewModel.Zip = documentTemplateDto.DriverInfo.Zip;
                driverViewModel.Email = documentTemplateDto.DriverInfo.Email;
                driverViewModel.Phone1 = documentTemplateDto.DriverInfo.Phone1;
                driverViewModel.Phone2 = documentTemplateDto.DriverInfo.Phone2;
                driverViewModel.DOB = documentTemplateDto.DriverInfo.DOB;
                driverViewModel.LicenceNumber = documentTemplateDto.DriverInfo.LicenceNumber;
                driverViewModel.LicenceState = documentTemplateDto.DriverInfo.LicenceState;
            }
            return driverViewModel;
        }



        internal static VehicleViewModel GetVehicleViewModel(VehicleDto vehicle)
        {
            VehicleViewModel vehicleViewModel = new VehicleViewModel();

            if (vehicle != null)
            {

                vehicleViewModel.UnitNumber = vehicle.UnitNumber;
                vehicleViewModel.Make = vehicle.Make;
                vehicleViewModel.Year = vehicle.Year;
                vehicleViewModel.Model = vehicle.Model;
                vehicleViewModel.VIN = vehicle.VIN;
                vehicleViewModel.Mileage = vehicle.Mileage;
                vehicleViewModel.Location = vehicle.Location;
                vehicleViewModel.Color = vehicle.Color;
                vehicleViewModel.TagNumber = vehicle.TagNumber;
            }

            return vehicleViewModel;
        }



        internal static IEnumerable<RiskBillingsViewModel> GetBillingViewModel(IEnumerable<RiskBillingDto> billingsDto)
        {
            IEnumerable<RiskBillingsViewModel> billings = null;

            if (billingsDto != null && billingsDto.Any())
            {
                billings = billingsDto.Select(x => new RiskBillingsViewModel
                {
                    Id = x.Id,
                    Amount = Math.Round(x.Amount, 2),
                    Discount = Math.Round(x.Discount.HasValue ? x.Discount.Value : default(double), 2),
                    BillingTypeId = x.BillingTypeId,
                    BillingTypeDesc = x.BillingTypeDesc,
                    ClaimId = x.ClaimId,
                    SubTotal = Math.Round(x.SubTotal, 2)

                }).ToList();
            }

            return billings;
        }

        internal static ContractViewModel MapContractViewModel(ContractDto ContractDto)
        {
            ContractViewModel contractViewModel = new ContractViewModel();
            if (ContractDto != null)
            {
                contractViewModel = new ContractViewModel();

                contractViewModel.Id = ContractDto.Id;
                contractViewModel.ContractNumber = ContractDto.ContractNumber;

                contractViewModel.CDW = ContractDto.CDW;
                contractViewModel.CDWVoided = ContractDto.CDWVoided;
                contractViewModel.LDWVoided = ContractDto.LDWVoided;
                contractViewModel.LDW = ContractDto.LDW;

                contractViewModel.PickupDate = ContractDto.PickupDate;
                contractViewModel.ReturnDate = ContractDto.ReturnDate;

            }
            return contractViewModel;
        }

        internal static InsuranceViewModel MapInsuranceViewModel(InsuranceDto insuranceDto)
        {
            InsuranceViewModel insuranceViewModel = new InsuranceViewModel();

            if (insuranceDto != null)
            {
                insuranceViewModel.Address = insuranceDto.Address;
                insuranceViewModel.City = insuranceDto.City;
                insuranceViewModel.State = insuranceDto.State;
                insuranceViewModel.Zip = insuranceDto.Zip;
                insuranceViewModel.InsuranceCompanyName = insuranceDto.CompanyName;
                insuranceViewModel.InsuranceClaimNumber = insuranceDto.InsuranceClaimNumber;
                insuranceViewModel.PolicyNumber = insuranceDto.InsurancePolicyNumber;
                insuranceViewModel.Fax = insuranceDto.Fax;
                insuranceViewModel.Phone = insuranceDto.Phone;

            }

            return insuranceViewModel;
        }

        internal static IEnumerable<PaymentInfoViewModel> GetPaymentsViewModel(PaymentDto paymentsDto)
        {
            IEnumerable<PaymentInfoViewModel> paymentInfoViewModel = null;

            if (paymentsDto != null && paymentsDto.Payments != null && paymentsDto.Payments.Any())
            {
                paymentInfoViewModel = paymentsDto.Payments.Select(x => new PaymentInfoViewModel
                {
                    Amount = x.Amount,
                    PaymentFrom = x.PaymentFrom,
                    ReceivedDate = x.ReceivedDate
                }).ToList();

            }

            return paymentInfoViewModel;


        }


        public static bool IsBillingsRequired(int documentId)
        {
            switch (documentId)
            {
                case (int)DocumentTypes.AR_DL1_RTR:
                    return true;
                case (int)DocumentTypes.AR_DL2_RTR:
                    return true;
                case (int)DocumentTypes.AR_DL3_RTR:
                    return true;
                case (int)DocumentTypes.Client_Info_and_Authorization:
                    return true;
                case (int)DocumentTypes.Demand_Letter_1_3rd_Party_Insurance:
                    return true;
                case (int)DocumentTypes.Demand_Letter_1_3rd_Party:
                    return true;
                case (int)DocumentTypes.Demand_Letter_1_Renter_Insurance:
                    return true;
                case (int)DocumentTypes.Demand_Letter_1_Renter:
                    return true;
                case (int)DocumentTypes.Demand_Letter_2_Renter:
                    return true;
                case (int)DocumentTypes.Demand_Letter_3_Renter:
                    return true;
                case (int)DocumentTypes.Demand_Letter_4_Renter:
                    return true;
                case (int)DocumentTypes.JNR_Debtor_Placement_Form:
                    return true;
                case (int)DocumentTypes.Payment_Balance_due:
                    return true;
                case (int)DocumentTypes.Payment_Claim_Closed:
                    return true;
                case (int)DocumentTypes.Payment_Plan:
                    return true;
                case (int)DocumentTypes.Wholesale_Bill_of_Sale:
                    return true;
                default: return false;
            }
        }

    }
}