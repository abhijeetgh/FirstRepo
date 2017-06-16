using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Data.Entity;
using EZRAC.RISK.Services.Implementation.Helper;

namespace EZRAC.RISK.Services.Implementation
{
    public class DocumentGeneratorService : IDocumentGeneratorService
    {
        IGenericRepository<RiskDocumentCategory> _documentCategoryRepository = null;
        IGenericRepository<RiskDriver> _riskDriverRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskBilling> _billingRepository = null;


        public DocumentGeneratorService(IGenericRepository<RiskDocumentCategory> documentCategoryRepository,
            IGenericRepository<RiskDriver> riskDriverRepository,
            IGenericRepository<Claim> claimRepository,
            IGenericRepository<RiskBilling> billingRepository
            )
        {

            _documentCategoryRepository = documentCategoryRepository;
            _riskDriverRepository = riskDriverRepository;
            _claimRepository = claimRepository;
            _billingRepository = billingRepository;

        }

        public async Task<IEnumerable<DocumentCategoryDto>> GetAllDocumentTypesAsync()
        {
            IEnumerable<DocumentCategoryDto> documentCategoriesDto = null;

            IEnumerable<RiskDocumentCategory> documentCategory = await _documentCategoryRepository.AsQueryable.Include(x => x.RiskDocumentTypes).ToListAsync();

            if (documentCategory != null)
            {
                documentCategoriesDto = MapDocumentCategoryDto(documentCategory);
            }

            return documentCategoriesDto;
        }



        public async Task<IEnumerable<DriverDto>> GetDriversByClaimAsync(long claimId)
        {
            var drivers = await _riskDriverRepository.AsQueryable.Include(x => x.RiskDriverInsurance).Where(x => x.ClaimId == claimId).ToListAsync();
            var driversDto = MapDriversDto(drivers);
            return driversDto;
        }

        public async Task<string> GetPolicyNumberByDriverId(long driverId)
        {
            var riskDriver = await _riskDriverRepository.AsQueryable.Include(x => x.RiskDriverInsurance).Where(x => x.Id == driverId).FirstOrDefaultAsync();
            var policyNumber = string.Empty;

            if (riskDriver.RiskDriverInsurance != null)
            {
                policyNumber = riskDriver.RiskDriverInsurance.PolicyNumber;
            }

            return policyNumber;
        }

        private IEnumerable<DocumentCategoryDto> MapDocumentCategoryDto(IEnumerable<RiskDocumentCategory> categories)//,IEnumerable<RiskDocumentType> types
        {
            var categoryDtoList = new List<DocumentCategoryDto>();
            DocumentCategoryDto categoryDto = null;

            foreach (var category in categories)
            {
                categoryDto = new DocumentCategoryDto();
                categoryDto.Category = category.Category;
                categoryDto.Id = category.Id;
                categoryDto.DocumentTypes = MapRiskDocumentType(category.RiskDocumentTypes);
                categoryDtoList.Add(categoryDto);
            }
            return categoryDtoList;
        }


        private IEnumerable<DocumentTypeDto> MapRiskDocumentType(IEnumerable<RiskDocumentType> types)
        {
            var listDocTypeDto = new List<DocumentTypeDto>();
            DocumentTypeDto documentTypeDto = null;

            if (types != null)
            {
                foreach (var type in types.Where(x => !x.IsDeleted))
                {
                    documentTypeDto = new DocumentTypeDto();
                    documentTypeDto.Id = type.Id;
                    documentTypeDto.Description = type.Description;
                    documentTypeDto.Category = type.CategoryId;
                    listDocTypeDto.Add(documentTypeDto);
                }

            }


            return listDocTypeDto;
        }



        private IEnumerable<DriverDto> MapDriversDto(IEnumerable<RiskDriver> drivers)
        {
            DriverDto driverDto = null;
            var listDrivers = new List<DriverDto>();
            foreach (var item in drivers)
            {
                driverDto = new DriverDto();
                driverDto.DriverId = item.Id;
                driverDto.FirstName = item.FirstName;
                driverDto.LastName = item.LastName;
                driverDto.Address = item.Address;
                driverDto.Address2 = item.Address2;
                driverDto.State = item.State;
                driverDto.City = item.City;
                driverDto.Zip = item.Zip;
                driverDto.Phone1 = item.Phone1;
                driverDto.Phone2 = item.Phone2;
                driverDto.Fax = item.Fax;
                driverDto.OtherContact = item.OtherContact;
                driverDto.Email = item.Email;
                driverDto.LicenceNumber = item.LicenceNumber;
                driverDto.DOB = item.DOB;
                driverDto.LicenceExpiryDate = item.LicenceExpiryDate;
                driverDto.LicenceState = item.LicenceState;
                driverDto.IsAuthorized = item.IsAuthorized;
                driverDto.DriverTypeId = item.DriverTypeId;
                if (item.RiskDriverInsurance != null)
                {
                    driverDto.InsuranceCompany = item.RiskDriverInsurance.InsuranceCompanyName;
                }
                listDrivers.Add(driverDto);
            }
            return listDrivers;
        }


        public async Task<double> GetTotalDueAsync(long[] selectedBillings, long claimId)
        {

            double totalDue = default(double);

            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskBillings,
                                                                             x => x.RiskContract).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim != null)
            {
                double totalBilling = GetTotalBilling(GetSelectedBillings(selectedBillings, claim.RiskBillings));

                if (claim.RiskContract != null && ((claim.RiskContract.CDW && !claim.RiskContract.CDWVoid) ||
                        (claim.RiskContract.LDW && !claim.RiskContract.LDWVoid)))
                {
                    totalDue = default(double);
                }
                else
                {
                    totalDue = totalBilling - (claim.TotalPayment.HasValue ? claim.TotalPayment.Value : default(double));
                }
            }

            return totalDue > 0 ? totalDue : default(double);
        }

        public async Task<double> GetOriginalBalanceAsync(long[] selectedBillings, long claimId)
        {

            double totalDue = default(double);

            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskBillings,
                                                                             x => x.RiskContract).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim != null)
            {
                double totalBilling = GetTotalBilling(GetSelectedBillings(selectedBillings, claim.RiskBillings));

                if (claim.RiskContract != null && ((claim.RiskContract.CDW && !claim.RiskContract.CDWVoid) ||
                        (claim.RiskContract.LDW && !claim.RiskContract.LDWVoid)))
                {
                    totalDue = default(double);
                }
                else if (claim.RiskContract != null && claim.RiskContract.LPC2)
                {
                    totalDue = totalBilling > Convert.ToInt64(ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.LPC2Deductible)) ? (totalBilling - Convert.ToInt64(ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.LPC2Deductible))) : default(double);
                }
                else
                {
                    totalDue = totalBilling;
                }
            }

            return totalDue > 0 ? totalDue : default(double);
        }

        private double GetTotalBilling(IList<RiskBilling> riskBilling)
        {
            double totalAmount = default(double);

            if (riskBilling != null && riskBilling.Any())
            {
                foreach (var billing in riskBilling)
                {
                    double subTotal = billing.Discount.HasValue ? billing.Amount * (1 - billing.Discount.Value / 100) : billing.Amount;

                    totalAmount += subTotal;

                }
            }
            return totalAmount;
        }


        private IList<RiskBilling> GetSelectedBillings(long[] selectedBillings, IList<RiskBilling> claimRiskBillings)
        {
            List<RiskBilling> selectedRiskBillings = null;
            if (selectedBillings != null && selectedBillings.Any())
            {
                selectedRiskBillings = new List<RiskBilling>();

                foreach (var item in selectedBillings)
                {
                    selectedRiskBillings.Add(claimRiskBillings.Where(x => x.Id == item).FirstOrDefault());
                }
            }

            return selectedRiskBillings;
        }




        public async Task<DocumentHeaderDto> GetDocumentHeaderInfoAsync(long claimId, long selectedDriverId)
        {
            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.AssignedUser,
                                                                             x => x.RiskContract,
                                                                              x => x.OpenLocation.Company,
                                                                              x => x.RiskDrivers,
                                                                              x => x.RiskVehicle,
                                                                              x => x.RiskIncident).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            DocumentHeaderDto documentHeaderDto = null;

            if (claimId != 0 && selectedDriverId != 0)
            {
                ContractDto contract = MapContractInfoDto(claim.RiskContract);

                documentHeaderDto = new DocumentHeaderDto();

                documentHeaderDto.Contarct = MapContractInfoDto(claim.RiskContract);

                documentHeaderDto.VehicleDto = MapVehicleInfoDto(claim.RiskVehicle, null);

                documentHeaderDto.ClaimId = claimId;

                documentHeaderDto.DriverInfo = GetDriverInfo(selectedDriverId, claim.RiskDrivers);

                documentHeaderDto.OpenDate = claim.OpenDate;

                documentHeaderDto.AssignedUserEmail = claim != null && claim.AssignedUser != null ? claim.AssignedUser.Email : String.Empty;

                documentHeaderDto.AssignedUserFullName = claim != null && claim.AssignedUser != null ? String.Format("{0} {1}", claim.AssignedUser.FirstName,
                                                                                            claim.AssignedUser.LastName) : String.Empty;
                documentHeaderDto.AssignedUserPhoneNumber = documentHeaderDto.DriverInfo != null && documentHeaderDto.DriverInfo != null ? Convert.ToString(documentHeaderDto.DriverInfo.Phone1) : string.Empty;

                documentHeaderDto.Location = MapLocationDto(claim.OpenLocation);

                if (claim.OpenLocation != null && claim.OpenLocation.Company != null)
                {
                    documentHeaderDto.Company = MapCompanyDto(claim.OpenLocation.Company);
                }

                documentHeaderDto.DateOfLoss = claim.RiskIncident != null ? claim.RiskIncident.LossDate : null;

                documentHeaderDto.ContractNumber = contract != null ? contract.ContractNumber : String.Empty;

                documentHeaderDto.LossTypeId = claim.LossTypeId;

            }

            return documentHeaderDto;
        }

        private LocationDto MapLocationDto(Location location)
        {
            LocationDto locationDto = null;
            if (location != null)
            {
                locationDto = new LocationDto();
                locationDto.Id = location.Id;
                locationDto.Code = location.Code;
                locationDto.Name = location.Name;
                locationDto.CompanyAbbr = location.CompanyAbbr;
                locationDto.State = location.State;
            }
            return locationDto;
        }

        public async Task<Nullable<DateTime>> GetDateOfLastPaymentAsync(long claimId)
        {
            Nullable<DateTime> lastPaymentDate = null;
            var claims = await _claimRepository.AsQueryable.Include(x => x.RiskPayments).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            if (claims.RiskPayments.Any())
            {
                lastPaymentDate = claims.RiskPayments.OrderByDescending(x => x.PaymentDate).Take(1).FirstOrDefault().PaymentDate;
            }
            return lastPaymentDate;
        }

        public async Task<double> GetActualCashValueForClaim(long claimId)
        {
            var claim = await _claimRepository.AsQueryable.Include(x => x.RiskBillings).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            var amount = claim.RiskBillings.Where(x => x.BillingTypeId == Convert.ToInt32(Constants.BillingTypes.ActualCashValue)).Sum(x => x.Amount);
            return amount;
        }

        public async Task<double> GetEstimatedBilling(long claimId)
        {
            var claim = await _claimRepository.AsQueryable.Include(x => x.RiskBillings).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            var estimatedBill = claim.RiskBillings.Where(x => x.BillingTypeId == Convert.ToInt32(Constants.BillingTypes.Estimate)).Sum(x => x.Amount);
            return estimatedBill;
        }

        public async Task<IncidentDto> GetIncidentByClaimIdAsync(long claimId)
        {
            var claim = await _claimRepository.AsQueryable.Include(x => x.RiskIncident).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            IncidentDto incidentDto = MapIncidentInfoDTO(claim.RiskIncident);
            return incidentDto;
        }

        private DriverForDocumentDto GetDriverInfo(long selectedDriverId, IList<RiskDriver> drivers)
        {
            DriverForDocumentDto driverForDocumentDto = null;

            if (drivers != null && drivers.Any())
            {
                RiskDriver driver = drivers.Where(x => x.Id == selectedDriverId).FirstOrDefault();

                if (driver != null)
                {
                    driverForDocumentDto = new DriverForDocumentDto();
                    driverForDocumentDto.Address = driver.Address;
                    driverForDocumentDto.Address2 = driver.Address2;
                    driverForDocumentDto.City = driver.City;
                    driverForDocumentDto.State = driver.State;
                    driverForDocumentDto.FirstName = driver.FirstName;
                    driverForDocumentDto.LastName = driver.LastName;
                    driverForDocumentDto.Email = Convert.ToString(driver.Email);
                    driverForDocumentDto.Zip = driver.Zip;
                    driverForDocumentDto.Phone1 = (driver.Phone1 != null) ? driver.Phone1 : string.Empty;
                    driverForDocumentDto.Phone2 = (driver.Phone2 != null) ? driver.Phone2 : string.Empty;
                    driverForDocumentDto.DOB = driver.DOB;
                    driverForDocumentDto.LicenceNumber = driver.LicenceNumber;
                    driverForDocumentDto.LicenceState = driver.LicenceState;
                }
            }






            return driverForDocumentDto;
        }

        private VehicleDto MapVehicleInfoDto(RiskVehicle riskVehicle, RiskContract contract)
        //IEnumerable<string> swappedVehicles)
        {
            VehicleDto vehicleInfo = null;
            if (riskVehicle != null)
            {
                vehicleInfo = new VehicleDto();

                vehicleInfo.Id = riskVehicle.Id;
                vehicleInfo.UnitNumber = riskVehicle.UnitNumber;
                vehicleInfo.TagNumber = riskVehicle.TagNumber;
                vehicleInfo.TagExpires = riskVehicle.TagExpires;
                vehicleInfo.VIN = riskVehicle.VIN;
                vehicleInfo.Model = riskVehicle.Model;
                vehicleInfo.Make = riskVehicle.Make;
                vehicleInfo.Year = riskVehicle.Year;
                vehicleInfo.Color = riskVehicle.Color;
                vehicleInfo.Mileage = riskVehicle.Mileage;
                vehicleInfo.Location = riskVehicle.Location;
                vehicleInfo.Status = riskVehicle.Status;
                vehicleInfo.Description = riskVehicle.Description;

                vehicleInfo.SwappedVehicles = contract != null ? contract.SwapedVehicles : String.Empty;
            }
            return vehicleInfo;
        }


        public async Task<IEnumerable<RiskBillingDto>> GetSelectedBillingsByIdsAsync(long[] selectedBillings)
        {
            IEnumerable<RiskBillingDto> billingDtoList = null;
            if (selectedBillings != null && selectedBillings.Any())
            {
                IList<RiskBilling> billings = await _billingRepository.AsQueryable.Include(x => x.RiskBillingType).Where(x => selectedBillings.Contains(x.Id)).ToListAsync();

                if (billings != null && billings.Any())
                {

                    billingDtoList = MapBillingDto(billings);

                }
            }
            return billingDtoList;

        }

        public async Task<InsuranceDto> GetInsuranceInfoByDriverId(long driverId)
        {
            InsuranceDto insuranceDto = null;

            RiskDriver diver = await _riskDriverRepository.AsQueryable.IncludeMultiple(x => x.RiskDriverInsurance,
                                                                                                    x => x.RiskDriverInsurance.RiskInsurance).
                                                        Where(x => x.Id == driverId).FirstOrDefaultAsync();

            if (diver != null && diver.RiskDriverInsurance != null && diver.RiskDriverInsurance.RiskInsurance != null)
            {
                insuranceDto = MapInsuranceDto(diver.RiskDriverInsurance.RiskInsurance);
                insuranceDto.InsuranceClaimNumber = diver.RiskDriverInsurance.InsuranceClaimNumber;
                insuranceDto.InsurancePolicyNumber = diver.RiskDriverInsurance.PolicyNumber;
            }

            return insuranceDto;
        }

        public async Task<string> GetSliPolicyNumberByClaimId(long claimId)
        {
            string sliPolicyNumber = string.Empty;
            var claim = await _claimRepository.AsQueryable.Include(x => x.OpenLocation.Sli).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            if (claim != null && claim.OpenLocation != null && claim.OpenLocation.Sli != null && claim.OpenLocation.Sli.Any())
            {
                sliPolicyNumber = claim.OpenLocation.Sli.FirstOrDefault().PolicyNumber;
            }
            return sliPolicyNumber;
        }


        private IEnumerable<RiskBillingDto> MapBillingDto(IList<RiskBilling> riskBilling)
        {
            List<RiskBillingDto> billings = null;

            if (riskBilling != null)
            {
                billings = new List<RiskBillingDto>();

                billings = riskBilling.Select(x => new RiskBillingDto
                {
                    Id = x.Id,
                    BillingTypeId = x.BillingTypeId,
                    Amount = x.Amount,
                    BillingTypeDesc = x.RiskBillingType.Description,
                    Discount = x.Discount,
                    ClaimId = x.ClaimId,
                    SubTotal = x.Discount.HasValue ? x.Amount * (1 - x.Discount.Value / 100) : x.Amount
                }).ToList();
            }
            return billings;
        }

        private InsuranceDto MapInsuranceDto(RiskInsurance riskInsurance)
        {
            var insuranceDto = new InsuranceDto();
            insuranceDto.Id = riskInsurance.Id;
            insuranceDto.CompanyName = riskInsurance.CompanyName;
            insuranceDto.Address = riskInsurance.Address;
            insuranceDto.City = riskInsurance.City;
            insuranceDto.Contact = riskInsurance.Contact;
            insuranceDto.Email = riskInsurance.Email;
            insuranceDto.Fax = riskInsurance.Fax;
            insuranceDto.Notes = riskInsurance.Notes;
            insuranceDto.Phone = riskInsurance.Phone;
            insuranceDto.State = riskInsurance.State;
            insuranceDto.Zip = riskInsurance.Zip;
            return insuranceDto;
        }


        private IncidentDto MapIncidentInfoDTO(RiskIncident riskIncident)
        {
            IncidentDto incidentInfo = null;

            if (riskIncident != null)
            {
                incidentInfo = new IncidentDto();

                incidentInfo.Id = riskIncident.Id;
                incidentInfo.LossDate = riskIncident.LossDate;
                incidentInfo.SelectedLocationName = riskIncident.LocationName;
                incidentInfo.SelectedPoliceAgencyId = riskIncident.PoliceAgencyId;
                incidentInfo.SelectedPoliceAgencyName = riskIncident.PoliceAgencyName;
                incidentInfo.SelectedLocationId = riskIncident.LocationId;
                incidentInfo.CaseNumber = riskIncident.CaseNumber;

                incidentInfo.RenterFault = riskIncident.RenterFault;
                incidentInfo.ThirdPartyFault = riskIncident.ThirdPartyFault;

                incidentInfo.ReportedDate = riskIncident.ReportedDate;
            }
            return incidentInfo;
        }

        private ContractDto MapContractInfoDto(RiskContract riskContract)
        {
            ContractDto contractInfo = null;
            if (riskContract != null)
            {
                contractInfo = new ContractDto();

                contractInfo.Id = riskContract.Id;
                contractInfo.ContractNumber = riskContract.ContractNumber;
                contractInfo.PickupDate = riskContract.PickUpDate;
                contractInfo.ReturnDate = riskContract.ReturnDate;
                contractInfo.DaysOut = riskContract.DaysOut;
                contractInfo.Miles = riskContract.Miles;
                contractInfo.DailyRate = riskContract.DailyRate;
                contractInfo.WeeklyRate = riskContract.WeeklyRate;
                contractInfo.CDW = riskContract.CDW;
                contractInfo.CDWVoided = riskContract.CDWVoid;
                contractInfo.LDWVoided = riskContract.LDWVoid;
                contractInfo.LDW = riskContract.LDW;
                contractInfo.SLI = riskContract.SLI;
                contractInfo.LPC = riskContract.LPC;
                contractInfo.LPC2 = riskContract.LPC2;
                contractInfo.TotalRate = riskContract.TotalRate;
                contractInfo.GARS = riskContract.GARS;

            }
            return contractInfo;
        }

        private CompanyDto MapCompanyDto(Company company)
        {
            CompanyDto companyDto = null;
            if (company != null)
            {
                companyDto = new CompanyDto();
                companyDto.Abbr = company.Abbr;
                companyDto.Address = company.Address;
                companyDto.City = company.City;
                companyDto.Fax = company.Fax;
                companyDto.State = company.State;
                companyDto.Zip = company.Zip;
                companyDto.Phone = company.Phone;
                companyDto.Name = company.Name;
                companyDto.Website = company.Website;
            }
            return companyDto;
        }


        public async Task<double> GetSalvageBillByClaimIdAsync(long claimId)
        {
            double total = default(double);
            RiskBilling billing = await _billingRepository.AsQueryable.Where(x => x.ClaimId == claimId && x.BillingTypeId == (long)Constants.BillingTypes.Salvage).FirstOrDefaultAsync();

            if (billing != null)
            {
                total = billing.Discount.HasValue ? billing.Amount * (1 - billing.Discount.Value / 100) : billing.Amount;

            }

            return total;


        }
    }
}
