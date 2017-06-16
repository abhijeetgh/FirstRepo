using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Models.Email;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class EmailTemplatesHelper
    {
        internal static EmailRequest CreateEmailForNewClaimAssigned(string createdByFullName, string assignedToEmail, Int64 ClaimId, List<string> locationUsersEmailIds)
        {
            EmailRequest emailRequest = null;
            string emailMessageBody = string.Empty;

            emailRequest = new EmailRequest();
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.NewClaimAssignedEmailTemplateFormatter);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailRequest.EmailBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.UserFullName,
               createdByFullName).Replace(EmailConstants.EmailTemplatePlaceholders.ClaimId, ClaimId.ToString());
            }

            emailRequest.EmailTo.Add(assignedToEmail);
            emailRequest.IsHtml = true;
            emailRequest.EmailFrom = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.FromMailId);

            if (locationUsersEmailIds != null && locationUsersEmailIds.Any())
            {
                foreach (var item in locationUsersEmailIds)
                {
                    emailRequest.CCEmailAddress.Add(item);
                }
            }
            //Adding Member Number
            emailRequest.EmailSubject = string.Format(ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.NewClaimSubmittedEmailSubject));
            emailRequest.DisplayName = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailDisplayName);
            emailRequest.Priority = System.Net.Mail.MailPriority.Normal;

            return emailRequest;
        }

        internal static EmailRequest CreateEmailForNewClaimSentforApproval(string createdByFullName, string assignedToEmail, Int64 ClaimId)
        {
            EmailRequest emailRequest = null;
            string emailMessageBody = string.Empty;

            emailRequest = new EmailRequest();
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.ClaimSentForApprovalEmailTemplateFormatter);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailRequest.EmailBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.UserFullName,
               createdByFullName).Replace(EmailConstants.EmailTemplatePlaceholders.ClaimId, ClaimId.ToString());
            }
            emailRequest.EmailTo.Add(assignedToEmail);

            emailRequest.IsHtml = true;
            emailRequest.EmailFrom = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.FromMailId);
            //Adding Member Number
            emailRequest.EmailSubject = string.Format(Constants.StringConstants.ApprovalRequiredEmailSubject, ClaimId);
            emailRequest.DisplayName = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailDisplayName);
            emailRequest.Priority = System.Net.Mail.MailPriority.Normal;

            return emailRequest;
        }



        internal static EmailRequest CreateEmailForNewClaimNotes(NotesEmailRequest request)
        {
            EmailRequest emailRequest = null;
            string emailMessageBody = string.Empty;

            emailRequest = new EmailRequest();
            emailMessageBody = LookUpHelpers.GetEmailTemplate(EmailConstants.EmailTemplates.NewNotesAddedForClaimEmailTemplateFormatter);

            if (!String.IsNullOrEmpty(emailMessageBody))
            {
                emailRequest.EmailBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceholders.Description, request.NoteDescription)
              .Replace(EmailConstants.EmailTemplatePlaceholders.ClaimId, request.ClaimId.ToString())
              .Replace(EmailConstants.EmailTemplatePlaceholders.EmailSender, request.NoteAddedBy)
              .Replace(EmailConstants.EmailTemplatePlaceholders.NoteType, request.NoteType)
              .Replace(EmailConstants.EmailTemplatePlaceholders.LastName, request.LastName)
              .Replace(EmailConstants.EmailTemplatePlaceholders.FirstName, request.FirstName);
            }
            emailRequest.EmailTo.Add(request.RecipientEmailId);
            emailRequest.IsHtml = true;
            emailRequest.EmailFrom = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.FromMailId);
            //Adding Member Number
            //emailRequest.EmailSubject = string.Format("{0} for {1}", Constants.StringConstants.NotesEmailSubject, claimId);
            emailRequest.EmailSubject = string.Format("{0} - {1} {2}", request.ClaimId, Constants.StringConstants.NotesEmailSubject, request.NoteAddedBy);
            emailRequest.DisplayName = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailDisplayName);
            emailRequest.Priority = System.Net.Mail.MailPriority.Normal;
            return emailRequest;
        }

        internal static EmailRequest CreateEmailForEmailGenerator(EmailGeneratorDto emailGeneratorDto)
        {
            EmailRequest emailRequest = null;

            string emailGeneratorHeader = EmailGeneratorManager.EmailGeneratorHeader(emailGeneratorDto);
            string informationToSend = EmailGeneratorManager.GetInformationToSendTemplate(emailGeneratorDto.InformationToSendDto);
            string notes = EmailGeneratorManager.GetNotesTemplate(emailGeneratorDto.SelectedNotes);

            if (emailGeneratorDto != null)
            {
                emailRequest = new EmailRequest();

                foreach (var email in emailGeneratorDto.EmailRecipients)
                    emailRequest.EmailTo.Add(email);

                if (emailGeneratorDto.SelectedFiles != null && emailGeneratorDto.SelectedFiles.Any())
                {
                    foreach (Attachment file in EmailGeneratorManager.GetAttachmentFiles(emailGeneratorDto.SelectedFiles))
                        emailRequest.Attachments.Add(file);
                }

                emailRequest.EmailSubject = emailGeneratorDto.Subject;

                emailRequest.EmailBody = String.Concat(emailGeneratorHeader, informationToSend, notes);
                emailRequest.IsHtml = true;
                emailRequest.EmailFrom = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.FromMailId);
                emailRequest.DisplayName = ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailDisplayName);
                emailRequest.Priority = System.Net.Mail.MailPriority.High;
                emailRequest.Headers = new System.Collections.Specialized.NameValueCollection();
                
                emailRequest.Headers.Add("Disposition-Notification-To", emailGeneratorDto.LoggedInUserEmail);
                emailRequest.Headers.Add("Return-Path", emailGeneratorDto.LoggedInUserEmail);
                emailRequest.Sender = emailGeneratorDto.LoggedInUserEmail;
                emailRequest.DeliveryOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;
            }

            return emailRequest;


        }
    }
}