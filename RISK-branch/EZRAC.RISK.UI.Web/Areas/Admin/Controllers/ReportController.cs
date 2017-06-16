using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Models;
using EZRAC.RISK.Util.Common;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.ViewModels.Reports;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Core.FileGenerator;
using System.IO;
using System.Data;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    //[CRMSAdminAccess]
    public class ReportController : Controller
    {
        private IRiskReport _riskReport;
        private IBillingService _billingService;
        private IPaymentService _paymentService;
        private ITSDService _tsdService;
        //
        // GET: /Admin/Report/
        public ReportController(IRiskReport riskReport, IBillingService billingService, IPaymentService paymentService, ITSDService tsdService)
        {
            _riskReport = riskReport;
            _billingService = billingService;
            _paymentService = paymentService;
            _tsdService = tsdService;
        }
        public async Task<ActionResult> Index()
        {
            ViewBag.reportList = await _riskReport.GetReportListAsync(SecurityHelper.GetUserIdFromContext());
            return View();
        }

        public async Task<ActionResult> GetReportFormByKey(string reportKey, string reportText)
        {
            ReportCriteriaViewModel reportCriteriaViewModel = new ReportCriteriaViewModel();
            reportCriteriaViewModel = CommmonControlData(reportKey, reportText);
            return PartialView("_ReportCriteria", reportCriteriaViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateReport(ReportCriteriaViewModel reportCriteriaViewModel)
        {
            ReportCriteriaDto reportCriteriaDto = new ReportCriteriaDto();
            reportCriteriaDto = ReportHelper.MappedReportCriteria(reportCriteriaViewModel);

            Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();
            dic = await (GenerateReportReturn(reportCriteriaDto));
            if (dic.Count() > 0)
            {
                return PartialView(dic.ElementAt(0).Key, dic.ElementAt(0).Value);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public async Task<FileResult> ExportAllClaims(string fromDate, string toDate, string type)
        {
            byte[] byteArrayExcel = null;
            string fileName = string.Empty, ExcelName = "ExplortAllClaimsReport";
            fileName = ExcelName + ".csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            DataTable AllExportData = await _riskReport.GetAllClaimExport(fromDate, toDate, type);

            byteArrayExcel = ReportExportHelper.ExportAllClaims(AllExportData, ExcelName);
            return File(byteArrayExcel, "application/excel");
        }

        /// Private Methods
        /// for Admin report criteria
        private static ReportCriteriaViewModel CommmonControlData(string reportKey, string reportText)
        {
            ReportCriteriaViewModel reportCriteriaViewModel = new ReportCriteriaViewModel();
            reportCriteriaViewModel.ReportKey = reportKey;
            reportCriteriaViewModel.ReportText = reportText;

            //Dropdown data
            reportCriteriaViewModel.LossTypeList = LookUpHelpers.GetLossTypeListItems();
            reportCriteriaViewModel.ClaimStatusList = LookUpHelpers.GetReportClaimStatusListItems(reportKey);
            reportCriteriaViewModel.LocationList = LookUpHelpers.GetReportLocationListItem(reportKey);
            reportCriteriaViewModel.UserList = LookUpHelpers.GetAllUserListItem();
            reportCriteriaViewModel.VehicalDamageList = LookUpHelpers.GetAllDamageTypesListItem();
            reportCriteriaViewModel.DateTypeList = LookUpHelpers.GetAllDateTypes();
            reportCriteriaViewModel.ReportTypeList = LookUpHelpers.GetAllReportTypes();
            reportCriteriaViewModel.ChargeTypeList = LookUpHelpers.GetAllBillingTypes();
            reportCriteriaViewModel.PaymentTypeList = LookUpHelpers.GetAllPaymentTypes();
            reportCriteriaViewModel.AgingDaysList = LookUpHelpers.GetAgingDays();
            //
            return reportCriteriaViewModel;
        }

        ///Generic method for all report type return and types
        private async Task<Dictionary<string, dynamic>> GenerateReportReturn(ReportCriteriaDto reportCriteriaDto)
        {
            Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();
            switch (reportCriteriaDto.ReportKey)
            {
                case ReportConstants.ReportKey.TagPlatereports:
                    IEnumerable<SearchTagPlateViewModel> searchTagPlateViewModel = null;
                    searchTagPlateViewModel = ReportHelper.MappedSearchTagModel(await _tsdService.GetSearchTagPlate(Convert.ToString(reportCriteriaDto.TagNumber).Trim()));

                    Session["ExportData"] = searchTagPlateViewModel;
                    Session["ReportName"] = ReportConstants.ReportKey.TagPlatereports;
                    dic.Add("_ReportSearchTagPlate", searchTagPlateViewModel);
                    break;
                //return PartialView("_ReportSearchTagPlate", commonViewModel);
                case ReportConstants.ReportKey.Adminreports:
                    AdminReportViewModel adminReportViewModel = new AdminReportViewModel();
                    adminReportViewModel = ReportHelper.MappedAdminReportViewModel(await _riskReport.GetAdminReportAsync(reportCriteriaDto), reportCriteriaDto.ReportTypeKey);
                    if (reportCriteriaDto.ReportTypeKey == "useractivity")
                    {
                        Session["ExportData"] = adminReportViewModel.UserActionLogViewModel.ToList();
                    }
                    else if (reportCriteriaDto.ReportTypeKey == "pendingamountdue")
                    {
                        Session["ExportData"] = adminReportViewModel.PaymentViewModels.ToList();
                    }
                    else if (reportCriteriaDto.ReportTypeKey == "calldates" || reportCriteriaDto.ReportTypeKey == "assignedcontracts" || reportCriteriaDto.ReportTypeKey == "customerservice")
                    {
                        Session["ExportData"] = adminReportViewModel.ReportClaimViewModels.ToList();
                    }
                    Session["ReportName"] = ReportConstants.ReportKey.Adminreports;
                    Session["AdminReportKeyName"] = reportCriteriaDto.ReportTypeKey;
                    dic.Add("_AdminReport", adminReportViewModel);
                    break;
                //return PartialView("_AdminReport", adminReportViewModel);
                case ReportConstants.ReportKey.Vehiclestobereleasedreport:
                    List<VehicleToBeReleasedViewModel> vehicleToBeReleasedViewModel = null;
                    vehicleToBeReleasedViewModel = ReportHelper.MappedVahicleToBeReleasedViewModel(await _riskReport.GetVahicleToBeReleasedReportAsync(reportCriteriaDto)).ToList<VehicleToBeReleasedViewModel>();
                    Session["ExportData"] = vehicleToBeReleasedViewModel.ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Vehiclestobereleasedreport;
                    dic.Add("_ReportVahicleToBeReleased", vehicleToBeReleasedViewModel);
                    break;
                case ReportConstants.ReportKey.Writeoffreport:
                    IEnumerable<WriteOffCustomReportViewModel> writeOffCustomReportViewModel = null;
                    writeOffCustomReportViewModel = ReportHelper.MappedWriteOffCustomReportViewModel(await _riskReport.GetWriteOffReportAsync(reportCriteriaDto));
                    Session["ExportData"] = ((writeOffCustomReportViewModel != null) ? writeOffCustomReportViewModel.ToList().SelectMany(x => x.writeOffReportViewModel) : new List<WriteOffReportViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Writeoffreport;
                    dic.Add("_WriteOffReport", (writeOffCustomReportViewModel != null) ? writeOffCustomReportViewModel : new List<WriteOffCustomReportViewModel>());
                    break;
                case ReportConstants.ReportKey.Accountsreceivablereport:
                    IEnumerable<AccountReceivableReportViewModel> accountReceivableReportViewModel = null;
                    accountReceivableReportViewModel = ReportHelper.MappedAccountReceivableReportViewModel(await _riskReport.GetAccountReceivableReportAsync(reportCriteriaDto));
                    Session["ExportData"] = ((accountReceivableReportViewModel != null) ? accountReceivableReportViewModel.ToList().SelectMany(x => x.AccountReceivableViewModel) : new List<AccountReceivableViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Accountsreceivablereport;
                    dic.Add("_AccountReceivableReport", (accountReceivableReportViewModel != null) ? accountReceivableReportViewModel : new List<AccountReceivableReportViewModel>());
                    break;
                case ReportConstants.ReportKey.Agingreport:
                    IEnumerable<ARAgingCustomReportViewModel> aRAgingCustomReportViewModel = null;
                    aRAgingCustomReportViewModel = ReportHelper.MappedARAgingCustomReportViewModel(await _riskReport.GetARAgingReportAsync(reportCriteriaDto));
                    Session["ExportData"] = ((aRAgingCustomReportViewModel != null) ? aRAgingCustomReportViewModel.ToList().SelectMany(x => x.ARAgingReportViewModel) : new List<ARAgingReportViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Agingreport;
                    dic.Add("_ARAgingReport", (aRAgingCustomReportViewModel != null) ? aRAgingCustomReportViewModel : new List<ARAgingCustomReportViewModel>());
                    break;
                case ReportConstants.ReportKey.Chargebacklossreport:
                    IEnumerable<ChargebackLossCustomReportViewModel> chargeBackLossCustomReportViewModel = null;
                    chargeBackLossCustomReportViewModel = ReportHelper.MappedChargeBackLossCustomReportViewModel(await _riskReport.GetChargeBackLossCustomReportAsync(reportCriteriaDto));
                    Session["ExportData"] = ((chargeBackLossCustomReportViewModel != null) ? chargeBackLossCustomReportViewModel.ToList().SelectMany(x => x.chargeBackLossReportViewModel) : new List<ChargebackLossReportViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Chargebacklossreport;
                    dic.Add("_ChargeBackLossReport", (chargeBackLossCustomReportViewModel != null) ? chargeBackLossCustomReportViewModel : new List<ChargebackLossCustomReportViewModel>());
                    break;
                case ReportConstants.ReportKey.Vehicledamagesectionreport:
                    IEnumerable<VehicleDamageSectionCustomReportViewModel> vehicleDamageSectionCustomReportViewModel = null;
                    vehicleDamageSectionCustomReportViewModel = ReportHelper.MappedVehicleDamageSectionCustomReportViewModel(await _riskReport.GetVehicleDamageSectionCustomReportDtoAsync(reportCriteriaDto));
                    Session["ExportData"] = ((vehicleDamageSectionCustomReportViewModel != null) ? vehicleDamageSectionCustomReportViewModel.ToList().SelectMany(x => x.VehicleDamageSectionReportViewModel) : new List<VehicleDamageSectionReportViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Vehicledamagesectionreport;
                    dic.Add("_VehicleDamageSection", (vehicleDamageSectionCustomReportViewModel != null) ? vehicleDamageSectionCustomReportViewModel : new List<VehicleDamageSectionCustomReportViewModel>());
                    break;

                case ReportConstants.ReportKey.Basicreports:
                    IEnumerable<BasicReportViewModel> basicReportViewModel = null;
                    basicReportViewModel = ReportHelper.MappedBasicReportViewModel(await _riskReport.GetBasicReportAsync(reportCriteriaDto));
                    Session["ExportData"] = basicReportViewModel.ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Basicreports;
                    //GenerateExportFile();
                    dic.Add("_BasicReport", basicReportViewModel);
                    break;

                //16.	Deposit Date Report
                case ReportConstants.ReportKey.Depositdatereport:
                    IEnumerable<DepositDateReportViewModel> depositDateReportViewModel = null;
                    depositDateReportViewModel = ReportHelper.MappedDepositDateReportViewModel(await _riskReport.GetDepositDateReportAsync(reportCriteriaDto));
                    Session["ExportData"] = depositDateReportViewModel.ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Depositdatereport;
                    dic.Add("_DepositDateReport", depositDateReportViewModel);
                    break;
                //14.	Charge Type Report
                case ReportConstants.ReportKey.Chargesreport:
                    IEnumerable<ChargeTypeReportViewModel> chargeTypeReportViewModel = null;
                    chargeTypeReportViewModel = ReportHelper.MappedChargeTypeReportViewModel(await _riskReport.GetChargeTypeReportAsync(reportCriteriaDto));
                    Session["ExportData"] = chargeTypeReportViewModel.ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Chargesreport;
                    dic.Add("_ChargeTypeReport", chargeTypeReportViewModel);
                    break;

                //14.	Charge Type Report
                case ReportConstants.ReportKey.Stolenrecoveredreport:
                    IEnumerable<StolenRecoveredReportViewModel> stolenRecoveredReportViewModel = null;
                    stolenRecoveredReportViewModel = ReportHelper.MappedStolenRecoveredReportViewModel(await _riskReport.GetStolenRecoveredReportsAsync(reportCriteriaDto));
                    Session["ExportData"] = stolenRecoveredReportViewModel.ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Stolenrecoveredreport;
                    dic.Add("_StolenRecoveredReport", stolenRecoveredReportViewModel);
                    break;
                //14.	Admin Fee Comission Report
                case ReportConstants.ReportKey.Adminfeecommissionreport:
                    AdminFeeCommissionReportViewModel adminFeeCommissionReportViewModel = null;
                    adminFeeCommissionReportViewModel = ReportHelper.MappedAdminFeeCommissionReportViewModel(await _riskReport.GetAdminFeeCommissionReportAsync(reportCriteriaDto));
                    Session["ExportData"] = adminFeeCommissionReportViewModel;
                    Session["ReportName"] = ReportConstants.ReportKey.Adminfeecommissionreport;
                    dic.Add("_AdminFeeCommissionReport", adminFeeCommissionReportViewModel);
                    break;

                case ReportConstants.ReportKey.Collectionreport:
                    IEnumerable<CollectionCustomReportViewModel> collectionCustomReportViewModel = null;
                    collectionCustomReportViewModel = ReportHelper.MappedCollectionCustomReportViewModel(await _riskReport.GetCollectionReportsAsync(reportCriteriaDto));
                    Session["ExportData"] = ((collectionCustomReportViewModel != null ? collectionCustomReportViewModel.ToList().SelectMany(x => x.collectionReportViewModel) : new List<CollectionReportViewModel>()).ToList<CollectionReportViewModel>()).ToList();
                    Session["ReportName"] = ReportConstants.ReportKey.Collectionreport;
                    dic.Add("_CollectionReport", (collectionCustomReportViewModel != null) ? collectionCustomReportViewModel : new List<CollectionCustomReportViewModel>());
                    break;

            }
            return dic;
        }
        /// <summary>
        /// to create the Xls file using the EPPlus 
        /// </summary>
        /// <returns>Download the Excel sheet file with filtered data</returns>
        public FileResult GenerateExportFile()
        {
            byte[] byteArrayExcel = null;
            string fileName = string.Empty, ExcelName = string.Empty;
            switch (Convert.ToString(Session["ReportName"]))
            {
                case ReportConstants.ReportKey.TagPlatereports:
                    ExcelName = "Tag-PlateReports";
                    IEnumerable<SearchTagPlateViewModel> commonViewModel = Session["ExportData"] as IEnumerable<SearchTagPlateViewModel>;
                    byteArrayExcel = ReportExportHelper.TagPlatereports(commonViewModel, ExcelName);
                    //fileName = "Tag-PlateReport.xls";
                    break;
                case ReportConstants.ReportKey.Adminreports:
                    string ReportKeyName = Convert.ToString(Session["AdminReportKeyName"]);
                    ExcelName = "Admin";
                    object sessionDate = Session["ExportData"];
                    byteArrayExcel = ReportExportHelper.Adminreports(sessionDate, ExcelName, ReportKeyName);
                    //fileName = "Admin.xls";
                    break;
                case ReportConstants.ReportKey.Vehiclestobereleasedreport:
                    ExcelName = "VehiclesToBeReleasedReport";
                    IEnumerable<VehicleToBeReleasedViewModel> vehicleToBeReleasedViewModel = Session["ExportData"] as IEnumerable<VehicleToBeReleasedViewModel>;
                    byteArrayExcel = ReportExportHelper.Vehiclestobereleasedreport(vehicleToBeReleasedViewModel, ExcelName);
                    //fileName = "VehiclesToBeReleasedReport.xls";
                    break;
                case ReportConstants.ReportKey.Writeoffreport:
                    ExcelName = "WriteOffReport";
                    IEnumerable<WriteOffReportViewModel> writeOffCustomReportViewModel = Session["ExportData"] as IEnumerable<WriteOffReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Writeoffreport(writeOffCustomReportViewModel, ExcelName);
                    //fileName = "WriteOffReport.xls";
                    break;
                case ReportConstants.ReportKey.Accountsreceivablereport:
                    ExcelName = "AccountsReceivableReport";
                    IEnumerable<AccountReceivableViewModel> accountReceivableReportViewModel = Session["ExportData"] as IEnumerable<AccountReceivableViewModel>;
                    byteArrayExcel = ReportExportHelper.Accountsreceivablereport(accountReceivableReportViewModel, ExcelName);
                    //fileName = "AccountsReceivableReport.xls";
                    break;
                case ReportConstants.ReportKey.Agingreport:
                    ExcelName = "AgingReport";
                    IEnumerable<ARAgingReportViewModel> arAgingCustomReportViewModel = Session["ExportData"] as IEnumerable<ARAgingReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Agingreport(arAgingCustomReportViewModel, ExcelName);
                    //fileName = "AgingReport.xls";
                    break;
                case ReportConstants.ReportKey.Chargebacklossreport:
                    ExcelName = "ChargeBackLossReport";
                    IEnumerable<ChargebackLossReportViewModel> chargeBackLossReportViewModel = Session["ExportData"] as IEnumerable<ChargebackLossReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Chargebacklossreport(chargeBackLossReportViewModel, ExcelName);
                    //fileName = "ChargeBackLossReport.xls";
                    break;
                case ReportConstants.ReportKey.Vehicledamagesectionreport:
                    ExcelName = "VehicleDamageSectionReport";
                    IEnumerable<VehicleDamageSectionReportViewModel> vehicleDamageSectionReportViewModel = Session["ExportData"] as IEnumerable<VehicleDamageSectionReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Vehicledamagesectionreport(vehicleDamageSectionReportViewModel, ExcelName);
                    //fileName = "VehicleDamageSectionReport.xls";
                    break;
                case ReportConstants.ReportKey.Basicreports:
                    ExcelName = "Basic";
                    IEnumerable<BasicReportViewModel> basicReportViewModel = Session["ExportData"] as IEnumerable<BasicReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Basicreports(basicReportViewModel, ExcelName);
                    //fileName = "BasicReport.xls";
                    break;
                case ReportConstants.ReportKey.Depositdatereport:
                    ExcelName = "DepositDateReport";
                    IEnumerable<DepositDateReportViewModel> depositDateReportViewModel = Session["ExportData"] as IEnumerable<DepositDateReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Depositdatereport(depositDateReportViewModel, ExcelName);
                    //fileName = "DepositDateReport.xls";
                    break;
                case ReportConstants.ReportKey.Chargesreport:
                    ExcelName = "ChargeTypeReport";
                    IEnumerable<ChargeTypeReportViewModel> chargeTypeReportViewModel = Session["ExportData"] as IEnumerable<ChargeTypeReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Chargesreport(chargeTypeReportViewModel, ExcelName);
                    //fileName = "ChargeTypeReport.xls";
                    break;
                case ReportConstants.ReportKey.Stolenrecoveredreport:
                    ExcelName = "StolenRecoveredReport";
                    IEnumerable<StolenRecoveredReportViewModel> stolenRecoveredReportViewModel = Session["ExportData"] as IEnumerable<StolenRecoveredReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Stolenrecoveredreport(stolenRecoveredReportViewModel, ExcelName);
                    //fileName = "StolenRecoveredReport.xls";
                    break;
                case ReportConstants.ReportKey.Collectionreport:
                    ExcelName = "CollectionReport";
                    IEnumerable<CollectionReportViewModel> collectionCustomReportViewModel = Session["ExportData"] as IEnumerable<CollectionReportViewModel>;
                    byteArrayExcel = ReportExportHelper.Collectionreport(collectionCustomReportViewModel, ExcelName);
                    //fileName = "CollectionReport.xls";
                    break;
            }
            fileName = ExcelName + ".xls";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            return File(byteArrayExcel, "application/excel");


        }

        [HttpGet]
        public async Task<PartialViewResult> ViewBilling(Int64 id)
        {

            BillingsDto billings = await _billingService.GetBillingsByClaimIdAsync(id);

            BillingViewModel model = ClaimHelper.GetBillingsViewModel(billings, null, false);

            return PartialView(model);
        }

        [HttpGet]
        public async Task<PartialViewResult> ViewPayments(Int64 id)
        {
            PaymentDto paymentsDto = await _paymentService.GetAllPaymentsByClaimId(id);

            PaymentsViewModel viewmodel = PaymentHelper.GetPaymentViewModel(paymentsDto);

            return PartialView(viewmodel);
        }

    }
}
