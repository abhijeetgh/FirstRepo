using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Util.Common
{
    public static class ClaimsConstant
    {
        // public const string AdminMenuAccess = "AdminMenu";
        // public const string CreateClaim = "Create Claim";
        // public const string ViewClaim = "View Claim";
        // public const string PendingClaims = "Pending Claims";
        // public const string Salvage = "Salvage";
        // public const string DeleteNotes = "Delete Notes";

        public const string PendingApprovedClaims = "http://schemas.risk.com/ws/2005/05/identity/claims/pending-approvedclaims";
        public const string NewClaim = "http://schemas.risk.com/ws/2005/05/identity/claims/newclaim";
        public const string DeleteClaim = "http://schemas.risk.com/ws/2005/05/identity/claims/deleteclaim";
        public const string EditClaimInfo = "http://schemas.risk.com/ws/2005/05/identity/claims/editclaiminfo";
        public const string EditDownloadContractInfo = "http://schemas.risk.com/ws/2005/05/identity/claims/editdownloadcontractinfo";
        public const string DownloadVehicleInfo = "http://schemas.risk.com/ws/2005/05/identity/claims/downloadvehicleinfo";
        public const string EditIncidentInfo = "http://schemas.risk.com/ws/2005/05/identity/claims/editincidentinfo";
        public const string EditDriverInfo = "http://schemas.risk.com/ws/2005/05/identity/claims/editdriverinfo";
        public const string AddDamages = "http://schemas.risk.com/ws/2005/05/identity/claims/adddamages";
        public const string DeletedDamages = "http://schemas.risk.com/ws/2005/05/identity/claims/deletedamages";
        public const string AddFiles = "http://schemas.risk.com/ws/2005/05/identity/claims/addfiles";
        public const string DeleteFiles = "http://schemas.risk.com/ws/2005/05/identity/claims/deletefiles";
        public const string AddPayments = "http://schemas.risk.com/ws/2005/05/identity/claims/addpayments";
        public const string DeletePayments = "http://schemas.risk.com/ws/2005/05/identity/claims/deletepayments";
        public const string AddSalvage = "http://schemas.risk.com/ws/2005/05/identity/claims/addsalvage";
        public const string DeleteNotes = "http://schemas.risk.com/ws/2005/05/identity/claims/deletenotes";
        public const string EmailGenerator = "http://schemas.risk.com/ws/2005/05/identity/claims/emailgenerator";
        public const string TagplateReports = "http://schemas.risk.com/ws/2005/05/identity/claims/tag-platereports";
        public const string BasicReports = "http://schemas.risk.com/ws/2005/05/identity/claims/basicreports";
        public const string AdminReports = "http://schemas.risk.com/ws/2005/05/identity/claims/adminreports";
        public const string VehiclesToBeReleasedReport = "http://schemas.risk.com/ws/2005/05/identity/claims/vehiclestobereleasedreport";
        public const string WriteOffReport = "http://schemas.risk.com/ws/2005/05/identity/claims/writeoffreport";
        public const string CollectionReport = "http://schemas.risk.com/ws/2005/05/identity/claims/collectionreport";
        public const string AccountsReceivableReport = "http://schemas.risk.com/ws/2005/05/identity/claims/accountsreceivablereport";
        public const string AgingReport = "http://schemas.risk.com/ws/2005/05/identity/claims/agingreport";
        public const string ChargeBackLossReport = "http://schemas.risk.com/ws/2005/05/identity/claims/chargebacklossreport";
        public const string StolenRecoveredReport = "http://schemas.risk.com/ws/2005/05/identity/claims/stolen-recoveredreport";
        public const string VehicleDamageSectionReport = "http://schemas.risk.com/ws/2005/05/identity/claims/vehicledamagesectionreport";
        public const string AdminFeeCommissionReport = "http://schemas.risk.com/ws/2005/05/identity/claims/adminfeecommissionreport";
        public const string ChargesReport = "http://schemas.risk.com/ws/2005/05/identity/claims/chargesreport";
        public const string DepositDateReport = "http://schemas.risk.com/ws/2005/05/identity/claims/depositdatereport";
        public const string DocumentGenerator = "http://schemas.risk.com/ws/2005/05/identity/claims/documentgenerator";
        public const string AddBilling = "http://schemas.risk.com/ws/2005/05/identity/claims/addbilling";
        public const string DeleteBilling = "http://schemas.risk.com/ws/2005/05/identity/claims/deletebilling";
        public const string ClaimEvent = "http://schemas.risk.com/ws/2005/05/identity/claims/claimevent";
        public const string EditSalvage = "http://schemas.risk.com/ws/2005/05/identity/claims/editsalvage";
        public const string EditDocumentsReceived = "http://schemas.risk.com/ws/2005/05/identity/claims/editdocumentsreceived";
        public const string TrackerUndo = "http://schemas.risk.com/ws/2005/05/identity/claims/trackerundo";


        public const string ContractInformation = "Contract Information";
        public const string VehicleInformation = "Vehicle Information";
        public const string IncidentInformation = "Incident Information";
        public const string DamageInformation = "Damage Information";
        public const string ChargeInformation = "Billings / Payments Information";
        public const string PrimaryDriverInformation = "Primary Driver Information";
        public const string AdditionalDriverInformation = "Additional Driver Information";
        public const string ThirdPartyDriverInformation = "Third Party Driver Information";
        public const string PrimaryDriverInsuranceInformation = "Primary Driver Insurance Information";
        public const string AdditionalDriverInsuranceInformation = "Additional Driver Insurance Information";
        public const string ThirdPartyDriverInsuranceInformation = "Third Party Driver Insurance Information";
        public const string CreditCardInformation = "Credit Card Information";

        public const string PrimaryDriver = "Primary Driver";
        public const string AdditionalDriver = "Additional Driver";
        public const string ThirdPartyDriver = "Third Party Driver";
        public const string PrimaryDriverInsurance = "Primary Driver Insurance";
        public const string AdditionalDriverInsurance = "Additional Driver Insurance";
        public const string ThirdPartyDriverInsurance = "Third Party Driver Insurance";
        public const string OtherRiskUser = "Other Risk Users";
        public const string CustomEmail = "Custom Email";
        public const string EmailToEmpire = "Send Email to Empire";
        public const string EmailToNationalCasualty = "Send Email to National Casualty";
        public const string EmailToKnightManagement = "Send Email to Knight Management";

        public const string AdminAccessRoles = "AdminAccess";


        public enum EmailInfoToSend : int
        {
            ContractInformation = 1,
            VehicleInformation = 2,
            IncidentInformation = 3,
            DamageInformation = 4,
            ChargeInformation = 5,
            PrimaryDriverInformation = 6,
            AdditionalDriverInformation = 7,
            ThirdPartyDriverInformation = 8,
            PrimaryDriverInsuranceInformation = 9,
            AdditionalDriverInsuranceInformation = 10,
            ThirdPartyDriverInsuranceInformation = 11,
            CreditCardInformation = 12
        }

        public enum EmailRecipients : int
        {
            PrimaryDriver = 1,
            AdditionalDriver = 2,
            ThirdPartyDriver = 3,
            AdditionalDriverInsurance = 4,
            PrimaryDriverInsurance = 5,
            ThirdPartyDriverInsurance = 6,
            OtherRiskUser = 7,
            CustomEmail = 8,
            EmailToEmpire = 9,
            EmailToNationalCasualty = 10,
            EmailToKnightManagement = 11
        }

        public struct Roles
        {

            public const string RiskSupervisor = "Risk Supervisor";
            public const string RiskManager = "Risk Manager";
            public const string LocationManager = "Location Manager";

        }

        public struct AdvanceSearchCriteriaKey
        {
            public const string select = "0";//
            public const string contract = "contract";//
            public const string primarydrivername = "primarydrivername";//
            public const string additionaldrivername = "additionaldrivername";//
            public const string thirdpartyname = "thirdpartyname";//
            public const string driverlicense = "driverlicense";//
            //public const string checkno = "checkno";
            //public const string checkamount = "checkamount";
            public const string unitno = "unitno";//
            public const string last8ofvin = "last8ofvin";//
            public const string tagno = "tagno";//
            public const string phone = "phone";//
            public const string dateofloss = "dateofloss";//
            public const string policecaseno = "policecaseno";//
            public const string inspolicyno = "inspolicyno";//
            //public  const string chargeback = "chargeback ";
            //public const string zurichclaimno = "zurichclaimno";//
            public const string primarydriverins = "primarydriverins";
            public const string additionaldriverins = "additionaldriverins";
            public const string thirdpartyins = "thirdpartyins";
            public const string notes = "notes";//
            public const string followdaterange = "followdaterange";//
            public const string CreditCardNumber = "CreditCardNumber";//
            public const string CreditCardType = "CreditCardType";
            public const string CreditCardExpiration = "CreditCardExpiration";
        }

        public static Dictionary<string, string> AdvanceSearchCriteria()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(AdvanceSearchCriteriaKey.select, "--Select--");
            //dict.Add(AdvanceSearchCriteriaKey.contract, "Claim/Contract");
            dict.Add(AdvanceSearchCriteriaKey.primarydrivername, "Primary Driver's Name");
            dict.Add(AdvanceSearchCriteriaKey.additionaldrivername, "Additional Driver's Name");
            dict.Add(AdvanceSearchCriteriaKey.thirdpartyname, "Third Party's Name");
            dict.Add(AdvanceSearchCriteriaKey.driverlicense, "Driver License Number");
            //          dict.Add(AdvanceSearchCriteriaKey.checkno,"Check Number");
            //            dict.Add(AdvanceSearchCriteriaKey.checkamount, "Check Amount");
            dict.Add(AdvanceSearchCriteriaKey.unitno, "Unit Number");
            dict.Add(AdvanceSearchCriteriaKey.last8ofvin, "Last 8 of VIN");
            dict.Add(AdvanceSearchCriteriaKey.tagno, "Tag Number");
            dict.Add(AdvanceSearchCriteriaKey.phone, "Phone");
            dict.Add(AdvanceSearchCriteriaKey.dateofloss, "Date of Loss");
            dict.Add(AdvanceSearchCriteriaKey.policecaseno, "Police Case Number");
            dict.Add(AdvanceSearchCriteriaKey.inspolicyno, "Insurance Policy Number");
            //dict.Add(AdvanceSearchCriteriaKey.chargeback,"ChargeBack ");
            //dict.Add(AdvanceSearchCriteriaKey.zurichclaimno, "Zurich Claim Number");
            dict.Add(AdvanceSearchCriteriaKey.primarydriverins, "Primary Driver's Insurance Claim Number");
            dict.Add(AdvanceSearchCriteriaKey.additionaldriverins, "Additional Driver's Insurance Claim Number");
            dict.Add(AdvanceSearchCriteriaKey.thirdpartyins, "Third Party's Insurance Claim Number");
            dict.Add(AdvanceSearchCriteriaKey.notes, "Notes");
            dict.Add(AdvanceSearchCriteriaKey.followdaterange, "Follow Up Date");
            dict.Add(AdvanceSearchCriteriaKey.CreditCardNumber, "Renters Credit Card Number");
            dict.Add(AdvanceSearchCriteriaKey.CreditCardType, "Renters Credit Card Type");
            dict.Add(AdvanceSearchCriteriaKey.CreditCardExpiration, "Renters Credit Card Expiration");
           
            return dict;
        }

        public enum GetNotesType : byte
        {
            AllNotes = 0,
            QuickNotes = 1
        }

        public enum DriverTypes : int
        {
            Primary = 1,
            Additional = 2,
            ThirdParty = 3

        }

        public enum LossTypes : int
        {
            TrafficParkingTicket = 20,
            ViolationTicket = 30
        }


    }

    public struct UserActionLogConstant
    {
        public const string LoginAction = "Login";
        public const string Logout = "LogOut";
        public const string CreateClaim = "Creat Claim";
        public const string ViewClaim = "View Claim";
        public const string ChangeFollowUpDate = "Change Follow Up Date of Claim";
        public const string ChangeAssignedTo = "Changed AssignedTo";
        public const string DeleteClaim = "Delete Claim/s";
        public const string FetchTSDInfo = "Fetch TSD Contract Info";
        public const string EditClaimInfo = "Edit Claim Information";
        public const string EditContractInformation = "Edit Contract Information";
        public const string EditNonContractInformation = "Edit Non Contract Information";
        public const string DownloadContractInformation = "Download Contract Information";
        public const string EditVehicleInformation = "Edit Vehicle Information";
        public const string SwapVechicle = "Swap Vehicle Information";
        public const string DownloadVehicleInformation = "Download Vehicle Information";
        public const string EditIncidentInfo = "Edit Incident Information";
        public const string EditDriverInsuranceInfo = "Edit Driver Insurance Information";
        public const string EditThirdPartyDriverInfo = "Edit Third Party Driver Insurance Information";
        public const string BasicSearch = "Basic Search";
        public const string AdvancedSeach = "Advanced Search";
        public const string ClaimApprove = "Claim Approved";
        public const string ClaimReject = "Claim Rejected";
        public const string AddDamageInfo = "Add Damage";
        public const string ViewDamage = "View Damage";
        public const string DeleteDamage = "Delete Damage";
        public const string AddFile = "Add File(s)";
        public const string DeleteFile = "Delete File";
        public const string DeletteGroupFiles = "Delete Group Files";
        public const string DownLoadFile = "Download File";
        public const string ViewFiles = "View Files";
        public const string AddBilling = "Add Billing";
        public const string ViewBilling = "View Billing";
        public const string DeleteBilling = "Delete Billing";
        public const string AddPayment = "Add Payment";
        public const string ViewPayment = "View Payment";
        public const string DeletePayment = "Delete Payment";
        public const string ViewSalvage = "View Salvage";
        public const string AddSalvage = "Add/Update Salvage";
        public const string AddNote = "Add Note";
        public const string DeleteNote = "Delete Note";
        public const string AddQuickNote = "Add Quick Note";
        public const string UpdateHeaderSection = "Update Claim Header";
        public const string DeleteWriteOff = "Delete WriteOff";
        public const string AddWriteOff = "Add WriteOff";
    }

    public struct CommonConstant
    {
        public const string DateFormat = "MM/dd/yyyy HH:mm";
        public const string MMDDYYYY = "MM/dd/yyyy";
    }
    public enum RoleViewTypes : long
    {
        View = 1,
        Create = 2,
        Edit = 3,
        Delete = 4,
        Other = 5
    }

    public enum PermissionCategory : long
    {
        Claim = 1,
        Report = 2,
        Other = 5,
        Tracker = 6
    }

    public enum DocumentTypes : int
    {
        AR_DL1_RTR = 1,
        AR_DL2_RTR = 2,
        AR_DL3_RTR = 3,
        ARAC_Invoice = 4,
        Attorney_Acknowledgement_Letter = 5,
        Body_Shop_Repair = 6,
        CDW_NotVoided_ULL = 7,
        Client_Info_and_Authorization = 8,
        Customer_Fax_Cover_Sheet = 9,
        Customer_Service_Information = 10,
        Demand_Letter_1_3rd_Party = 11,
        Demand_Letter_1_3rd_Party_Insurance = 12,
        Demand_Letter_1_Renter = 13,
        Demand_Letter_1_Renter_Insurance = 14,
        Demand_Letter_2_Renter = 15,
        Demand_Letter_3_Renter = 16,
        Demand_Letter_4_Renter = 17,
        GFL_Renter = 18,
        Insurance_Fax_Cover_Sheet = 19,
        Introduction_Letter = 20,
        JNR_Debtor_Placement_Form = 21,
        National_Casualty_Insurance_Cover = 22,
        Send_Email_to_National_Casualty = 23,
        Payment_Balance_due = 24,
        Payment_Claim_Closed = 25,
        Payment_Plan = 26,
        Payoff_Form = 27,
        Repo_Auth_Nationwide_Repo = 28,
        Reposession_Authorization = 29,
        Salvage_Bid = 30,
        Salvage_Bid_Accepted = 31,
        SLI_Fax_Cover_Sheet = 32,
        Ticket_Admin_Fee = 33,
        Ticket_Affidavit = 34,
        Vehicle_Release = 35,
        Violation_Admin_Fee = 36,
        Violation_Admin_Fee_Decline = 37,
        Wholesale_Bill_of_Sale = 38,
        Zurich_Empire_Letter = 39,
        Reported_Stolen = 40

    }
    public static class ReportsConstanct
    {
        //ReportSection
        public static Dictionary<string, string> DateTypes()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("openDate", "Open Date");
            dict.Add("closeDate", "Close Date");
            dict.Add("pickupDate", "Pickup Date");
            dict.Add("returnDate", "Return Date");
            dict.Add("dateOfLoss", "Date Of Loss");
            return dict;
        }
        public static Dictionary<string, string> ReportTypes()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("useractivity", "User Activity");
            dict.Add("calldates", "Follow Up Date");
            dict.Add("assignedcontracts", "Assigned Contracts");
            dict.Add("pendingamountdue", "Pending Amount Due");
            dict.Add("customerservice", "Customer Service");
            return dict;
        }
        //End ReportSection
    }

    public struct DocumemtTemplateUrl
    {
        public const string AR_DL1_RTR = "Views/DocumentGenerator/Templates/ARDL1RTR.cshtml";
        public const string AR_DL2_RTR = "Views/DocumentGenerator/Templates/ARDL2RTR.cshtml";
        public const string AR_DL3_RTR = "Views/DocumentGenerator/Templates/ARDL3RTR.cshtml";   
        public const string Introduction_Letter = "Views/DocumentGenerator/Templates/IntroductionLetter.cshtml";
        public const string JNR_Debtor_Placement_Form = "Views/DocumentGenerator/Templates/JNRDebtorPlacementForm.cshtml";
        public const string Attorney_Acknowledgement_Letter = "Views/DocumentGenerator/Templates/AttorneyAcknowledgementLetter.cshtml";
        public const string CDW_NotVoided_ULL = "Views/DocumentGenerator/Templates/CdwNotVoided.cshtml";
        public static string Client_Info_and_Authorization = "Views/DocumentGenerator/Templates/ClientInfoAndAuthorization.cshtml";
        public static string Customer_Fax_Cover_Sheet = "Views/DocumentGenerator/Templates/CustomerFaxCoverSheet.cshtml";
        public const string National_Casualty_Insurance_Cover = "Views/DocumentGenerator/Templates/NationalCasualtyInsuranceCover.cshtml";
        public const string Zurich_Empire_Letter = "Views/DocumentGenerator/Templates/ZurichEmpireLetter.cshtml";
        public static string Customer_Service_Information = "Views/DocumentGenerator/Templates/CustomerServiceInformation.cshtml";
        public static string Demand_Letter_1_3rd_Party = "Views/DocumentGenerator/Templates/ThirdPartyDemandLetter.cshtml";
        public const string Payment_Balance_due = "Views/DocumentGenerator/Templates/PaymentBalanceDue.cshtml";
        public const string Payment_Claim_Closed = "Views/DocumentGenerator/Templates/PaymentClaimClosed.cshtml";
        public const string Payment_Plan = "Views/DocumentGenerator/Templates/PaymentPlan.cshtml";
        public static string Demand_Letter_1_3rd_Party_Insurance = "Views/DocumentGenerator/Templates/ThirdPartyInsuranceDemandLetter.cshtml";
        public const string Payoff_Form = "Views/DocumentGenerator/Templates/Payoff_Form.cshtml";
        public const string Repo_Auth_Nationwide_Repo = "Views/DocumentGenerator/Templates/ReposessionAuthorizationNationwideRepo.cshtml";
        public static string Demand_Letter_1_Renter_Insurance = "Views/DocumentGenerator/Templates/RenterInsuranceDemandLetter.cshtml";
        public static string Demand_Letter_1_Renter = "Views/DocumentGenerator/Templates/RenterDemandLetterOne.cshtml";
        public static string Demand_Letter_2_Renter = "Views/DocumentGenerator/Templates/RenterDemandLetterTwo.cshtml";
        public static string Demand_Letter_3_Renter = "Views/DocumentGenerator/Templates/RenterDemandLetterThree.cshtml";
        public static string Demand_Letter_4_Renter = "Views/DocumentGenerator/Templates/RenterDemandLetterFour.cshtml";
        public const string Reposession_Authorization = "Views/DocumentGenerator/Templates/ReposessionAuthorization.cshtml";
        public const string Salvage_Bid_Accepted = "Views/DocumentGenerator/Templates/SalvageBidAccepted.cshtml";
        public const string Salvage_Bid = "Views/DocumentGenerator/Templates/SalvageBid.cshtml";
        public static string GFL_Renter = "Views/DocumentGenerator/Templates/GflRenter.cshtml";
        public const string Ticket_Admin_Fee = "Views/DocumentGenerator/Templates/TicketAdminFee.cshtml";
        public static string Insurance_Fax_Cover_Sheet = "Views/DocumentGenerator/Templates/InsuranceFaxCover.cshtml";
        public const string Ticket_Affidavit = "Views/DocumentGenerator/Templates/TicketAffidavit.cshtml";
        public const string Violation_Admin_Fee = "Views/DocumentGenerator/Templates/ViolationAdminFee.cshtml";
        public const string Violation_Admin_Fee_Decline = "Views/DocumentGenerator/Templates/ViolationAdminFeeDeclined.cshtml";
        public static string Vehicle_Release = "Views/DocumentGenerator/Templates/VehicleRelease.cshtml";
        public static string Body_Shop_Repair = "Views/DocumentGenerator/Templates/BodyShopRepair.cshtml";
        public static string Wholesale_Bill_of_Sale = "Views/DocumentGenerator/Templates/WholeSaleBillOfSale.cshtml";
        public const string SLI_Fax_Cover_Sheet = "Views/DocumentGenerator/Templates/SLIFaxCoverSheet.cshtml";
        public const string Reported_Stolen = "Views/DocumentGenerator/Templates/ReportedStolen.cshtml";
    }

    public struct AppSettings
    {
        public const string TrafficParkingTicketPhone = "TrafficParkingTicketPhone";
        public const string TrafficParkingTicketFax = "TrafficParkingTicketFax";
        public const string ViolationTicketPhone = "ViolationTicketPhone";
        public const string ViolationTicketFax = "ViolationTicketFax";
        public const string SendEmailToEmpire = "SendEmailToEmpire";
        public const string SendEmailToNationalCasualty = "SendEmailToNationalCasualty";
        public const string SendEmailToKnightManagement = "SendEmailToKnightManagement";
    }

    public enum PurchaseType
    {
        Program,
        Risk,
        Used,
        Lease,
        Repurchased
    }

}
