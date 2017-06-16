using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class Constants
    {



        public struct FetchContract
        {
            internal const string InvalidContractNumber = "InvalidContractNumber";
            internal const string ContractNotFound = "ContractNotFound";

        }

        public struct CrmsDateFormates
        {
            public const string MMDDYYYYForAttr = "{0:MM/dd/yyyy}";
            public const string MMDDYYYY = "MM/dd/yyyy";
            public const string MMMM = "MMMM";
            public const string Y = "Y";
        }

        public struct Roles
        {
            public const int Administrator = 1;
            public const int RiskAgent = 2;
            public const int RiskManager = 3;
            public const int RiskSupervisor = 4;
            public const int LocationManager = 5;

        }

        public struct StringConstants
        {
            public const string NoNotesFound = "Notes are not found ";
            public const string NoClaimsFound = "No Claims Found";
            public const string NotesEmailSubject = "Notes added by";
            public const string ApprovalRequiredEmailSubject = "Approval required for claim : {0}";
        }

        public struct AppSettings
        {
            internal const string PaymentPlanDate = "PaymentPlanDate";
        }


        public struct PageInfoConstants
        {
            internal const int PageNumber = 1;
            internal const string SortByClaimId = "Id";
            internal const string SortByColumn = "FollowUpDate";
            internal const string SortByDate = "Date";
            internal const bool SortOrder = false;
            internal const int DefaultRecordToShow = 5;
            internal const bool SortOrderTrue = true;
        }

        public enum BillingTypes : int
        {
            AdminChange = 1,
            Estimate = 2,
            LossOfUse = 3
        }

        public enum PaymentTypes : int
        {
            Cdw = 16,
            Ldw = 17,
            Lpc2 = 19,

        }

        public struct CacheConstants
        {
            internal const string AssignedUsers = "AssignedUsers";
            internal const string Locations = "Locations";
            internal const string LossTypes = "LossTypes";
            internal const string ClaimStatuses = "ClaimStatuses";
            internal const string NoteTypes = "NoteTypes";
            internal const string PoliceAgenecies = "PoliceAgenecies";
            internal const string DamageTypes = "DamageTypes";
            internal const string InsuranceCompanies = "InsuranceCompanies";
            internal const string FileCategories = "FileCategories";
            internal const string BilingTypes = "BilingTypes";
            internal const string Companies = "Companies";
            internal const string Users = "Users";
            internal const string PaymentType = "PaymentType";
            public static string AllUsersEmails = "AllUsersEmails";
            internal const string WriteOffTypes = "WriteOffTypes";
        }        

        public enum TrackerType :long
        { 
            AuthorizationSupport = 1,
            Risk = 2,
            Location = 3
        }

    }
}