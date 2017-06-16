using EZRAC.Risk.UI.Web.ViewModels.ClaimBasicInfo;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Risk.UI.Web.ViewModels.Reports;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class ReportHelper
    {
        internal static IEnumerable<CommonViewModel> MappedCommonViewModel(dynamic CustomDtos)
        {
            CommonViewModel commonViewModel;
            List<CommonViewModel> objViewModel = new List<CommonViewModel>();
            foreach (var item in CustomDtos.Result)
            {
                commonViewModel = new CommonViewModel();
                commonViewModel.Claim = item.Claim;
                commonViewModel.Contract = item.Contract;
                commonViewModel.ClaimContract = item.ClaimContract;
                commonViewModel.TagNumber = item.TagNumber;
                commonViewModel.UnitNumber = item.UnitNumber;
                commonViewModel.UnitDetails = item.UnitDetails;
                commonViewModel.Location = item.Location;
                commonViewModel.User_Agent = item.User_Agent;
                commonViewModel.Date = item.Date;
                objViewModel.Add(commonViewModel);
            }
            return objViewModel;
        }
        internal static ReportCriteriaDto MappedReportCriteria(ReportCriteriaViewModel reportCriteriaViewModel)
        {
            ReportCriteriaDto reportCriteriaDto = new ReportCriteriaDto();

            reportCriteriaDto.ReportKey = reportCriteriaViewModel.ReportKey;
            reportCriteriaDto.ReportText = reportCriteriaViewModel.ReportText;
            reportCriteriaDto.TagNumber = reportCriteriaViewModel.TagNumber;
            reportCriteriaDto.DateTypeKey = reportCriteriaViewModel.DateTypeKey;
            reportCriteriaDto.Status = reportCriteriaViewModel.Status;
            reportCriteriaDto.TicketOrViolations = reportCriteriaViewModel.TicketOrViolations;
            reportCriteriaDto.GreaterThanLessThan = reportCriteriaViewModel.GreaterThanLessThan;
            reportCriteriaDto.GreaterThanLessThanValue = reportCriteriaViewModel.GreaterThanLessThanValue;
            reportCriteriaDto.FromDate = reportCriteriaViewModel.FromDate;
            reportCriteriaDto.ToDate = reportCriteriaViewModel.ToDate;
            reportCriteriaDto.AsOfDate = reportCriteriaViewModel.AsOfDate;
            reportCriteriaDto.IncludeCollection = reportCriteriaViewModel.IncludeCollection;
            reportCriteriaDto.IncludeTicket = reportCriteriaViewModel.IncludeTicket;
            reportCriteriaDto.PaymentTypeId = (reportCriteriaViewModel.PaymentTypeId != null) ? string.Join(",", reportCriteriaViewModel.PaymentTypeId) : string.Empty;
            reportCriteriaDto.ReportTypeKey = reportCriteriaViewModel.ReportTypeKey;
            reportCriteriaDto.LossTypeIds = (reportCriteriaViewModel.LossTypeIds != null) ? string.Join(",", reportCriteriaViewModel.LossTypeIds) : string.Empty;
            reportCriteriaDto.AgentId = reportCriteriaViewModel.AgentId;
            reportCriteriaDto.ChargeTypeId = reportCriteriaViewModel.ChargeTypeId;
            reportCriteriaDto.ClaimStatusIds = (reportCriteriaViewModel.ClaimStatusIds != null) ? string.Join(",", reportCriteriaViewModel.ClaimStatusIds) : string.Empty;
            reportCriteriaDto.LocationIds = (reportCriteriaViewModel.LocationIds != null) ? string.Join(",", reportCriteriaViewModel.LocationIds) : string.Empty;
            reportCriteriaDto.UserIds = (reportCriteriaViewModel.UserIds != null) ? string.Join(",", reportCriteriaViewModel.UserIds) : string.Empty;
            reportCriteriaDto.VehicalDamageIds = (reportCriteriaViewModel.VehicalDamageIds != null) ? string.Join(",", reportCriteriaViewModel.VehicalDamageIds) : string.Empty;
            reportCriteriaDto.AgingDays = reportCriteriaViewModel.AgingDays;

            return reportCriteriaDto;
        }
        internal static AdminReportViewModel MappedAdminReportViewModel(ReportAdminDto reportAdminDto, string ReportTypeKey)
        {
            AdminReportViewModel adminReportViewModel = new AdminReportViewModel();
            adminReportViewModel.ReportTypeKey = ReportTypeKey;
            if (ReportTypeKey == "useractivity")
            {
                adminReportViewModel.UserActionLogViewModel = null;
                adminReportViewModel.UserActionLogViewModel = MappedUserActionViewModel(reportAdminDto.UserActionLogDtos);
            }
            else if (ReportTypeKey == "pendingamountdue")
            {
                adminReportViewModel.PaymentViewModels = null;
                adminReportViewModel.PaymentViewModels = MappedReportPaymentViewModel(reportAdminDto.ReportPaymentDtos);
            }
            else if (ReportTypeKey == "calldates" || ReportTypeKey == "assignedcontracts" || ReportTypeKey == "customerservice")
            {
                IEnumerable<ClaimViewModel> ReportClaimViewModels = MappedClaimViewModel(reportAdminDto.ReportClaimDtos).ToList();
                adminReportViewModel.ReportClaimViewModels = ReportClaimViewModels;
            }
            adminReportViewModel.ReportDate = DateTime.Now;
            return adminReportViewModel;
        }
        internal static IEnumerable<PaymentViewModel> MappedReportPaymentViewModel(IEnumerable<ReportPaymentDto> reportPaymentDto)
        {
            IEnumerable<PaymentViewModel> paymentViewModel = null;
            if (reportPaymentDto != null)
            {
                paymentViewModel = reportPaymentDto.Select(x => new PaymentViewModel
                {
                    Charges = Convert.ToString(x.Charges),
                    Payments = Convert.ToString(x.Payments),
                    Estimated = Convert.ToString(x.Estimated),
                    OtherChanrges = Convert.ToString(x.OtherChanrges),
                    AdminFee = x.AdminFee,
                    DiminFee = x.DiminFee,
                    AppFee = x.AppFee,
                    PayForm = x.PayForm,
                    PaidDate = x.PaidDate,
                    CheckAmount = x.CheckAmount,
                    Billed = x.Billed,
                    Balance = Convert.ToString(x.Balance),
                    Location = x.Location,
                    Contract = Convert.ToString((x.Contract != null) ? Convert.ToString(x.Contract) : string.Empty),
                    Claim = Convert.ToString(x.Claim),
                    CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                });
            }
            return paymentViewModel;
        }
        internal static IEnumerable<ClaimViewModel> MappedClaimViewModel(IEnumerable<ReportClaimDto> reportClaimDtos)
        {
            IEnumerable<ClaimViewModel> claimViewModel = null;
            if (reportClaimDtos != null)
            {
                claimViewModel = reportClaimDtos.Select(x => new ClaimViewModel
                {
                    Claim = Convert.ToString(x.Claim),
                    Contract = Convert.ToString((x.Contract != null) ? Convert.ToString(x.Contract) : string.Empty),
                    Status = x.Status,
                    OpeningAgent = x.OpeningAgent,
                    ClosedReason = x.ClosedReason,
                    OpenDate = Convert.ToString((x.OpenDate != null) ? Convert.ToDateTime(x.OpenDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                    FollowUpDate = Convert.ToString((x.FollowUpDate != null) ? Convert.ToDateTime(x.FollowUpDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                    CloseDate = Convert.ToString((x.CloseDate != null) ? Convert.ToDateTime(x.CloseDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                    DriverName = x.DriverName,
                    UserDetail = x.UserDetail,
                    CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                }).ToList();
            }
            return claimViewModel;
        }
        internal static IEnumerable<UserActionLogViewModel> MappedUserActionViewModel(IEnumerable<UserActionLogDto> userActionLogDto)
        {
            IEnumerable<UserActionLogViewModel> userActionLogViewModel = null;
            if (userActionLogDto != null)
            {
                userActionLogViewModel = userActionLogDto.Select(x => new UserActionLogViewModel
                {
                    UserName = x.UserName,
                    ContractNo = Convert.ToString(x.ContractNo),
                    Name = x.Name,
                    ClaimId = Convert.ToString(x.ClaimId),
                    Date = Convert.ToString(x.Date.ToString(Constants.CrmsDateFormates.MMDDYYYY)),
                    UserAction = x.UserAction,
                    CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                });
            }
            return userActionLogViewModel;
        }
        internal static IEnumerable<VehicleToBeReleasedViewModel> MappedVahicleToBeReleasedViewModel(IEnumerable<VehicleToBeReleasedReportDto> vehicleToBeReleasedDto)
        {
            IEnumerable<VehicleToBeReleasedViewModel> vehicleToBeReleasedViewModel = null;
            if (vehicleToBeReleasedDto != null)
            {
                vehicleToBeReleasedViewModel = vehicleToBeReleasedDto.Select(x => new VehicleToBeReleasedViewModel
                {
                    Claim = Convert.ToString(x.Claim),
                    ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                    UnitDetails = x.UnitDetails,
                    ClosedDate = Convert.ToString(x.ClosedDate),
                    LossDate = Convert.ToString(x.LossDate.ToString(Constants.CrmsDateFormates.MMDDYYYY)),
                    LossType = x.LossType,
                    OpenDate = Convert.ToString(x.OpenDate.ToString(Constants.CrmsDateFormates.MMDDYYYY)),
                    Status = x.Status,
                    RiskAgent = x.RiskAgent,
                    CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                });
            }
            return vehicleToBeReleasedViewModel;
        }
        internal static IEnumerable<WriteOffCustomReportViewModel> MappedWriteOffCustomReportViewModel(IEnumerable<WriteOffCustomReportDto> writeOffCustomReportDto)
        {
            IEnumerable<WriteOffCustomReportViewModel> writeOffCustomReportViewModel = null;
            if (writeOffCustomReportDto != null)
            {
                writeOffCustomReportViewModel = writeOffCustomReportDto.Select(item => new WriteOffCustomReportViewModel()
                {
                    ReportDate = DateTime.Now,
                    LocationId = item.LocationId,
                    Location = item.Location,
                    writeOffReportViewModel = item.writeOffReportDto.Select(x => new WriteOffReportViewModel()
                    {
                        Claim = x.Claim,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        UnitNumber = x.UnitNumber,
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        ClosedDate = Convert.ToString((x.ClosedDate != null) ? Convert.ToDateTime(x.ClosedDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                        TotalCollected = Convert.ToString(x.TotalCollected),
                        TotalDue = Convert.ToString(x.TotalDue),
                        ClosedReason = x.ClosedReason,
                        Renter = x.Renter,
                        CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                    }),
                    LocationTotal = item.LocationTotal,
                    FinalRenterTotal = Convert.ToString(item.FinalRenterTotal),
                    FinalTotalCollected = Convert.ToString(item.FinalTotalCollected),
                    FinalTotalDue = Convert.ToString(item.FinalTotalDue),
                    FinalTotalClaims = Convert.ToString(item.FinalTotalClaims)
                });
            }
            return writeOffCustomReportViewModel;
        }
        internal static IEnumerable<AccountReceivableReportViewModel> MappedAccountReceivableReportViewModel(IEnumerable<AccountReceivableCustomReportDto> accountReceivableCustomReportDto)
        {
            IEnumerable<AccountReceivableReportViewModel> accountReceivableReportViewModel = null;
            if (accountReceivableCustomReportDto != null)
            {
                accountReceivableReportViewModel = accountReceivableCustomReportDto.Select(item => new AccountReceivableReportViewModel()
                {
                    ReportDate = DateTime.Now,
                    LocationId = item.LocationId,
                    Location = item.Location,
                    AccountReceivableViewModel = item.accountReceivableReportDto.Select(x => new AccountReceivableViewModel()
                    {
                        Claim = x.Claim,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        TotalCollected = Convert.ToString(x.TotalCollected),
                        TotalDue = Convert.ToString(x.TotalDue),
                        Renter = x.Renter,
                        CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                    }),
                    LocationTotal = item.LocationTotal,
                    FinalRenterTotal = Convert.ToString(item.FinalRenterTotal),
                    FinalTotalCollected = Convert.ToString(item.FinalTotalCollected),
                    FinalTotalDue = Convert.ToString(item.FinalTotalDue),
                    FinalTotalClaims = Convert.ToString(item.FinalTotalClaims)
                });
            }
            return accountReceivableReportViewModel;
        }
        internal static IEnumerable<ARAgingCustomReportViewModel> MappedARAgingCustomReportViewModel(IEnumerable<ARAgingCustomReportDto> aRAgingCustomReportDto)
        {
            IEnumerable<ARAgingCustomReportViewModel> aRAgingCustomReportViewModel = null;
            if (aRAgingCustomReportDto != null)
            {
                aRAgingCustomReportViewModel = aRAgingCustomReportDto.Select(item => new ARAgingCustomReportViewModel()
                {
                    ReportDate = DateTime.Now,
                    LocationId = item.LocationId,
                    Location = item.Location,
                    ARAgingReportViewModel = item.aRAgingReportDto.Select(x => new ARAgingReportViewModel()
                    {
                        Claim = x.Claim,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        UnitNumber = x.UnitNumber,
                        DateOfLoss = Convert.ToString(x.DateofLoss),
                        TotalCollected = Convert.ToString(x.TotalCollected),
                        TotalDue = Convert.ToString(x.TotalDue),
                        TotalBilled = Convert.ToString(x.TotalBilled),
                        Renter = x.Renter,
                        NoOfDays = x.NoOfDays,
                        CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty),
                        AssignedTo = x.AssignedTo
                    }),
                    TotalClaims = item.TotalClaims,
                    TotalReceivables = item.TotalReceivables
                });


            }
            return aRAgingCustomReportViewModel;
        }
        internal static IEnumerable<ChargebackLossCustomReportViewModel> MappedChargeBackLossCustomReportViewModel(IEnumerable<ChargeBackLossCustomReportDto> chargeBackLossCustomReportDto)
        {
            IEnumerable<ChargebackLossCustomReportViewModel> chargeBackLossCustomReportViewModel = null;
            if (chargeBackLossCustomReportDto != null)
            {
                chargeBackLossCustomReportViewModel = chargeBackLossCustomReportDto.Select(item => new ChargebackLossCustomReportViewModel()
                {
                    ReportDate = DateTime.Now,
                    LocationId = item.LocationId,
                    Location = item.Location,
                    chargeBackLossReportViewModel = item.chargeBackLossReportDto.Select(x => new ChargebackLossReportViewModel()
                    {
                        Claim = x.Claim,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        ClosedDate = Convert.ToString((x.ClosedDate != null) ? Convert.ToDateTime(x.ClosedDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                        TotalCollected = Convert.ToString(x.TotalCollected),
                        TotalDue = Convert.ToString(x.TotalDue),
                        ClosedReason = x.ClosedReason,
                        Renter = x.Renter,
                        AgentName = x.AgentName,
                        OpeningAgent = Convert.ToString((x.OpeningAgent != null) ? x.OpeningAgent : String.Empty),
                        CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                    }),
                    LocationTotal = item.LocationTotal,
                    FinalRenterTotal = Convert.ToString(item.FinalRenterTotal),
                    FinalTotalCollected = Convert.ToString(item.FinalTotalCollected),
                    FinalTotalDue = Convert.ToString(item.FinalTotalDue),
                    FinalTotalClaims = item.FinalTotalClaims
                });
                 chargeBackLossCustomReportViewModel.ToList().SelectMany(x => x.chargeBackLossReportViewModel);
            }
            return chargeBackLossCustomReportViewModel;
        }
        internal static IEnumerable<VehicleDamageSectionCustomReportViewModel> MappedVehicleDamageSectionCustomReportViewModel(IEnumerable<VehicleDamageSectionCustomReportDto> vehicleDamageSectionCustomReportDto)
        {
            IEnumerable<VehicleDamageSectionCustomReportViewModel> vehicleDamageSectionCustomReportViewModel = null;
            if (vehicleDamageSectionCustomReportDto != null)
            {
                vehicleDamageSectionCustomReportViewModel = vehicleDamageSectionCustomReportDto.Select(item => new VehicleDamageSectionCustomReportViewModel()
                {
                    ReportDate = DateTime.Now,
                    LocationId = item.LocationId,
                    Location = item.Location,
                    VehicleDamageSectionReportViewModel = item.vehicleDamageSectionReportDto.Select(x => new VehicleDamageSectionReportViewModel()
                    {
                        Claim = x.Claim,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        OpenDate = Convert.ToString((x.OpenDate != null) ? Convert.ToDateTime(x.OpenDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                        TotalCollected = Convert.ToString(x.TotalCollected),
                        TotalDue = Convert.ToString(x.TotalDue),
                        Status = x.Status,
                        Renter = x.Renter,
                        UnitNumber = x.UnitNumber,
                        VehicleSection = x.VehicleSection,
                        CompanyAbbr = Convert.ToString((x.CompanyAbbr != null) ? x.CompanyAbbr : string.Empty)
                    }),
                    LocationTotal = item.LocationTotal,
                    FinalRenterTotal = item.FinalRenterTotal,
                    FinalTotalCollected = Convert.ToString(item.FinalTotalCollected),
                    FinalTotalDue = Convert.ToString(item.FinalTotalDue)
                });
            }
            return vehicleDamageSectionCustomReportViewModel;
        }

        internal static IEnumerable<BasicReportViewModel> MappedBasicReportViewModel(IEnumerable<BasicReportDtos> basicReportDto)
        {
            List<BasicReportViewModel> lstBasicReportViewModel = new List<BasicReportViewModel>();
            foreach (var item in basicReportDto)
            {
                BasicReportViewModel basicReportViewModel = new BasicReportViewModel();
                basicReportViewModel.Claim = Convert.ToString((item.Claim != null) ? Convert.ToString(item.Claim) : string.Empty);
                basicReportViewModel.Contract = Convert.ToString(item.Contract);
                basicReportViewModel.NextCallDate = item.NextCallDate;
                basicReportViewModel.Location = item.Location;
                basicReportViewModel.Customer = item.Customer;
                basicReportViewModel.Status = item.Status;
                basicReportViewModel.NextCallDate = item.NextCallDate;
                basicReportViewModel.Unit = item.Unit;
                basicReportViewModel.MakeModel = item.MakeModel;
                basicReportViewModel.Tag = item.Tag;
                basicReportViewModel.Charges = Math.Round(item.Charges,2); //Math.Round(item.Select(x => x.TotalCollected).Sum(), 2),
                basicReportViewModel.Payment = Math.Round(item.Payment,2);
                basicReportViewModel.Balance = Math.Round(item.Balance, 2);
                basicReportViewModel.CompanyAbbr = Convert.ToString((item.CompanyAbbr != null) ? item.CompanyAbbr : string.Empty);
                lstBasicReportViewModel.Add(basicReportViewModel);
            }

            return lstBasicReportViewModel.ToList();
        }
        internal static IEnumerable<DepositDateReportViewModel> MappedDepositDateReportViewModel(IEnumerable<DepositDateReportDto> depositDateReportDto)
        {
            List<DepositDateReportViewModel> lstDepositDateReportViewModel = new List<DepositDateReportViewModel>();
            foreach (var item in depositDateReportDto)
            {
                DepositDateReportViewModel depositDateReportViewModel = new DepositDateReportViewModel();
                depositDateReportViewModel.ClaimId = item.ClaimId.ToString();
                depositDateReportViewModel.Claim = Convert.ToString((item.Claim != null) ? Convert.ToString(item.Claim) : string.Empty);
                depositDateReportViewModel.Unit = item.Unit;
                depositDateReportViewModel.UnitDescription = item.UnitDescr;
                depositDateReportViewModel.LossType = item.LossType;
                depositDateReportViewModel.ClaimStatus = item.ClaimStatus;
                depositDateReportViewModel.LossDate = item.LossDate;
                depositDateReportViewModel.PaidFrom = item.PaidFrom;
                depositDateReportViewModel.PaidDate = item.PaidDate;
                depositDateReportViewModel.CheckAmount =  Math.Round(item.CheckAmount,2);
                depositDateReportViewModel.Billed =  Math.Round(item.Billed,2);
                depositDateReportViewModel.Balance =  Math.Round(item.Balance,2);
                depositDateReportViewModel.CompanyAbbr = Convert.ToString((item.CompanyAbbr != null) ? item.CompanyAbbr : string.Empty);
                lstDepositDateReportViewModel.Add(depositDateReportViewModel);
            }
            return lstDepositDateReportViewModel.ToList();
        }
        internal static IEnumerable<ChargeTypeReportViewModel> MappedChargeTypeReportViewModel(IEnumerable<ChargeTypeReportDto> chargeTypeReportDto)
        {
            List<ChargeTypeReportViewModel> lstChargeTypeReportViewModel = new List<ChargeTypeReportViewModel>();
            foreach (var item in chargeTypeReportDto)
            {
                ChargeTypeReportViewModel chargeTypeReportViewModel = new ChargeTypeReportViewModel();
                chargeTypeReportViewModel.ClaimID = item.ClaimID.ToString();
                chargeTypeReportViewModel.Claim = Convert.ToString((item.Claim != null) ? Convert.ToString(item.Claim) : string.Empty);
                chargeTypeReportViewModel.Unit = item.Unit;
                chargeTypeReportViewModel.Status = item.Status;
                chargeTypeReportViewModel.LossDate = item.LossDate;
                chargeTypeReportViewModel.ChargeAmount =  Math.Round(item.ChargeAmount,2);
                chargeTypeReportViewModel.CompanyAbbr = Convert.ToString((item.CompanyAbbr != null) ? item.CompanyAbbr : string.Empty);
                lstChargeTypeReportViewModel.Add(chargeTypeReportViewModel);
            }
            return lstChargeTypeReportViewModel.ToList();
        }
        internal static IEnumerable<StolenRecoveredReportViewModel> MappedStolenRecoveredReportViewModel(IEnumerable<StolenRecoveredReportDto> stolenRecoveredReportDto)
        {
            List<StolenRecoveredReportViewModel> lstStolenRecoveredReportViewModel = new List<StolenRecoveredReportViewModel>();
            foreach (var item in stolenRecoveredReportDto)
            {
                StolenRecoveredReportViewModel stolenRecoveredReportViewModel = new StolenRecoveredReportViewModel();
                stolenRecoveredReportViewModel.ClaimID = item.ClaimID.ToString();
                stolenRecoveredReportViewModel.Claim = Convert.ToString((item.Claim != null) ? Convert.ToString(item.Claim) : string.Empty);
                stolenRecoveredReportViewModel.Unit = item.Unit;
                stolenRecoveredReportViewModel.Status = item.Status;
                stolenRecoveredReportViewModel.LossType = item.LossType;
                stolenRecoveredReportViewModel.OpenDate = item.OpenDate;
                stolenRecoveredReportViewModel.RAExpectedCloseDate = item.RAExpectedCloseDate;
                stolenRecoveredReportViewModel.ReportedToPD = item.ReportedToPD;
                stolenRecoveredReportViewModel.RiskAgent = item.RiskAgent;
                stolenRecoveredReportViewModel.CompanyAbbr = Convert.ToString((item.CompanyAbbr != null) ? item.CompanyAbbr : string.Empty);
                lstStolenRecoveredReportViewModel.Add(stolenRecoveredReportViewModel);
            }
            return lstStolenRecoveredReportViewModel.ToList();
        }
        internal static IEnumerable<CollectionCustomReportViewModel> MappedCollectionCustomReportViewModel(IEnumerable<CollectionCustomReportDto> collectionCustomReportDto)
        {
            IEnumerable<CollectionCustomReportViewModel> collectionCustomReportViewModel = null;
            if (collectionCustomReportDto != null)
            {
                collectionCustomReportViewModel = collectionCustomReportDto.Select(item => new CollectionCustomReportViewModel()
                {
                    LocationId = item.LocationId,
                    Location = item.Location,
                    collectionReportViewModel = item.collectionReportDto.Select(x => new CollectionReportViewModel()
                    {
                        ClaimId = x.ClaimId,
                        ContractNo = Convert.ToString((x.ContractNo != null) ? Convert.ToString(x.ContractNo) : string.Empty),
                        CompanyAbbr = Convert.ToString(x.CompanyAbbr),
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        Code = x.Code,
                        PayDate = Convert.ToString((x.PayDate != null) ? Convert.ToDateTime(x.PayDate).ToString(Constants.CrmsDateFormates.MMDDYYYY) : string.Empty),
                        Collected = Convert.ToString(x.Collected),
                        PayFrom = Convert.ToString(x.PayFrom),
                        Damage = Convert.ToString(x.Damage),
                        Status = Convert.ToString(x.Status),
                        Renter = Convert.ToString(x.Renter),
                        LossOfUse = Convert.ToString(x.LossOfUse),
                        AdminFee = Convert.ToString(x.AdminFee),
                        DiminFee = Convert.ToString(x.DiminFee),
                        OtherFee = Convert.ToString(x.OtherFee),
                        SubroTotal =Convert.ToString(x.SubroTotal),
                        WriteOffTotal=Convert.ToString(x.WriteOffTotal),
                        //SubroTotal = Convert.ToString(Convert.ToDecimal((x.Damage) +( x.LossOfUse) + (x.AdminFee) + (x.DiminFee) + (x.OtherFee))),
                       // SubroTotal = Convert.ToString(SumSubro(x.Damage,x.LossOfUse,x.AdminFee,x.DiminFee ,x.OtherFee)),
                        Agent = Convert.ToString(x.Agent)
                    }).ToList < CollectionReportViewModel>(),
                    LocationTotal = item.LocationTotal,
                    FinalRenterTotal = Convert.ToString(item.FinalRenterTotal),
                    FinalTotalCollected = Convert.ToString(item.FinalTotalCollected),

                    FinalTotalDamage = Convert.ToString(item.FinalTotalDamage),
                    FinalTotalLossOfUse = Convert.ToString(item.FinalTotalLossOfUse),
                    FinalTotalAdminFee = Convert.ToString(item.FinalTotalAdminFee),
                    FinalTotalDiminFee = Convert.ToString(item.FinalTotalDiminFee),
                    FinalTotalOtherFee = Convert.ToString(item.FinalTotalOtherFee),
                    FinalTotalSubroTotal = Convert.ToString((Convert.ToDecimal(item.FinalTotalDamage) + Convert.ToDecimal(item.FinalTotalLossOfUse) + Convert.ToDecimal(item.FinalTotalAdminFee) + Convert.ToDecimal(item.FinalTotalDiminFee) + Convert.ToDecimal(item.FinalTotalOtherFee))),
                    FinalTotalClosed = Convert.ToString(item.FinalTotalClosed),
                    FinalTotalPending = Convert.ToString(item.FinalTotalPending),
                    FinalTotalWriteOffTotal=Convert.ToString(item.FinalTotalWriteOffTotal)
                });
                //    chargeBackLossCustomReportViewModel.ToList().SelectMany(x => x.chargeBackLossReportViewModel);
            }
            return collectionCustomReportViewModel;
        }       
        internal static AdminFeeCommissionReportViewModel MappedAdminFeeCommissionReportViewModel(AdminFeeCommissionReportDto adminFeeCommissionReportDto)
        {
            AdminFeeCommissionReportViewModel adminFeeCommissionReportViewModel = new AdminFeeCommissionReportViewModel();

            adminFeeCommissionReportViewModel.UserWiseAdminFeeCommissionReportViewModel = adminFeeCommissionReportDto.UserWiseAdminFeeCommissionReportDto.Select(x => new UserWiseAdminFeeCommissionReportViewModel()
            {
                Employee = x.Employee,
                AmountEach = x.AmountEach.ToString(),
                Quantity = x.Quantity.ToString(),
                Commission = x.Commission.ToString()
            }).ToList<UserWiseAdminFeeCommissionReportViewModel>();

            adminFeeCommissionReportViewModel.ClaimWiseAdminFeeCommissionReportViewModel = adminFeeCommissionReportDto.ClaimWiseAdminFeeCommissionReportDto.Select(x => new ClaimWiseAdminFeeCommissionReportViewModel()
            {
                Claim = x.Claim,
                Contract= x.Contract,
                Location = x.Location,
                CompanyAbbr = x.CompanyAbbr,
                Payment =  x.Payment
            }).ToList < ClaimWiseAdminFeeCommissionReportViewModel>();
            //item.Select(x => x.Claim).Distinct().Count(),
            adminFeeCommissionReportViewModel.TotalContracts =  adminFeeCommissionReportDto.ClaimWiseAdminFeeCommissionReportDto.Select(x => new ClaimWiseAdminFeeCommissionReportViewModel()
            {
                Claim = x.Claim,
                Contract = x.Contract,
                Location = x.Location,
                CompanyAbbr = x.CompanyAbbr,
                Payment = x.Payment
            }).Select(x=>x.Claim).Distinct().Count().ToString();

            adminFeeCommissionReportViewModel.TotalPayments =  Math.Round(adminFeeCommissionReportDto.ClaimWiseAdminFeeCommissionReportDto.Select(x => new ClaimWiseAdminFeeCommissionReportViewModel()
            {
                Claim = x.Claim,
                Contract = x.Contract,
                Location = x.Location,
                CompanyAbbr = x.CompanyAbbr,
                Payment = x.Payment
            }).Select(x => x.Payment).Sum(),2).ToString();


            return adminFeeCommissionReportViewModel;
        }
        

        internal static IEnumerable<SearchTagPlateViewModel> MappedSearchTagModel(IEnumerable<SearchTagPlateDto> searchTagPlateDtoList)
        {
            List<SearchTagPlateViewModel> searchTagPlateList = null;
            if (searchTagPlateDtoList != null)
            {
                searchTagPlateList = searchTagPlateDtoList.Select(x => new SearchTagPlateViewModel()
                 {
                     UnitNumber = x.UnitNumber,
                     Messages = x.Messages,
                     TransDate = x.TransDate
                 }).ToList();

            }
            return searchTagPlateList;
        }
    }
}
