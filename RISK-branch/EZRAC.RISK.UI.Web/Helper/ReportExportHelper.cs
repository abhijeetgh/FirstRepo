using EZRAC.Core.FileGenerator;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Risk.UI.Web.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class ReportExportHelper
    {

        //internal static byte[] GetExportDateArray(string FileName, dynamic excelresult)
        //{
        //    byte[] arrayExcel = null;

        //    arrayExcel = ExcelHelper.GenerateExcel(excelresult, FileName);
        //    return arrayExcel;
        //}
        internal static byte[] TagPlatereports(IEnumerable<SearchTagPlateViewModel> searchTagPlateViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = searchTagPlateViewModel.Select(x => new
            {
                Unit = x.UnitNumber,
                UnitDetails = x.UnitNumber,
                Date = x.TransDate
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }

        internal static byte[] Adminreports(object AdminModel, string FileName, string ReportKeyName)
        {
            dynamic finalexcelresult = null;
            byte[] byteArrayExcel = null;

            if (ReportKeyName == "useractivity")
            {
                IEnumerable<UserActionLogViewModel> userActionLogViewModel = AdminModel as IEnumerable<UserActionLogViewModel>;
                finalexcelresult = userActionLogViewModel.Select(x => new
                {
                    //Contractno = x.ContractNo + "/" + x.ClaimId + "-" + x.CompanyAbbr,
                    Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.ClaimId),
                    User = x.Name,
                    Date = x.Date,
                    Action = x.UserAction
                });
                //byteArrayExcel = GetExportDateArray(FileName, finalexcelresult);
            }
            else if (ReportKeyName == "pendingamountdue")
            {
                IEnumerable<PaymentViewModel> paymentViewModel = AdminModel as IEnumerable<PaymentViewModel>;
                finalexcelresult = paymentViewModel.Select(x => new
                {
                    //Contractno = x.Contract + "/" + x.Claim + "-" + x.CompanyAbbr,
                    Contractno = String.Format("{0} / {1}-{2}", x.Contract, x.CompanyAbbr, x.Claim),
                    Location = x.Location,
                    Estimate = x.Estimated,
                    OtherCharge = x.OtherChanrges,
                    TotalCharge = x.Charges,
                    TotalPayment = x.Payments,
                    Balance = x.Balance
                });
                // byteArrayExcel = GetExportDateArray(FileName, finalexcelresult);
                //byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, "Admin");
            }
            else if (ReportKeyName == "calldates" || ReportKeyName == "assignedcontracts" || ReportKeyName == "customerservice")
            {
                IEnumerable<ClaimViewModel> claimViewModel = AdminModel as IEnumerable<ClaimViewModel>;

                if (ReportKeyName == "calldates")
                {
                    finalexcelresult = claimViewModel.Select(x => new
                    {
                        //ContractNo = x.Contract + "/" + x.CompanyAbbr + "-" + x.Claim,
                        Contractno = String.Format("{0} / {1}-{2}", x.Contract, x.CompanyAbbr, x.Claim),
                        User = x.UserDetail,
                        NextCallDate = x.FollowUpDate,
                        Driver = x.DriverName
                    });
                }
                if (ReportKeyName == "assignedcontracts")
                {
                    finalexcelresult = claimViewModel.Select(x => new
                    {
                        //ContractNo = x.Contract + "/" + x.CompanyAbbr + "-" + x.Claim,
                        Contractno = String.Format("{0} / {1}-{2}", x.Contract, x.CompanyAbbr, x.Claim),
                        User = x.UserDetail,
                        OpenDate = x.OpenDate,
                        Status = x.Status,
                        Driver = x.DriverName
                    });
                }
                if (ReportKeyName == "customerservice")
                {
                    finalexcelresult = claimViewModel.Select(x => new
                    {
                        //ContractNo = x.Contract + "/" + x.CompanyAbbr + "-" + x.Claim,
                        Contractno = String.Format("{0} / {1}-{2}", x.Contract, x.CompanyAbbr, x.Claim),
                        Open = x.OpenDate,
                        Status = x.Status,
                        Driver = x.DriverName
                    });
                }
            }
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, FileName);
            return byteArrayExcel;
        }

        internal static byte[] Vehiclestobereleasedreport(IEnumerable<VehicleToBeReleasedViewModel> vehicleToBeReleasedViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            IEnumerable<VehicleToBeReleasedViewModel> oVehicleToBeReleasedViewModel = vehicleToBeReleasedViewModel;
            finalexcelresult = oVehicleToBeReleasedViewModel.Select(x => new
            {
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                UnitDetail = x.UnitDetails,
                RAClosedDate = x.ClosedDate,
                LossDate = x.LossDate,
                OpenDate = x.OpenDate,
                LossType = x.LossType,
                Status = x.Status,
                RiskAgent = x.RiskAgent
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Writeoffreport(IEnumerable<WriteOffReportViewModel> writeOffReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            IEnumerable<WriteOffReportViewModel> oWriteOffReportViewModel = writeOffReportViewModel;
            finalexcelresult = oWriteOffReportViewModel.Select(x => new
            {
                CloseDate = x.ClosedDate,
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                Renter = x.Renter,
                UnitNumber = x.UnitNumber,
                Collected = x.TotalCollected,
                AmountDue = x.TotalDue,
                ClosedReason = x.ClosedReason,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Accountsreceivablereport(IEnumerable<AccountReceivableViewModel> accountReceivableViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            IEnumerable<AccountReceivableViewModel> oAccountReceivableViewModel = accountReceivableViewModel;
            finalexcelresult = oAccountReceivableViewModel.Select(x => new
            {
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                Renter = x.Renter,
                TotalCollected = x.TotalCollected,
                AmountDue = x.TotalDue,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Agingreport(IEnumerable<ARAgingReportViewModel> aRAgingReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            IEnumerable<ARAgingReportViewModel> oARAgingReportViewModel = aRAgingReportViewModel;
            finalexcelresult = oARAgingReportViewModel.Select(x => new
            {
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                Renter = x.Renter,
                UnitNumber = x.UnitNumber,
                LossDate = x.DateOfLoss,
                Billed = x.TotalBilled,
                Collected = x.TotalCollected,
                AmountDue = x.TotalDue,
                Days = x.NoOfDays,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Chargebacklossreport(IEnumerable<ChargebackLossReportViewModel> chargeBackLossReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = chargeBackLossReportViewModel.Select(x => new
            {
                CloseDate = x.ClosedDate,
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                Renter = x.Renter,
                TotalCollected = x.TotalCollected,
                TotalDue = x.TotalDue,
                ClosedReason = x.ClosedReason,
                OpeningAgent = x.OpeningAgent,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Vehicledamagesectionreport(IEnumerable<VehicleDamageSectionReportViewModel> vehicleDamageSectionReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = vehicleDamageSectionReportViewModel.Select(x => new
            {
                OpenDate = x.OpenDate,
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.Claim,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.Claim),
                Renter = x.Renter,
                UnitNumber = x.UnitNumber,
                TotalCollected = x.TotalCollected,
                TotalDue = x.TotalDue,
                VehicleSection = x.VehicleSection,
                ClaimStatus = x.Status,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Basicreports(IEnumerable<BasicReportViewModel> basicReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = basicReportViewModel.Select(x => new
            {
                claim = x.Claim,
                //Contractno = x.Contract + "/" + x.Claim + "-" + x.CompanyAbbr,
                Contractno = String.Format("{0} / {1}-{2}", x.Contract, x.CompanyAbbr, x.Claim),
                Customer = x.Customer,
                Unit = x.Unit,
                MakeModel = x.MakeModel,
                Tag = x.Tag,
                Location = x.Location,
                Charges = x.Charges,
                Payments = x.Payment,
                Balance = x.Balance,

            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Depositdatereport(IEnumerable<DepositDateReportViewModel> depositDateReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = depositDateReportViewModel.Select(x => new
            {

                //Contractno = x.Claim + "/" + x.ClaimId + "-" + x.CompanyAbbr,
                Contractno = String.Format("{0} / {1}-{2}", x.Claim, x.CompanyAbbr, x.ClaimId),
                Unit = x.Unit,
                UnitDescription = x.UnitDescription,
                Losstype = x.LossType,
                ClaimStatus = x.ClaimStatus,
                LossDate = x.LossDate,
                PaidFrom = x.PaidFrom,
                PaidDate = x.PaidDate,
                CheckAmount = x.CheckAmount,
                Billed = x.Billed,
                Balance = x.Balance,
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Chargesreport(IEnumerable<ChargeTypeReportViewModel> chargeTypeReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = chargeTypeReportViewModel.Select(x => new
            {
                Contractno = String.Format("{0} / {1}-{2}", x.Claim, x.CompanyAbbr, x.ClaimID),
                // Contractno = x.Claim + "/" + x.ClaimID + "-" + x.CompanyAbbr,
                ChargeAmount = x.ChargeAmount,
                LossDate = x.LossDate,
                Unit = x.Unit,
                Status = x.Status,
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Stolenrecoveredreport(IEnumerable<StolenRecoveredReportViewModel> stolenRecoveredReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = stolenRecoveredReportViewModel.Select(x => new
            {
                //Contractno = x.Claim + "/" + x.ClaimID + "-" + x.CompanyAbbr,
                Contractno = String.Format("{0} / {1}-{2}", x.Claim, x.CompanyAbbr, x.ClaimID),
                Unit = x.Unit,
                RAExpectedCloseDate = x.RAExpectedCloseDate,
                ReportedToPD = x.ReportedToPD,
                ClaimOpenDate = x.OpenDate,
                LossType = x.LossType,
                Status = x.Status,
                RiskAgent = x.RiskAgent,
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Collectionreport(IEnumerable<CollectionReportViewModel> collectionReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = collectionReportViewModel.Select(x => new
            {
                PayDate = x.PayDate,
                // ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.ClaimId,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.ClaimId),
                Renter = x.Renter,
                AmtCollected = x.Collected,
                PayFrom = x.PayFrom,
                EstDamage = x.Damage,
                LossOfUse = x.LossOfUse,
                AdminFee = x.AdminFee,
                DiminFee = x.DiminFee,
                OtherFee = x.OtherFee,
                SubroTotal = x.SubroTotal,
                WriteOffTotal = x.WriteOffTotal,
                Status = x.Status,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] Adminfeecommissionreport(IEnumerable<CollectionReportViewModel> collectionReportViewModel, string fileName)
        {
            dynamic finalexcelresult;
            byte[] byteArrayExcel = null;
            finalexcelresult = collectionReportViewModel.Select(x => new
            {
                PayDate = x.PayDate,
                //ContractNo = x.ContractNo + "/" + x.CompanyAbbr + "-" + x.ClaimId,
                Contractno = String.Format("{0} / {1}-{2}", x.ContractNo, x.CompanyAbbr, x.ClaimId),
                Renter = x.Renter,
                AmtCollected = x.Collected,
                PayFrom = x.PayFrom,
                EstDamage = x.Damage,
                LossOfUse = x.LossOfUse,
                AdminFee = x.AdminFee,
                DiminFee = x.DiminFee,
                OtherFee = x.OtherFee,
                SubroTotal = x.SubroTotal,
                Status = x.Status,
                LocationName = x.LocationName
            });
            byteArrayExcel = ExcelHelper.GenerateExcel(finalexcelresult, fileName);
            return byteArrayExcel;
        }
        internal static byte[] ExportAllClaims(DataTable exportClaims, string fileName)
        {
            byte[] byteArrayExcel = null;
            byteArrayExcel = ExcelHelper.GenerateDataTableExcel(exportClaims, fileName);
            return byteArrayExcel;
        }
    }
}