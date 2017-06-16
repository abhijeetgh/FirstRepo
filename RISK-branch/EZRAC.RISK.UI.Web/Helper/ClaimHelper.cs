using EZRAC.Risk.UI.Web.ViewModels.ClaimBasicInfo;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class ClaimHelper
    {
        internal static IEnumerable<ClaimInfoViewModel> GetClaimListViewModel(IEnumerable<ClaimDto> listOfClaims)
        {
            ClaimInfoViewModel claimToAdd;
            List<ClaimInfoViewModel> objViewModel = new List<ClaimInfoViewModel>();

            foreach (ClaimDto claim in listOfClaims)
            {
                claimToAdd = new ClaimInfoViewModel();
                // claimToAdd.SelectedAssignedUser = claim.SelectedAssignedUser;
                claimToAdd.ClaimID = Convert.ToInt32(claim.Id);
                claimToAdd.ContractNumber = claim.ContractNo;
                claimToAdd.DriverName = claim.DriverName;
                claimToAdd.FollowupDate = claim.FollowUpdate;
                //  claimToAdd.SelectedStatus = claim.SelectedStatus;
                claimToAdd.UnitNumber = claim.UnitNumber;
                claimToAdd.VehicleName = claim.VehicleName;
                claimToAdd.Completed = claim.IsComplete;
                claimToAdd.Users = LookUpHelpers.GetSelectListItems(claim.Users);
                claimToAdd.SelectedAssignedUserId = claim.SelectedAssignedUserId;
                claimToAdd.SelectedAssignedUserName = claim.SelectedAssignedUserName;
                claimToAdd.SelectedStatusName = claim.SelectedStatusName;
                claimToAdd.ApprovalStatus = claim.ApprovalStatus;
                claimToAdd.CompanyAbbr = claim.CompanyAbbr != null ? claim.CompanyAbbr.ToUpper() : String.Empty;
                objViewModel.Add(claimToAdd);
            }


            return objViewModel;
        }

        internal static CreateClaimViewModel GetCreateClaimViewModel()
        {
            CreateClaimViewModel createClaimVM = new CreateClaimViewModel();

            createClaimVM.Users = LookUpHelpers.GetAssignedToUsersListItem();
            createClaimVM.Locations = LookUpHelpers.GetLocationListItem();
            createClaimVM.LossTypes = LookUpHelpers.GetLossTypeListItems();
            createClaimVM.DateOfLoss = DateTime.Now.Date;
            return createClaimVM;

        }


        internal static FetchDetailsViewModel GetContractInfoFromTSDViewModel(FetchedContractDetailsDto objFetchedDetails, IEnumerable<ClaimDto> claimList)
        {
            FetchDetailsViewModel fetchedContractInfo = new FetchDetailsViewModel();

            fetchedContractInfo.ContractNo = objFetchedDetails.ContractNo;
            fetchedContractInfo.DateIn = objFetchedDetails.DateIn.ToString();
            fetchedContractInfo.DateOut = objFetchedDetails.DateOut.ToString();
            fetchedContractInfo.OriginalDate = objFetchedDetails.OriginalDate.ToString();
            fetchedContractInfo.FirstName = objFetchedDetails.FirstName;
            fetchedContractInfo.LastName = objFetchedDetails.LastName;
            fetchedContractInfo.DOB = objFetchedDetails.DOB;
            fetchedContractInfo.LicenceNo = objFetchedDetails.LicenseNumber;
            fetchedContractInfo.UnitNo = objFetchedDetails.UnitNo;
            fetchedContractInfo.ClaimsOnContract = claimList;
            fetchedContractInfo.LocationId = objFetchedDetails.LocationId;
            fetchedContractInfo.SwappedVehicles = objFetchedDetails.SwappedVehicles;

            PurchaseType purchaseType;

            if (Enum.TryParse<PurchaseType>(objFetchedDetails.PurchaseType, true, out purchaseType))
            {
                fetchedContractInfo.PurchaseType = purchaseType;
            }

            return fetchedContractInfo;
        }

        internal static ClaimBasicInfoViewModel GetClaimBasicInfoViewModel(ClaimBasicInfoDto claimBasicInfo)
        {
            ClaimBasicInfoViewModel claimBasicInfoViewModel = null;

            if (claimBasicInfo != null)
            {
                claimBasicInfoViewModel = new ClaimBasicInfoViewModel();

                claimBasicInfoViewModel.ClaimInfo = GetClaimInfoForViewEditViewModel(claimBasicInfo.ClaimInfo, false);

                Int64 claimId = claimBasicInfo.ClaimInfo != null ? claimBasicInfo.ClaimInfo.Id : default(Int64);

                claimBasicInfoViewModel.ContractInfo = GetContractInfoViewModel(claimBasicInfo.ContractInfo, claimId);

                claimBasicInfoViewModel.NonContractInfo = GetNonContractInfoViewModel(claimBasicInfo.NonContractInfo, claimId);

                claimBasicInfoViewModel.IncidentInfo = GetIncidentInfoViewModel(claimBasicInfo.IncidentInfo, false);

                claimBasicInfoViewModel.VehicleInfo = GetVehicleInfoViewModel(claimBasicInfo.VehicleInfo, claimId);

                //Passed in vehicle info to get swapped vehicles
                //TODO: Need to remove and swapped vehicles should be saved in database or swapped vehicles should be moved to contract section
                if (claimBasicInfoViewModel.VehicleInfo != null & claimBasicInfo.ContractInfo != null)
                {
                    claimBasicInfoViewModel.VehicleInfo.ContractNumber = claimBasicInfo.ContractInfo.ContractNumber;
                }


            }
            return claimBasicInfoViewModel;
        }


        internal static VehicleInfoViewModel GetVehicleInfoViewModel(VehicleDto vehicleInfoDto, Int64 claimId)
        {
            VehicleInfoViewModel vehicleInfoViewModel = new VehicleInfoViewModel();

            if (vehicleInfoDto != null)
            {


                vehicleInfoViewModel.Id = vehicleInfoDto.Id;
                vehicleInfoViewModel.UnitNumber = vehicleInfoDto.UnitNumber;


                vehicleInfoViewModel.TagExpires = vehicleInfoDto.TagExpires;
                vehicleInfoViewModel.TagNumber = vehicleInfoDto.TagNumber;


                vehicleInfoViewModel.VIN = vehicleInfoDto.VIN;
                vehicleInfoViewModel.VehicleShortDesc = String.Format("{0} {1} {2} - {3}", vehicleInfoDto.Year, vehicleInfoDto.Make,
                                                                                       vehicleInfoDto.Model,
                                                                                       vehicleInfoDto.Color);

                vehicleInfoViewModel.TagNumberAndExp = vehicleInfoDto.TagExpires.HasValue ?
                    String.Format("{0} - {1}", vehicleInfoDto.TagNumber, vehicleInfoDto.TagExpires.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY)) :
                                                                                            vehicleInfoDto.TagNumber;

                vehicleInfoViewModel.Make = vehicleInfoDto.Make;
                vehicleInfoViewModel.Model = vehicleInfoDto.Model;
                vehicleInfoViewModel.Year = vehicleInfoDto.Year;
                vehicleInfoViewModel.Color = vehicleInfoDto.Color;
                vehicleInfoViewModel.Location = vehicleInfoDto.Location;
                vehicleInfoViewModel.Status = vehicleInfoDto.Status;

                vehicleInfoViewModel.Mileage = vehicleInfoDto.Mileage;
                vehicleInfoViewModel.LocationNameAndStatus = String.Format("{0} / {1}",
                    vehicleInfoDto.Location != null ? vehicleInfoDto.Location : String.Empty,
                    vehicleInfoDto.Status != null ? vehicleInfoDto.Status : String.Empty);

                vehicleInfoViewModel.PurchaseType = vehicleInfoDto.PurchaseType;

                vehicleInfoViewModel.SwappedVehicles = vehicleInfoDto.SwappedVehicles != null ? vehicleInfoDto.SwappedVehicles.Split(new char[] { ',' }) : null;
            }

            vehicleInfoViewModel.ClaimId = claimId;
            return vehicleInfoViewModel;
        }

        internal static IncidentInfoViewModel GetIncidentInfoViewModel(IncidentDto incidentInfo, bool isEditView)
        {
            IncidentInfoViewModel incidentInfoToAdd = new IncidentInfoViewModel(); //TODO: need to add incidents for all claims

            if (isEditView)
            {
                incidentInfoToAdd.Locations = LookUpHelpers.GetLocationListItem();
            }

            incidentInfoToAdd.PoliceAgencies = LookUpHelpers.GetAllPoliceAgenciesListItem();

            if (incidentInfo != null)
            {
                //incidentInfoToAdd = new IncidentInfoViewModel();
                incidentInfoToAdd.Id = incidentInfo.Id;
                incidentInfoToAdd.LossDate = incidentInfo.LossDate;

                incidentInfoToAdd.SelectedPoliceAgencyId = incidentInfo.SelectedPoliceAgencyId;
                incidentInfoToAdd.SelectedPoliceAgencyName = incidentInfo.SelectedPoliceAgencyName;
                incidentInfoToAdd.SelectedLocationId = incidentInfo.SelectedLocationId;

                incidentInfoToAdd.SelectedLocationName = incidentInfo.SelectedLocationName;

                // incidentInfoToAdd.PoliceAgency = LookUpHelpers.GetSelectListItems(claimBasicInfo.IncidentInfo.PoliceAgencies);
                incidentInfoToAdd.CaseNumber = incidentInfo.CaseNumber;
                incidentInfoToAdd.RenterFault = incidentInfo.RenterFault;
                incidentInfoToAdd.ThirdPartyFault = incidentInfo.ThirdPartyFault;
                incidentInfoToAdd.ReportedDate = incidentInfo.ReportedDate;
            }
            return incidentInfoToAdd;
        }

        internal static ContractInfoViewModel GetContractInfoViewModel(ContractDto contractInfo, Int64 claimId)
        {
            ContractInfoViewModel contractInfoViewModel = new ContractInfoViewModel();

            if (contractInfo != null)
            {

                contractInfoViewModel.Id = contractInfo.Id;
                contractInfoViewModel.ContractNumber = contractInfo.ContractNumber;
                contractInfoViewModel.PickupDate = contractInfo.PickupDate;
                contractInfoViewModel.ReturnDate = contractInfo.ReturnDate;
                contractInfoViewModel.DaysOut = contractInfo.DaysOut;
                contractInfoViewModel.Miles = contractInfo.Miles;

                if (contractInfo.DailyRate.HasValue)
                    contractInfoViewModel.DailyRate = (double)Math.Round(contractInfo.DailyRate.Value, 2);

                if (contractInfo.WeeklyRate.HasValue)
                    contractInfoViewModel.WeeklyRate = (double)Math.Round(contractInfo.WeeklyRate.Value, 2);

                string dailyRate = default(string);
                string weeklyRate = default(string);

                if (contractInfo.DailyRate.HasValue)
                    dailyRate = Math.Round(contractInfo.DailyRate.Value, 2).ToString();

                if (contractInfo.WeeklyRate.HasValue)
                    weeklyRate = Math.Round(contractInfo.WeeklyRate.Value, 2).ToString();

                contractInfoViewModel.Rates = String.Format("D- ${0} /W- ${1}", dailyRate, weeklyRate);

                contractInfoViewModel.CDW = contractInfo.CDW;
                contractInfoViewModel.CDWVoided = contractInfo.CDWVoided;
                contractInfoViewModel.LDWVoided = contractInfo.LDWVoided;
                contractInfoViewModel.LDW = contractInfo.LDW;
                contractInfoViewModel.SLI = contractInfo.SLI;
                contractInfoViewModel.LPC = contractInfo.LPC;
                contractInfoViewModel.LPC2 = contractInfo.LPC2;
                contractInfoViewModel.GARS = contractInfo.GARS;
                contractInfoViewModel.CardNumber = contractInfo.CardNumber;
                contractInfoViewModel.CardType = contractInfo.CardType;
                contractInfoViewModel.CardExpDate = contractInfo.CardExpDate;



            }

            //Populated for downloading of contract
            contractInfoViewModel.ClaimId = claimId;

            return contractInfoViewModel;
        }

        internal static IEnumerable<DriverAndInsuranceViewModel> GetDriversAndInsuranceInfoViewModel(IEnumerable<DriverInfoDto> driverAndInsuranceInfo, Int64 claimId)
        {


            List<DriverAndInsuranceViewModel> driverListToReturn = new List<DriverAndInsuranceViewModel>();

            if (driverAndInsuranceInfo.Any())
            {
                DriverAndInsuranceViewModel driverAndInsuranceViewModel = null;
                foreach (DriverInfoDto driver in driverAndInsuranceInfo)
                {
                    driverAndInsuranceViewModel = GetDriverAndInsuranceInfoViewModel(driver, claimId);

                    if (driverAndInsuranceViewModel != null)
                        driverAndInsuranceViewModel.ClaimId = claimId;

                    driverListToReturn.Add(driverAndInsuranceViewModel);
                }
            }


            //Adding empty primary and additional divers 
            if (!driverListToReturn.Where(x => x.DriverType == Convert.ToInt32(ClaimsConstant.DriverTypes.Primary)).Any())
            {
                driverListToReturn.Add(new DriverAndInsuranceViewModel() { DriverType = Convert.ToInt32(ClaimsConstant.DriverTypes.Primary), ClaimId = claimId });

            }
            if (!driverListToReturn.Where(x => x.DriverType == Convert.ToInt32(ClaimsConstant.DriverTypes.Additional)).Any())
            {
                driverListToReturn.Add(new DriverAndInsuranceViewModel() { DriverType = Convert.ToInt32(ClaimsConstant.DriverTypes.Additional), ClaimId = claimId });
            }


            return driverListToReturn.OrderBy(d => d.DriverType);
        }

        internal static DriverAndInsuranceViewModel GetDriverAndInsuranceInfoViewModel(DriverInfoDto driver, Int64 claimId)
        {
            DriverAndInsuranceViewModel driverToAdd = null;
            if (driver != null)
            {
                driverToAdd = new DriverAndInsuranceViewModel();

                driverToAdd.DriverId = driver.Id;
                driverToAdd.FirstName = driver.FirstName;
                driverToAdd.LastName = driver.LastName;
                driverToAdd.Address1 = driver.Address1;
                driverToAdd.Address2 = driver.Address2;
                driverToAdd.State = driver.State;
                driverToAdd.City = driver.City;
                driverToAdd.Zip = driver.Zip;
                driverToAdd.Phone1 = driver.Phone1;
                driverToAdd.Phone2 = driver.Phone2;
                driverToAdd.Fax = driver.Fax;
                driverToAdd.OtherContact = driver.OtherContact;
                driverToAdd.Email = driver.Email;
                driverToAdd.LicenceNumber = driver.LicenceNumber;
                driverToAdd.DOB = driver.DOB;
                driverToAdd.LicenceExpiry = driver.LicenceExpiry;
                driverToAdd.LicenceState = driver.LicenceState;
                driverToAdd.IsAuthorizedDriver = driver.IsAuthorizedDriver;
                driverToAdd.DriverType = driver.DriverTypeId;


                driverToAdd.InsuranceCompanies = LookUpHelpers.GetAllInsuranceComapanyListItems();

                driverToAdd.InsuranceCompanyId = driver.InsuranceId;
                driverToAdd.InsuranceCompanyName = driver.InsuranceCompanyName;
                driverToAdd.PolicyNumber = driver.PolicyNumber;
                driverToAdd.Deductible = driver.Deductible;
                driverToAdd.InsuranceClaimNumber = driver.InsuranceClaimNumber;
                driverToAdd.InsuranceExpiry = driver.InsuranceExpiry;
                driverToAdd.CreditCardCompany = driver.CreditCardCompany;
                driverToAdd.CreditCardPolicyNumber = driver.CreditCardPolicyNumber;
                driverToAdd.CreditCardCoverageAmt = driver.CreditCardCoverageAmount;
                driverToAdd.ClaimId = claimId;
            }
            return driverToAdd;
        }


        internal static BillingViewModel GetBillingsViewModel(BillingsDto billingsDto, Nullable<double> labourHour, bool isEdit)
        {
            BillingViewModel billingViewModel = new BillingViewModel();

            if (isEdit)
            {
                billingViewModel.BillingTypes = LookUpHelpers.GetAllBillingTypes();
            }
            billingViewModel.LabourHour = labourHour;
            if (billingsDto != null)
            {
                billingViewModel.TotalBilling = Math.Round(billingsDto.TotalBilling, 2);
                billingViewModel.TotalDue = Math.Round(billingsDto.TotalDue, 2);
                billingViewModel.ClaimId = billingsDto.ClaimId;
                billingViewModel.AutoAdminFeeCalculate = true;
                //should not assign object directly. Need to change
                if (billingsDto.Billings != null)
                {
                    billingViewModel.Billings = billingsDto.Billings.Select(x => new RiskBillingsViewModel
                    {
                        Id = x.Id,
                        Amount = Math.Round(x.Amount, 2),
                        Discount = Math.Round(x.Discount.HasValue ? x.Discount.Value : default(double), 2),
                        BillingTypeId = x.BillingTypeId,
                        BillingTypeDesc = x.BillingTypeDesc,
                        ClaimId = x.ClaimId,
                        SubTotal = Math.Round(x.SubTotal, 2)

                    }).ToList();

                    var billing = billingsDto.Billings.Where(x => x.BillingTypeId == (int)Constants.BillingTypes.AdminChange).FirstOrDefault();
                    if (billing != null)
                    {
                        billingViewModel.AdminFee = billing.Discount.HasValue ? billing.Amount * (1 - billing.Discount.Value / 100) : billing.Amount;

                    }

                }

            }

            return billingViewModel;
        }

        internal static WriteOffViewModel GetWriteOffViewModel(WriteOffDto writeOffsDto, bool isEdit)
        {
            WriteOffViewModel writeOffViewModel = new WriteOffViewModel();

            if (isEdit)
            {
                writeOffViewModel.WriteOffTypes = LookUpHelpers.GetAllWriteOffTypes();
            }

            if (writeOffsDto != null)
            {
                writeOffViewModel.TotalWriteOff = (writeOffsDto.TotalWriteOff.HasValue) ? Math.Round(writeOffsDto.TotalWriteOff.Value, 2) : default(double);
                writeOffViewModel.TotalDue = (writeOffsDto.TotalDue.HasValue) ? Math.Round(writeOffsDto.TotalDue.Value, 2) : default(double);
                writeOffViewModel.ClaimId = writeOffsDto.ClaimId;
                writeOffViewModel.WriteOffs = writeOffsDto.WriteOffInfo.Select(x => new WriteOffInfoViewModel()
                {
                    WriteOffId = x.WriteOffId,
                    Amount = x.Amount,
                    WriteOffType = x.WriteOffType,
                    WriteOffTypeId = x.WriteOffTypeId,
                    ClaimId = x.ClaimId
                });
            }

            return writeOffViewModel;
        }

        internal static SalvageViewModel GetSalvageInfo(SalvageDto salvageDto)
        {
            SalvageViewModel salvageViewModel = new SalvageViewModel();

            salvageViewModel.SalvageAmount = salvageDto.Amount;
            salvageViewModel.SalvageReceiptDate = salvageDto.Date;
            salvageViewModel.ClaimId = salvageDto.ClaimId;

            return salvageViewModel;
        }


        internal static ClaimDto GetClaimDto(CreateClaimViewModel objCreateClaim)
        {
            ClaimDto claimToReturn = null;

            if (objCreateClaim != null)
            {
                claimToReturn = new ClaimDto();
                claimToReturn.SelectedOpenLocationId = objCreateClaim.SelectedLocation;
                claimToReturn.SelectedLossTypeId = objCreateClaim.SelectedLossType;
                claimToReturn.DateOfLoss = objCreateClaim.DateOfLoss;
                claimToReturn.ContractNo = objCreateClaim.ContractNo;
                claimToReturn.SelectedAssignedUserId = objCreateClaim.SelectedAssignedUser;
                claimToReturn.UnitNumber = objCreateClaim.SelectedUnitNumber;

            }



            return claimToReturn;

        }

        internal static ClaimDto GetClaimDtoOfHeader(ClaimInfoHeaderViewModel objCreateClaim)
        {
            ClaimDto claimToReturn = null;
            if (objCreateClaim != null)
            {
                claimToReturn = new ClaimDto();

                claimToReturn.Id = objCreateClaim.ClaimID;
                claimToReturn.SelectedStatusId = objCreateClaim.SelectedStatusId;
                claimToReturn.SelectedAssignedUserId = objCreateClaim.SelectedAssignedUserId;

                claimToReturn.FollowUpdate = objCreateClaim.FollowupDate;
                claimToReturn.ApproverId = objCreateClaim.SelectedApprover;
            }

            return claimToReturn;

        }

        internal static ClaimDto GetClaimInfoDto(ClaimInfoForViewEditViewModel claimViewModel)
        {
            ClaimDto claimToReturn = null;
            if (claimViewModel != null)
            {
                claimToReturn = new ClaimDto();

                claimToReturn.Id = claimViewModel.ClaimID;
                claimToReturn.OpenDate = claimViewModel.OpenDate;
                claimToReturn.CloseDate = claimViewModel.CloseDate;
                claimToReturn.EstReturnDate = claimViewModel.EstReturnDate;
                claimToReturn.SelectedOpenLocationId = claimViewModel.SelectedOpenLocationId;
                claimToReturn.SelectedCloseLocationId = claimViewModel.SelectedCloseLocationId;
                claimToReturn.SelectedLossTypeId = claimViewModel.SelectedLossTypeId;
                claimToReturn.LabourHour = claimViewModel.LabourHour;
                claimToReturn.IsCollectable = claimViewModel.IsCollectable;
                claimToReturn.HasContract = claimViewModel.HasContract;
            }

            return claimToReturn;
        }

        internal static ContractDto GetContractInfoDto(ContractInfoViewModel contractViewModel)
        {

            ContractDto contractDto = null;

            if (contractViewModel != null)
            {
                contractDto = new ContractDto();
                contractDto.Id = contractViewModel.Id;
                contractDto.ContractNumber = contractViewModel.ContractNumber;
                contractDto.PickupDate = contractViewModel.PickupDate;
                contractDto.ReturnDate = contractViewModel.ReturnDate;
                contractDto.DaysOut = contractViewModel.DaysOut;
                contractDto.Miles = contractViewModel.Miles;
                contractDto.DailyRate = contractViewModel.DailyRate;
                contractDto.WeeklyRate = contractViewModel.WeeklyRate;
                contractDto.SLI = contractViewModel.SLI;
                contractDto.CDW = contractViewModel.CDW;
                contractDto.CDWVoided = contractViewModel.CDWVoided;
                contractDto.LDWVoided = contractViewModel.LDWVoided;

                //contract.CDWVoid = contractInfo.CDWVoided;
                contractDto.SLI = contractViewModel.SLI;
                contractDto.LPC = contractViewModel.LPC;
                contractDto.LPC2 = contractViewModel.LPC2;
                contractDto.GARS = contractViewModel.GARS;
                contractDto.LDW = contractViewModel.LDW;
                contractDto.CardNumber = contractViewModel.CardNumber;
                contractDto.CardType = contractViewModel.CardType;
                contractDto.CardExpDate = contractViewModel.CardExpDate;

            }

            return contractDto;

        }


        internal static IEnumerable<NotesViewModel> GetNotesViewModel(IEnumerable<NotesDto> notesDto)
        {
            List<NotesViewModel> noteViewModelList = new List<NotesViewModel>();

            NotesViewModel noteViewModel = null;

            foreach (NotesDto note in notesDto)
            {
                noteViewModel = new NotesViewModel();
                noteViewModel.ClaimId = note.ClaimId;
                noteViewModel.Date = (DateTime)note.Date;
                noteViewModel.Description = note.Description;
                noteViewModel.UpdatedBy = note.UpdatedBy;
                noteViewModel.SelectedNoteType = note.NoteTypeDescription;
                noteViewModel.IsPrivilege = note.IsPrivilege;
                noteViewModel.NoteId = note.NoteId;

                noteViewModelList.Add(noteViewModel);
            }

            return noteViewModelList;
        }


        internal static VehicleDto GetVehicleInfoDto(VehicleInfoViewModel vehicleInfoViewModel)
        {
            VehicleDto vehicleDto = null;
            if (vehicleInfoViewModel != null)
            {
                vehicleDto = new VehicleDto();
                vehicleDto.Id = vehicleInfoViewModel.Id;
                vehicleDto.UnitNumber = vehicleInfoViewModel.UnitNumber;
                vehicleDto.TagNumber = vehicleInfoViewModel.TagNumber;
                vehicleDto.TagExpires = vehicleInfoViewModel.TagExpires;
                vehicleDto.Description = vehicleInfoViewModel.Description;
                vehicleDto.Make = vehicleInfoViewModel.Make;
                vehicleDto.Model = vehicleInfoViewModel.Model;
                vehicleDto.Year = vehicleInfoViewModel.Year;
                vehicleDto.VIN = vehicleInfoViewModel.VIN;
                vehicleDto.Location = vehicleInfoViewModel.Location;
                vehicleDto.Color = vehicleInfoViewModel.Color;
                vehicleDto.Status = vehicleInfoViewModel.Status;
                vehicleDto.Mileage = vehicleInfoViewModel.Mileage;
            }

            return vehicleDto;


        }



        internal static IncidentDto GetIncidentInfoDto(IncidentInfoViewModel incidentInfoViewModel)
        {
            IncidentDto incidentDto = null;

            if (incidentInfoViewModel != null)
            {
                incidentDto = new IncidentDto();
                incidentDto.Id = incidentInfoViewModel.Id;
                incidentDto.LossDate = incidentInfoViewModel.LossDate;
                incidentDto.SelectedLocationId = incidentInfoViewModel.SelectedLocationId;
                incidentDto.SelectedPoliceAgencyId = incidentInfoViewModel.SelectedPoliceAgencyId;
                incidentDto.CaseNumber = incidentInfoViewModel.CaseNumber;
                incidentDto.RenterFault = incidentInfoViewModel.RenterFault;
                incidentDto.ThirdPartyFault = incidentInfoViewModel.ThirdPartyFault;
                incidentDto.ReportedDate = incidentInfoViewModel.ReportedDate;
            }

            return incidentDto;
        }


        internal static IEnumerable<DamageViewModel> GetDamagesViewModel(IEnumerable<DamageDto> damageList)
        {
            List<DamageViewModel> damageViewModelList = new List<DamageViewModel>();

            DamageViewModel damageViewModel = null;

            foreach (DamageDto damage in damageList)
            {
                damageViewModel = new DamageViewModel();
                damageViewModel.SelectedSection = damage.Section;
                damageViewModel.Details = damage.Details;
                //damageViewModel.SectionId = damage.SectionId;
                damageViewModel.DamageId = damage.DamageId;

                damageViewModelList.Add(damageViewModel);
            }

            return damageViewModelList.ToList();
        }


        internal static DownloadContractInfoViewModel GetDownloadContractInfoViewModel(Int64 claimId, Int64 contractId, string contractNumber)
        {
            DownloadContractInfoViewModel downloadContractInfoViewModel = new DownloadContractInfoViewModel();
            downloadContractInfoViewModel.ClaimId = claimId;
            downloadContractInfoViewModel.ContractNumber = contractNumber;
            downloadContractInfoViewModel.ContractId = contractId;

            return downloadContractInfoViewModel;

        }

        internal static DownloadVehicleInfoViewModel GetDownloadVehicleInfoViewModel(Int64 claimId, Int64 vehicleId, string contractNumber, string unitNumber)
        {
            DownloadVehicleInfoViewModel downloadVehicleInfoViewModel = new DownloadVehicleInfoViewModel();
            downloadVehicleInfoViewModel.ClaimID = claimId;
            downloadVehicleInfoViewModel.UnitNumber = unitNumber;
            downloadVehicleInfoViewModel.ContractNumber = contractNumber;
            downloadVehicleInfoViewModel.VehicleId = vehicleId;

            return downloadVehicleInfoViewModel;
        }

        internal static DriverInfoDto GetDriverAndInsuranceInfoDto(DriverAndInsuranceViewModel driverIncViewModel)
        {
            DriverInfoDto driverIncDto = null;
            if (driverIncViewModel != null)
            {
                driverIncDto = new DriverInfoDto();

                driverIncDto.Id = driverIncViewModel.DriverId;
                driverIncDto.FirstName = driverIncViewModel.FirstName;
                driverIncDto.LastName = driverIncViewModel.LastName;
                driverIncDto.Address1 = driverIncViewModel.Address1;
                driverIncDto.Address2 = driverIncViewModel.Address2;
                driverIncDto.State = driverIncViewModel.State;
                driverIncDto.City = driverIncViewModel.City;
                driverIncDto.Zip = driverIncViewModel.Zip;
                driverIncDto.Phone1 = driverIncViewModel.Phone1;
                driverIncDto.Phone2 = driverIncViewModel.Phone2;
                driverIncDto.Fax = driverIncViewModel.Fax;
                driverIncDto.OtherContact = driverIncViewModel.OtherContact;
                driverIncDto.Email = driverIncViewModel.Email;
                driverIncDto.LicenceNumber = driverIncViewModel.LicenceNumber;
                driverIncDto.DOB = driverIncViewModel.DOB;
                driverIncDto.LicenceExpiry = driverIncViewModel.LicenceExpiry;
                driverIncDto.LicenceState = driverIncViewModel.LicenceState;
                driverIncDto.IsAuthorizedDriver = driverIncViewModel.IsAuthorizedDriver;
                driverIncDto.DriverTypeId = driverIncViewModel.DriverType;
                // driverToAdd.InsuranceCompany = LookUpHelpers.GetSelectListItems(info.InsuranceCompany);

                driverIncDto.InsuranceId = driverIncViewModel.InsuranceCompanyId;
                driverIncDto.InsuranceCompanyName = driverIncViewModel.InsuranceCompanyName;
                driverIncDto.PolicyNumber = driverIncViewModel.PolicyNumber;
                driverIncDto.Deductible = driverIncViewModel.Deductible;
                driverIncDto.InsuranceClaimNumber = driverIncViewModel.InsuranceClaimNumber;
                driverIncDto.InsuranceExpiry = driverIncViewModel.InsuranceExpiry;
                driverIncDto.CreditCardCompany = driverIncViewModel.CreditCardCompany;
                driverIncDto.CreditCardPolicyNumber = driverIncViewModel.CreditCardPolicyNumber;
                driverIncDto.CreditCardCoverageAmount = driverIncViewModel.CreditCardCoverageAmt;
            }
            return driverIncDto;
        }

        internal static DriverAndInsuranceViewModel GetEmptyDriverInsuranceViewModel(int driverType, Int64 claimId)
        {
            DriverAndInsuranceViewModel driverAndInsuranceViewModel = new DriverAndInsuranceViewModel();

            driverAndInsuranceViewModel.DriverType = driverType;
            driverAndInsuranceViewModel.ClaimId = claimId;
            driverAndInsuranceViewModel.InsuranceCompanies = LookUpHelpers.GetAllInsuranceComapanyListItems();

            return driverAndInsuranceViewModel;
        }

        internal static IncidentInfoViewModel GetListsInIncidentViewModel(IncidentInfoViewModel incidentInfoViewModel)
        {
            incidentInfoViewModel.Locations = LookUpHelpers.GetLocationListItem();

            incidentInfoViewModel.PoliceAgencies = LookUpHelpers.GetAllPoliceAgenciesListItem();

            return incidentInfoViewModel;
        }

        internal static RiskBillingDto GetBillingsDto(BillingViewModel billingViewModel)
        {
            RiskBillingDto billingDto = null;
            if (billingViewModel != null)
            {
                billingDto = new RiskBillingDto();

                billingDto.BillingTypeId = billingViewModel.SelectedBillingTypeId;
                billingDto.ClaimId = billingViewModel.ClaimId;
                billingDto.AutoAdminFeeCalculate = billingViewModel.AutoAdminFeeCalculate;
                billingDto.Amount = billingViewModel.Amount.HasValue ? billingViewModel.Amount.Value : default(double);
                billingDto.Discount = billingViewModel.Discount.HasValue ? billingViewModel.Discount.Value : default(double);

            }

            return billingDto;
        }

        internal static ClaimInfoForViewEditViewModel GetListsInClaimViewModel(ClaimInfoForViewEditViewModel claimViewModel)
        {
            claimViewModel.OpenLocations = LookUpHelpers.GetLocationListItem();
            claimViewModel.CloseLocations = LookUpHelpers.GetLocationListItem();
            claimViewModel.LossTypes = LookUpHelpers.GetLossTypeListItems();
            return claimViewModel;

        }

        internal static ClaimInfoForViewEditViewModel GetClaimInfoForViewEditViewModel(ClaimDto claimInfo, bool isEditView)
        {
            ClaimInfoForViewEditViewModel claimInfoToAdd = new ClaimInfoForViewEditViewModel();

            if (claimInfo != null)
            {

                claimInfoToAdd.ClaimID = claimInfo.Id;
                if (isEditView)
                {
                    claimInfoToAdd.CloseLocations = LookUpHelpers.GetLocationListItem();
                    claimInfoToAdd.OpenLocations = LookUpHelpers.GetLocationListItem();
                    claimInfoToAdd.LossTypes = LookUpHelpers.GetLossTypeListItems();
                }

                claimInfoToAdd.OpenDate = claimInfo.OpenDate;
                claimInfoToAdd.CloseDate = claimInfo.CloseDate;
                claimInfoToAdd.EstReturnDate = claimInfo.EstReturnDate;
                claimInfoToAdd.LocationName = claimInfo.SelectedOpenLocationName;
                claimInfoToAdd.SelectedOpenLocationId = claimInfo.SelectedOpenLocationId;
                claimInfoToAdd.SelectedCloseLocationId = claimInfo.SelectedCloseLocationId;
                claimInfoToAdd.SelectedLossTypeId = claimInfo.SelectedLossTypeId;
                claimInfoToAdd.LossTypeDesc = claimInfo.SelectedLossTypeDescription;
                claimInfoToAdd.LabourHour = claimInfo.LabourHour;
                claimInfoToAdd.IsCollectable = claimInfo.IsCollectable;
                claimInfoToAdd.HasContract = claimInfo.HasContract;
                claimInfoToAdd.DisableHasContract = claimInfo.DisableHasContract;

            }
            return claimInfoToAdd;
        }

        internal static ClaimInfoHeaderViewModel GetClaimInfoHeaderViewModel(ClaimDto claimInfo, IEnumerable<UserDto> users, IEnumerable<UserDto> approvalUsers)
        {
            ClaimInfoHeaderViewModel claimInfoToAdd = new ClaimInfoHeaderViewModel();

            if (claimInfo != null)
            {

                claimInfoToAdd.ClaimID = claimInfo.Id;


                claimInfoToAdd.Status = LookUpHelpers.GetClaimStatusListItems();

                if (users != null)
                    claimInfoToAdd.Users = LookUpHelpers.GetSelectListItems(users);

                if (approvalUsers != null)
                    claimInfoToAdd.ApprovalUsers = LookUpHelpers.GetSelectListItems(approvalUsers);

                claimInfoToAdd.SelectedStatusId = claimInfo.SelectedStatusId;
                claimInfoToAdd.SelectedAssignedUserId = claimInfo.SelectedAssignedUserId;
                claimInfoToAdd.FollowupDate = claimInfo.FollowUpdate;
                claimInfoToAdd.CompanyAbbr = claimInfo.CompanyAbbr != null ? claimInfo.CompanyAbbr.ToUpper() : String.Empty;
                claimInfoToAdd.ContractNumber = claimInfo.ContractNo;
                claimInfoToAdd.SelectedApprover = claimInfo.ApproverId ?? default(Int64);
                claimInfoToAdd.TotalDue = claimInfo.TotalDue.HasValue ? claimInfo.TotalDue.Value : default(double);
                claimInfoToAdd.IsCollectable = claimInfo.IsCollectable;

                var loggedInUser = SecurityHelper.GetUserIdFromContext();
                if (claimInfo.ApproverId == loggedInUser && claimInfo.ApprovalStatus == null)
                {
                    claimInfoToAdd.HasAccessToApprove = true;
                }
            }
            return claimInfoToAdd;
        }


        internal static ClaimInfoHeaderViewModel GetClaimInfoHeaderViewModel(ClaimInfoHeaderViewModel claimInfo, IEnumerable<UserDto> users, IEnumerable<UserDto> approvalUsers)
        {

            claimInfo.Status = LookUpHelpers.GetClaimStatusListItems();

            if (users != null)
                claimInfo.Users = LookUpHelpers.GetSelectListItems(users);

            if (approvalUsers != null)
                claimInfo.ApprovalUsers = LookUpHelpers.GetSelectListItems(approvalUsers);

            return claimInfo;
        }

        internal static string GetDriverTypeText(int? driverType)
        {
            string driverTypeText = String.Empty;
            if (driverType.HasValue)
            {
                ClaimsConstant.DriverTypes driverTypeEnum = (ClaimsConstant.DriverTypes)driverType.Value;

                switch (driverTypeEnum)
                {
                    case ClaimsConstant.DriverTypes.Primary:
                        driverTypeText = "Primary";
                        break;
                    case ClaimsConstant.DriverTypes.Additional:
                        driverTypeText = "Additional";
                        break;
                    case ClaimsConstant.DriverTypes.ThirdParty:
                        driverTypeText = "Third Party";
                        break;
                    default:
                        break;
                }

            }
            return driverTypeText;
        }


        internal static NonContractInfoViewModel GetNonContractInfoViewModel(NonContractDto nonContractDto, long claimId)
        {
            NonContractInfoViewModel nonContractInfoViewModel = new NonContractInfoViewModel();
            if (nonContractDto != null)
            {
                nonContractInfoViewModel.Id = nonContractDto.Id;

                nonContractInfoViewModel.NonContractNumber = nonContractDto.NonContractNumber;
            }
            nonContractInfoViewModel.ClaimId = claimId;
            return nonContractInfoViewModel;
        }

        internal static NonContractDto GetNonContractInfoDto(NonContractInfoViewModel claimViewModel)
        {
            NonContractDto nonContractDto = new NonContractDto();
            if (claimViewModel != null)
            {
                nonContractDto.Id = claimViewModel.Id;
                nonContractDto.NonContractNumber = claimViewModel.NonContractNumber;
            }

            return nonContractDto;
        }
    }
}