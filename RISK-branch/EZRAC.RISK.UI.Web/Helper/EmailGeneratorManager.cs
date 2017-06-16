using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class EmailGeneratorManager
    {

        internal static string GetInformationToSendTemplate(InformationToSendDto informationToSendDto)
        {
            
            StringBuilder emailBody = new StringBuilder();
            if (informationToSendDto != null)
            {

                if (informationToSendDto.ContractInfo != null)
                {
                    emailBody.Append(GetContractInformationTemplate(informationToSendDto.ContractInfo));
                }

                if (informationToSendDto.VehicleInfo != null)
                {
                    emailBody.Append(GetVehicleInformationTemplate(informationToSendDto.VehicleInfo));
                }

                if (informationToSendDto.IncidentInfo != null)
                {
                    emailBody.Append(GetIncidentInformationTemplate(informationToSendDto.IncidentInfo));
                }

                if (informationToSendDto.DriverInfo != null && informationToSendDto.DriverInfo.Any() && 
                        informationToSendDto.SelectedDriverInfo != null && informationToSendDto.SelectedDriverInfo.Any())
                {
                    emailBody.Append(GetDriversInformationTemplate(informationToSendDto.DriverInfo, informationToSendDto.SelectedDriverInfo));
                }

                if (informationToSendDto.DriverInfo != null && informationToSendDto.SelectedInsuranceInfo != null && informationToSendDto.SelectedInsuranceInfo.Any())
                {
                    emailBody.Append(GetDriversInsuranceInformationTemplate(informationToSendDto.DriverInfo, informationToSendDto.SelectedInsuranceInfo));
                }

                if (informationToSendDto.Damages != null && informationToSendDto.Damages.Any())
                {
                    emailBody.Append(GetDamageInformationTemplate(informationToSendDto.Damages));
                }

                if (informationToSendDto.Billings != null && informationToSendDto.Billings.Billings.Any())
                {
                    emailBody.Append(GetBillingsInformationTemplate(informationToSendDto.Billings));
                }

                if (informationToSendDto.Payments != null && informationToSendDto.Payments.Payments.Any())
                {
                    emailBody.Append(GetPaymentsInformationTemplate(informationToSendDto.Payments));
                }
            }
            return emailBody.ToString();
        }

        private static string GetPaymentsInformationTemplate(PaymentDto paymentDto)
        {
            string emailMessageBody = String.Empty;

          
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.PaymentHeader);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                foreach (var payment in paymentDto.Payments)
                {
                    emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.PaymentContent);

                    emailMessageBody =  emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.PaymentFrom, payment.PaymentFrom)
                                                       .Replace(EmailConstants.EmailTemplatePlaceholders.PaymentAmount, payment.Amount.ToString());

                  
                }

                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.PaymentContent);

                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.PaymentFrom, "Total Payment")
                                                   .Replace(EmailConstants.EmailTemplatePlaceholders.PaymentAmount, paymentDto.TotalPayment.ToString());

                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.PaymentContent);

                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.PaymentFrom, "Total Due")
                                                   .Replace(EmailConstants.EmailTemplatePlaceholders.PaymentAmount, (paymentDto.TotalBilling - paymentDto.TotalPayment).ToString());



                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.PaymentFooter);
               
            }

            return emailMessageBody;
        }

        private static string GetBillingsInformationTemplate(BillingsDto billingsDto)
        {
            string emailMessageBody = String.Empty;

            
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.BillingHeader);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                foreach (var billing in billingsDto.Billings)
                {

                    emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.BillingContent);

                    double subTotal = billing.Discount.HasValue ? billing.Amount * (1 - billing.Discount.Value / 100) : billing.Amount;

                    emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.BillingTypeDesc, billing.BillingTypeDesc)
                                                       .Replace(EmailConstants.EmailTemplatePlaceholders.SubTotal, subTotal.ToString());

                }


                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.BillingContent);

                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.BillingTypeDesc, "Total Billing")
                                                   .Replace(EmailConstants.EmailTemplatePlaceholders.SubTotal, billingsDto.TotalBilling.ToString());

                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.BillingFooter);

                
            }
            return emailMessageBody;
        }


        private static string GetDamageInformationTemplate(IEnumerable<DamageDto> damages)
        {
            string emailMessageBody = String.Empty;
           
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.VehicleDamageHeader);
            

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                foreach (var damage in damages)
                {
                    emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.VehicleDamageContent);

                    emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.DamageSection, damage.Section)
                                                        .Replace(EmailConstants.EmailTemplatePlaceholders.DamageDetails, damage.Details);

                    //damageText = damageText + String.Format("{0}: {1}<br></br>", damage.Section, damage.Details);
                }

                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.VehicleDamageFooter);
                //emailMessageBody = String.Concat(emailMessageBody, damageText);
            }
            return emailMessageBody;
        }

        private static string GetDriversInsuranceInformationTemplate(IEnumerable<DriverInfoDto> drivers, IEnumerable<ClaimsConstant.DriverTypes> driverTypes)
        {
            string emailMessageBody = String.Empty;

            foreach (var driverType in driverTypes)
            {
                DriverInfoDto driver = drivers.Where(x => x.DriverTypeId == Convert.ToInt32(driverType)).FirstOrDefault();
                if (driver != null)
                {
                    emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.DriversInsuranceInformation);
                    if (!String.IsNullOrEmpty(emailMessageBody))
                    {
                        emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.DriverType, ClaimHelper.GetDriverTypeText(driver.DriverTypeId))
                    .Replace(EmailConstants.EmailTemplatePlaceholders.InsuranceCompany, driver.InsuranceCompanyName)
                    .Replace(EmailConstants.EmailTemplatePlaceholders.PolicyNumber, driver.PolicyNumber)
                    .Replace(EmailConstants.EmailTemplatePlaceholders.Deductible, driver.Deductible.HasValue ? driver.Deductible.Value.ToString() : "0")
                    .Replace(EmailConstants.EmailTemplatePlaceholders.Expires, driver.InsuranceExpiry.HasValue ? driver.InsuranceExpiry.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty);
                    }
                }
            }
            return emailMessageBody;
        }

        private static string GetDriversInformationTemplate(IEnumerable<DriverInfoDto> drivers, IEnumerable<ClaimsConstant.DriverTypes> selectedDriverTypes)
        {
            string emailMessageBody = String.Empty;

            foreach (var driverType in selectedDriverTypes)
            {
                DriverInfoDto driver = drivers.Where(x => x.DriverTypeId == Convert.ToInt32(driverType)).FirstOrDefault();
                if (driver != null)
                {
                    emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.DriverInformation);

                    if (!String.IsNullOrEmpty(emailMessageBody))
                    {
                        emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.DriverType, ClaimHelper.GetDriverTypeText(driver.DriverTypeId))
                   .Replace(EmailConstants.EmailTemplatePlaceholders.DriverName, String.Format("{0} {1}", driver.FirstName, driver.LastName))
                   .Replace(EmailConstants.EmailTemplatePlaceholders.DriverAddress, driver.Address1)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.City, driver.City)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.State, driver.State)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.ZIP, driver.Zip)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.Phone1, driver.Phone1)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.Phone2, driver.Phone2)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.Email, driver.Email)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.Fax, driver.Fax)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.DOB, driver.DOB.HasValue ? driver.DOB.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty)
                   .Replace(EmailConstants.EmailTemplatePlaceholders.License, driver.LicenceNumber);
                    }
                }
            }

            return emailMessageBody;
        }

        //private static string GetDriverTypeText(int? driverType)
        //{
        //    string driverTypeText = String.Empty;
        //    if (driverType.HasValue)
        //    {
        //        ClaimsConstant.DriverTypes driverTypeEnum = (ClaimsConstant.DriverTypes)driverType.Value;

        //        switch (driverTypeEnum)
        //        {
        //            case ClaimsConstant.DriverTypes.Primary:
        //                driverTypeText = "Primary";
        //                break;
        //            case ClaimsConstant.DriverTypes.Additional:
        //                driverTypeText = "Additional";
        //                break;
        //            case ClaimsConstant.DriverTypes.ThirdParty:
        //                driverTypeText = "Third Party";
        //                break;
        //            default:
        //                break;
        //        }

        //    }
        //    return driverTypeText;
        //}

        private static string GetIncidentInformationTemplate(IncidentDto incidentDto)
        {
            string emailMessageBody = String.Empty;

            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.IncidentInformation);
            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.LossDate, incidentDto.LossDate.HasValue ? incidentDto.LossDate.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty)
           .Replace(EmailConstants.EmailTemplatePlaceholders.IncidentLocation, incidentDto.SelectedLocationName)
           .Replace(EmailConstants.EmailTemplatePlaceholders.CaseNumber, incidentDto.CaseNumber)
           .Replace(EmailConstants.EmailTemplatePlaceholders.RenterFault, incidentDto.RenterFault ? "Yes" : "No")
           .Replace(EmailConstants.EmailTemplatePlaceholders.ThirdPartyFault, incidentDto.ThirdPartyFault ? "Yes" : "No");
            }

            return emailMessageBody;
        }

        private static string GetVehicleInformationTemplate(VehicleDto vehicleDto)
        {
            string emailMessageBody = String.Empty;

            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.VehicleInformation);
            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.UnitNumber, vehicleDto.UnitNumber)
            .Replace(EmailConstants.EmailTemplatePlaceholders.TagNumber, vehicleDto.TagNumber)
            .Replace(EmailConstants.EmailTemplatePlaceholders.TagExpiration, vehicleDto.TagExpires.HasValue ? vehicleDto.TagExpires.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty)
            .Replace(EmailConstants.EmailTemplatePlaceholders.VIN, vehicleDto.VIN)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Year, vehicleDto.Year)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Make, vehicleDto.Make)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Model, vehicleDto.Model)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Color, vehicleDto.Color)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Miles, vehicleDto.Mileage.HasValue ? vehicleDto.Mileage.Value.ToString() : String.Empty)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Status, vehicleDto.Status)
            .Replace(EmailConstants.EmailTemplatePlaceholders.Location, vehicleDto.Location)
            .Replace(EmailConstants.EmailTemplatePlaceholders.PurchaseType, vehicleDto.PurchaseType.ToString());
            }

            return emailMessageBody;
        }

        private static string GetContractInformationTemplate(ContractDto contractDto)
        {
            string emailMessageBody = String.Empty;

            if (contractDto.CardNumber.HasValue || !String.IsNullOrEmpty(contractDto.CardExpDate) || !String.IsNullOrEmpty(contractDto.CardType))
            {
                emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.ContractInformation);
            }
            else 
            {
                emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.ContractInformationWithoutCreditCardDetails);
            
            }

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.ContractNumber, contractDto.ContractNumber)
                .Replace(EmailConstants.EmailTemplatePlaceholders.PickupDate, contractDto.PickupDate.HasValue ? contractDto.PickupDate.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty)
                .Replace(EmailConstants.EmailTemplatePlaceholders.ReturnDate, contractDto.PickupDate.HasValue ? contractDto.ReturnDate.Value.ToString(Constants.CrmsDateFormates.MMDDYYYY) : String.Empty)
                .Replace(EmailConstants.EmailTemplatePlaceholders.MilesDriven, contractDto.Miles.HasValue ? contractDto.Miles.Value.ToString() : String.Empty)
                .Replace(EmailConstants.EmailTemplatePlaceholders.DailyRate, contractDto.DailyRate.HasValue ? contractDto.DailyRate.Value.ToString() : String.Empty)
                .Replace(EmailConstants.EmailTemplatePlaceholders.WeeklyRate, contractDto.WeeklyRate.HasValue ? contractDto.WeeklyRate.Value.ToString() : String.Empty)
                .Replace(EmailConstants.EmailTemplatePlaceholders.CDW, contractDto.CDW ? "Yes" : "No")
                .Replace(EmailConstants.EmailTemplatePlaceholders.LDW, contractDto.LDW ? "Yes" : "No")
                .Replace(EmailConstants.EmailTemplatePlaceholders.SLI, contractDto.SLI ? "Yes" : "No")
                .Replace(EmailConstants.EmailTemplatePlaceholders.LPC, contractDto.LPC ? "Yes" : "No");


                if (contractDto.CardNumber.HasValue || !String.IsNullOrEmpty(contractDto.CardExpDate) || !String.IsNullOrEmpty(contractDto.CardType))
                {
                    emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.RentersCreditCardNumber, contractDto.CardNumber.HasValue ? contractDto.CardNumber.Value.ToString() : String.Empty)
                     .Replace(EmailConstants.EmailTemplatePlaceholders.RentersCreditCardType, contractDto.CardType)
                     .Replace(EmailConstants.EmailTemplatePlaceholders.RentersCreditCardExpirationDate, contractDto.CardExpDate);

                }

            }

            return emailMessageBody;
        }

        internal static string GetNotesTemplate(IEnumerable<NotesDto> notes)
        {
            string notesHeader = String.Empty;
            string notesContent = String.Empty;
           
            if (notes != null && notes.Any())
            {
                notesHeader = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.NotesHeader);
                 notesContent = GetNotesContent(notes);
            }
            return String.Concat(notesHeader, notesContent);
        }

       

        private static string GetNotesContent(IEnumerable<NotesDto> notes)
        {
            string emailMessageBody = String.Empty;

            foreach (var note in notes)
            {
                emailMessageBody = emailMessageBody + LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.NotesContent);
                if (!String.IsNullOrEmpty(emailMessageBody))
                {
                    emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.NoteCategory, note.NoteTypeDescription)
                 .Replace(EmailConstants.EmailTemplatePlaceholders.NotesDesc, note.Description)
                 .Replace(EmailConstants.EmailTemplatePlaceholders.NotesDate, note.Date.HasValue ? note.Date.Value.ToString() : String.Empty)
                  .Replace(EmailConstants.EmailTemplatePlaceholders.CreatedBy, note.UpdatedBy);
                }
            }
            return emailMessageBody;
        }

        //private static string GetNotesHeader()
        //{
        //    string emailMessageBody = String.Empty;
        //    string emailTemplateFilePath = String.Empty;

        //    string emailTemplateFileName = String.Format(EmailConstants.EmailTemplates.NotesHeader);

        //    if (string.IsNullOrWhiteSpace(emailTemplateFilePath))
        //    {
        //        emailTemplateFilePath = String.Concat(System.Web.HttpContext.Current.Server.MapPath(ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailTemplatePath)), emailTemplateFileName);

        //    }

        //    if (File.Exists(emailTemplateFilePath))
        //    {

        //        StreamReader templateReader = File.OpenText(emailTemplateFilePath);
        //        try
        //        {
        //            if (templateReader != null)
        //            {
        //                emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.NotesHeader);
        //            }
        //        }
        //        finally
        //        {
        //            templateReader.Close();
        //        }

        //    }

        //    return emailMessageBody;
        //}

       
        internal static IEnumerable<Attachment> GetAttachmentFiles(IEnumerable<PicturesAndFilesDto> files)
        {
            List<Attachment> attachments = null;
            string emailTemplateFilePath = String.Empty;

            if (files != null && files.Any())
            {
                attachments = new List<Attachment>();
                foreach (var file in files)
                {
                    string emailTemplateFileName = String.Format(file.FilePath);
                    emailTemplateFilePath = String.Concat(System.Web.HttpContext.Current.Server.MapPath(ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.PicturesAndFilesPath)), emailTemplateFileName);

                    attachments.Add(new Attachment(emailTemplateFilePath));
                }
            }
            return attachments;
        }


        internal static string EmailGeneratorHeader(EmailGeneratorDto emailGeneratorDto)
        {
            string emailMessageBody = String.Empty;

            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.EmailGeneratorHeader);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.Remarks, emailGeneratorDto.Remarks)
                    .Replace(EmailConstants.EmailTemplatePlaceholders.ClaimId, String.Format("{0}-{1}", emailGeneratorDto.CompanyAbbr != null ?
                    emailGeneratorDto.CompanyAbbr.ToUpper() : String.Empty, emailGeneratorDto.ClaimId));
            }

            return emailMessageBody;
        }
    }
}