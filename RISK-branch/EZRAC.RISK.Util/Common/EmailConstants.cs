using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Util.Common
{
    public static class EmailConstants
    {
        public struct EmailTemplates
        {
            public const string NewClaimAssignedEmailTemplateFormatter = "NewClaimAssignedEmail.htm";

            public const string ClaimSentForApprovalEmailTemplateFormatter = "ClaimSentForApproval.htm";

            public const string NewNotesAddedForClaimEmailTemplateFormatter = "NewNoteAddedForClaim.htm";

            public const string ContractInformation = "ContractInformation.htm";

            public const string ContractInformationWithoutCreditCardDetails = "ContractInformationWithoutCreditCardDetails.htm";

            public const string EmailGeneratorHeader = "EmailGeneratorHeader.htm";

            public const string VehicleInformation = "VehicleInformation.htm";

            public const string IncidentInformation = "IncidentInformation.htm";

            public const string VehicleDamageHeader = "VehicleDamageHeader.htm";

            public const string VehicleDamageContent = "VehicleDamageContent.htm";

            public const string VehicleDamageFooter = "VehicleDamageFooter.htm";

            public const string DriverInformation = "DriverInformation.htm";

            public const string DriversInsuranceInformation = "DriversInsuranceInformation.htm";

            public const string BillingHeader = "BillingHeader.htm";

            public const string BillingContent = "BillingContent.htm";

            public const string BillingFooter = "BillingFooter.htm";

            public const string PaymentHeader = "PaymentHeader.htm";

            public const string PaymentContent = "PaymentContent.htm";

            public const string PaymentFooter = "PaymentFooter.htm";

            public const string NotesContent = "NotesContent.htm";

            public const string NotesHeader = "NotesHeader.htm";
           
        }

        public struct AppSettings
        {
            public const string EmailTemplatePath = "EmailTemplatePath";

            public const string FromMailId = "FromMailId";

            public const string EmailDisplayName = "EmailDisplayName";

            public const string NewClaimSubmittedEmailSubject = "NewClaimSubmittedEmailSubject";

            public const string RecordsToDisplay = "RecordsToDisplay";

            public const string PicturesAndFilesPath = "PicturesAndFilesPath";
        }

        public struct EmailTemplatePlaceholders
        {
            public const string UserFullName = "||UserFullName||";
            public const string ClaimId = "||ClaimId||";
            public const string EmailSender = "||EmailSender||";
            public const string Description = "||Description||";
            public const string FirstName = "||FirstName||";
            public const string LastName = "||LastName||";
            public const string NoteType = "||NoteType||";

            // Start Email Generator Placeholders//
            //Email Generator Header
            
            public const string Remarks = "||Remarks||";


            //Contract Information//
            public const string ContractNumber = "||ContractNumber||";
            public const string PickupDate = "||PickupDate||";
            public const string ReturnDate = "||ReturnDate||";
            public const string MilesDriven = "||MilesDriven||";
            public const string DailyRate = "||DailyRate||";
            public const string WeeklyRate = "||WeeklyRate||";
            public const string CDW = "||CDW||";
            public const string LDW = "||LDW||";
            public const string SLI = "||SLI||";
            public const string LPC = "||LPC||";
            public const string RentersCreditCardNumber = "||RentersCreditCardNumber||";
            public const string RentersCreditCardType = "||RentersCreditCardType||";
            public const string RentersCreditCardExpirationDate = "||RentersCreditCardExpirationDate||";


            //Vehicle Information//
            public const string UnitNumber = "||UnitNumber||";
            public const string TagNumber = "||TagNumber||";
            public const string TagExpiration = "||TagExpiration||";
            public const string VIN = "||VIN||";
            public const string Year = "||Year||";
            public const string Make = "||Make||";
            public const string Model = "||Model||";
            public const string Color = "||Color||";
            public const string Miles = "||Miles||";
            public const string Status = "||Status||";
            public const string Location = "||Location||";
            public const string PurchaseType = "||PurchaseType||";

            //Incident Information//
            public const string LossDate = "||LossDate||";
            public const string IncidentLocation = "||IncidentLocation||";
            public const string CaseNumber = "||CaseNumber||";
            public const string RenterFault = "||RenterFault||";
            public const string ThirdPartyFault = "||ThirdPartyFault||";

            //Driver Information
            public const string DriverType = "||DriverType||";
            public const string DriverName = "||DriverName||";
            public const string DriverAddress = "||DriverAddress||";
            public const string City = "||City||";
            public const string State = "||State||";
            public const string ZIP = "||ZIP||";
            public const string Phone1 = "||Phone1||";
            public const string Phone2 = "||Phone2||";
            public const string Email = "||Email||";
            public const string Fax = "||Fax||";
            public const string DOB = "||DOB||";
            public const string License = "||License||";

            //Driver Insurance
            public const string InsuranceCompany = "||InsuranceCompany||";
            public const string PolicyNumber = "||PolicyNumber||";
            public const string Deductible = "||Deductible||";
            public const string Expires = "||Expires||";

            //Notes
            public const string NoteCategory = "||NoteCategory||";
            public const string NotesDesc = "||NotesDesc||";
            public const string NotesDate = "||NotesDate||";
            public const string CreatedBy = "||CreatedBy||";


            //Damage section
            public const string DamageSection = "||Section||";
            public const string DamageDetails = "||Details||";

            //Billing Section
            public const string BillingTypeDesc = "||BillingTypeDesc||";
            public const string SubTotal = "||subTotal||";

            //Payments Section
            public const string PaymentFrom = "||PaymentFrom||";
            public const string PaymentAmount = "||Amount||";

            // End Email Generator Placeholders//

        }
    }
}
