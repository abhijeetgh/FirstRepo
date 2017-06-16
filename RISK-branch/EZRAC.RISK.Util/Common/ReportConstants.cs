using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Util.Common
{
    public static class ReportConstants
    {
        public struct ReportKey
        {
            public const string TagPlatereports = "tag-platereports";
            public const string Basicreports = "basicreports";
            public const string Adminreports = "adminreports";
            public const string Vehiclestobereleasedreport = "vehiclestobereleasedreport";
            public const string Writeoffreport = "writeoffreport";
            public const string Collectionreport = "collectionreport";
            public const string Accountsreceivablereport = "accountsreceivablereport";
            public const string Agingreport = "agingreport";
            public const string Chargebacklossreport = "chargebacklossreport";
            public const string Stolenrecoveredreport = "stolen-recoveredreport";
            public const string Vehicledamagesectionreport = "vehicledamagesectionreport";
            public const string Adminfeecommissionreport = "adminfeecommissionreport";
            public const string Chargesreport = "chargesreport";
            public const string Crispininvoice = "crispininvoice";
            public const string Depositdatereport = "depositdatereport";
            public const string ExportClaimsReport = "exportclaimreport";
        }
        public struct USPNames
        {
            public const string uspGetUserActivityAdminReport = "uspGetUserActivityAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds";
            public const string uspGetCallDateAdminReport = "uspGetCallDateAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds";
            public const string uspGetAssignedContractsAdminReport = "uspGetAssignedContractsAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds";
            public const string uspGetPendingPaymentDueAdminReport = "uspGetPendingPaymentDueAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds";
            public const string uspGetCustomerServiceAdminReport = "uspGetCustomerServiceAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds";
            public const string uspGetBasicReport = "uspGetBasicReport @DateType=@DateType,@FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Status=@Status,@IncludeTicket=@IncludeTicket";
            public const string uspAdminFeeCommissionReport = "uspAdminFeeCommissionReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Type=@Type";
            public const string uspUserWiseAdminFeeCommissionReport = "uspUserWiseAdminFeeCommissionReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Type=@Type";
            public const string uspGetChargeTypeReport = "uspGetChargeTypeReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@ChargeType=@ChargeType,@ChargeCondition=@ChargeCondition";
            public const string uspGetStolenRecoveredReport = "uspGetStolenRecoveredReport @Location=@Location,@ClaimStatus=@ClaimStatus";
            public const string uspGetDepositDateReport = "uspGetDepositDateReport @FromDate=@FromDate,@ToDate=@ToDate,@PaidFrom=@PaidFrom";
            public const string uspGetVehicalsToBeReleased = "uspGetVehicalsToBeReleased @Location=@Location,@Status=@Status,@LossType=@LossType";
            public const string uspGetWriteOffRerpot = "uspGetWriteOffRerpot @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Status=@Status,@LossType=@LossType";
            public const string uspGetAccountReceivableReport = "uspGetAccountReceivableReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Status=@Status";
            public const string uspGetAgingReport = "uspGetAgingReport @AsOfDate=@AsOfDate,@Location=@Location,@Status=@Status,@LossType=@LossType,@StartDate=@StartDate,@EndDate=@EndDate";
            public const string uspGetChargeBackLossReport = "uspGetChargeBackLossReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Status=@Status";
            public const string uspGetVehicleDamageSectionReport = "uspGetVehicleDamageSectionReport @FromDate=@FromDate,@ToDate=@ToDate,@Location=@Location,@Status=@Status,@VehicleType=@VehicleType";
            public const string uspGetCollectionReport = "uspGetCollectionReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@Location=@Location,@IncludeCollection=@IncludeCollection,@IncludeTicket=@IncludeTicket";
            public const string uspGetClaimsToExport = "uspGetClaimsToExport";
        }
        public struct DefaultSelectionKey
        {
            public const string Loc = "LOC";
            public const string Status = "STATUS";
        }
    }
}
