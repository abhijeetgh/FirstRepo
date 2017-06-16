using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using RateShopper.Domain.DTOs;
using System.Configuration;
using System.IO;
using RateShopper.Core.Cache;
using RateShopper.Domain.Constant;
using RateShopper.Core.Email_Helper;


namespace RateShopper.Services.Helper
{
    public static class EmailTemplateHelper
    {

        public static EmailRequest CreateEmailForShopDateDetail(List<FTBJobShopDateDetails> ftbShopDateDetails, FTBEmailCommonSettings emailCommonSettings)
        {
            EmailRequest emailRequest = null;
            try
            {
                string shopDatesWithTarget = GetShopDatesTemplate(ftbShopDateDetails);
                string ShopDateMasterTemplate = string.Empty;
                string adminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                string adminName = ConfigurationManager.AppSettings["AdminName"];
                if (emailCommonSettings != null)
                {
                    emailRequest = new EmailRequest();
                    emailRequest.EmailTo.Add(emailCommonSettings.assignedToEmail);
                    emailCommonSettings.UserName = emailCommonSettings.UserName;
                    if (string.IsNullOrEmpty(emailCommonSettings.assignedToEmail))
                    {
                        emailRequest.EmailTo.Add(adminEmail);
                        emailCommonSettings.UserName = adminName;
                    }
                    else
                    {
                        emailRequest.CCEmailAddress.Add(adminEmail);
                    }

                    if (string.IsNullOrEmpty(emailCommonSettings.BlackoutPeriod))
                    {
                        emailCommonSettings.BlackoutPeriod = ConfigurationManager.AppSettings["Blackout"];
                    }
                    // add cc to email request
                    if (emailCommonSettings.EmailRecipients != null && emailCommonSettings.EmailRecipients.Count > 0)
                    {
                        foreach (var ccEmail in emailCommonSettings.EmailRecipients)
                        {
                            emailRequest.CCEmailAddress.Add(ccEmail);
                        }
                    }

                    // add bcc to email request
                    if (emailCommonSettings.BccEmailRecipients != null && emailCommonSettings.BccEmailRecipients.Count > 0)
                    {
                        foreach (var bccEmail in emailCommonSettings.BccEmailRecipients)
                        {
                            emailRequest.BccEmailAddress.Add(bccEmail);
                        }
                    }

                    ShopDateMasterTemplate = GetEmailTemplate(EmailPathConstants.EmailTemplatePath.FTBEmailTempPath, EmailConstants.EmailTemplates.ShopDateMasterTemplate);

                    if (!string.IsNullOrEmpty(ShopDateMasterTemplate))
                    {
                        ShopDateMasterTemplate = ShopDateMasterTemplate.Replace(EmailConstants.EmailTemplatePlaceHolders.ShopDateList, shopDatesWithTarget).Replace(EmailConstants.EmailTemplatePlaceHolders.MonthYear, emailCommonSettings.MonthYear)
                            .Replace(EmailConstants.EmailTemplatePlaceHolders.UserName, emailCommonSettings.UserName).Replace(EmailConstants.EmailTemplatePlaceHolders.Message, ConfigurationManager.AppSettings["TargetMessage"])
                           .Replace(EmailConstants.EmailTemplatePlaceHolders.LocationBrand, emailCommonSettings.LocationBrand).Replace(EmailConstants.EmailTemplatePlaceHolders.BlackOut, emailCommonSettings.BlackoutPeriod);
                    }
                    emailRequest.EmailSubject = emailCommonSettings.Subject;
                    emailRequest.EmailBody = ShopDateMasterTemplate;
                    emailRequest.IsHtml = true;
                    emailRequest.EmailFrom = ConfigurationManager.AppSettings["FromMailId"];
                    emailRequest.EmailSubject = ConfigurationManager.AppSettings["TargetSubject"];
                    emailRequest.Priority = System.Net.Mail.MailPriority.Normal;
                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteToLogFile("Error Occured in CreateEmailForShopDateDetail", ex.Message + "Inner Exception" + ex.InnerException
                       + ", Stack Trace: " + ex.StackTrace + LogHelper.GetLogFilePath());
            }
            return emailRequest;
        }

        public static EmailRequest CreateGeneralEmail(string message, FTBEmailCommonSettings emailCommonSettings)
        {
            EmailRequest emailRequest = null;

            if (emailCommonSettings != null)
            {
                if (string.IsNullOrEmpty(emailCommonSettings.UserName))
                {
                    emailCommonSettings.UserName = ConfigurationManager.AppSettings["AdminName"];
                }
                string emailMessageBody = GetEmailTemplate(EmailPathConstants.EmailTemplatePath.FTBEmailTempPath, EmailConstants.EmailTemplates.GeneralTemplate);
                if (!String.IsNullOrEmpty(emailMessageBody))
                {
                    emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceHolders.UserName, emailCommonSettings.UserName)
                        .Replace(EmailConstants.EmailTemplatePlaceHolders.LocationBrand, emailCommonSettings.LocationBrand)
                        .Replace(EmailConstants.EmailTemplatePlaceHolders.MonthYear, emailCommonSettings.MonthYear)
                        .Replace(EmailConstants.EmailTemplatePlaceHolders.Message, message);

                }
                emailRequest = new EmailRequest();

                if (!string.IsNullOrEmpty(emailCommonSettings.assignedToEmail))
                {
                    emailRequest.EmailTo.Add(emailCommonSettings.assignedToEmail);
                    emailRequest.CCEmailAddress.Add(ConfigurationManager.AppSettings["AdminEmail"]);
                }
                else
                {
                    emailRequest.EmailTo.Add(ConfigurationManager.AppSettings["AdminEmail"]);
                }

                // add cc to email request
                if (emailCommonSettings.EmailRecipients != null && emailCommonSettings.EmailRecipients.Count > 0)
                {
                    foreach (var ccEmail in emailCommonSettings.EmailRecipients)
                    {
                        emailRequest.CCEmailAddress.Add(ccEmail);
                    }
                }

                // add bcc to email request
                if (emailCommonSettings.BccEmailRecipients != null && emailCommonSettings.BccEmailRecipients.Count > 0)
                {
                    foreach (var bccEmail in emailCommonSettings.BccEmailRecipients)
                    {
                        emailRequest.BccEmailAddress.Add(bccEmail);
                    }
                }

                emailRequest.EmailSubject = emailCommonSettings.Subject;

                emailRequest.EmailBody = String.Concat(emailMessageBody);
                emailRequest.IsHtml = true;
                emailRequest.EmailFrom = ConfigurationManager.AppSettings["FromMailId"];
                emailRequest.Priority = System.Net.Mail.MailPriority.High;
            }
            return emailRequest;
        }

        public static MailMessage CreateQuickViewEmail(QuickViewEmailDTO objQuickViewEmailDTO)
        {
            EmailRequest emailRequest = null;

            if (objQuickViewEmailDTO != null)
            {
                if (string.IsNullOrEmpty(objQuickViewEmailDTO.UserName))
                {
                    objQuickViewEmailDTO.UserName = ConfigurationManager.AppSettings["AdminName"];
                }
                string emailMessageBody = GetEmailTemplate(EmailPathConstants.EmailTemplatePath.QuickViewEmailTempPath, EmailConstants.EmailTemplates.QuickViewTemplate);
                if (!String.IsNullOrEmpty(emailMessageBody))
                {
                    emailMessageBody = emailMessageBody.Replace(EmailConstants.QuickViewPlaceHolders.UserName, objQuickViewEmailDTO.UserName)
                        .Replace(EmailConstants.QuickViewPlaceHolders.LocationBrand, objQuickViewEmailDTO.BrandLocation)
                        .Replace(EmailConstants.QuickViewPlaceHolders.ShopDate, objQuickViewEmailDTO.ShopDate)
                        .Replace(EmailConstants.QuickViewPlaceHolders.CarClasses, objQuickViewEmailDTO.CarClasses)
                        .Replace(EmailConstants.QuickViewPlaceHolders.ChangeDates, objQuickViewEmailDTO.ChangeDates)
                        .Replace(EmailConstants.QuickViewPlaceHolders.LOR, objQuickViewEmailDTO.LOR);
                }
                emailRequest = new EmailRequest();

                if (!string.IsNullOrEmpty(objQuickViewEmailDTO.Email))
                {
                    emailRequest.EmailTo.Add(objQuickViewEmailDTO.Email);
                    emailRequest.CCEmailAddress.Add(ConfigurationManager.AppSettings["AdminEmail"]);
                }
                else
                {
                    emailRequest.EmailTo.Add(ConfigurationManager.AppSettings["AdminEmail"]);
                }

                // add cc to email request
                if (objQuickViewEmailDTO.EmailRecipients != null && objQuickViewEmailDTO.EmailRecipients.Count > 0)
                {
                    foreach (var ccEmail in objQuickViewEmailDTO.EmailRecipients)
                    {
                        emailRequest.CCEmailAddress.Add(ccEmail);
                    }
                }

                // add bcc to email request
                if (objQuickViewEmailDTO.BccEmailRecipients != null && objQuickViewEmailDTO.BccEmailRecipients.Count > 0)
                {
                    foreach (var bccEmail in objQuickViewEmailDTO.BccEmailRecipients)
                    {
                        emailRequest.BccEmailAddress.Add(bccEmail);
                    }
                }

                emailRequest.EmailSubject = string.Concat(ConfigurationManager.AppSettings["QuickViewSubject"], " - ", objQuickViewEmailDTO.BrandLocation);

                emailRequest.EmailBody = String.Concat(emailMessageBody);
                emailRequest.IsHtml = true;
                emailRequest.EmailFrom = ConfigurationManager.AppSettings["FromMailId"];
                emailRequest.Priority = System.Net.Mail.MailPriority.High;
            }
            return EmailHelper.CreateEmailMessage(emailRequest);
        }

        static string GetShopDatesTemplate(List<FTBJobShopDateDetails> ftbShopDates)
        {
            string shopDateReport = String.Empty;
            try
            {
                if (ftbShopDates != null && ftbShopDates.Count > 0)
                {
                    shopDateReport = GetShopDateContent(ftbShopDates);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in GetShopDatesTemplate", ex.Message + "Inner Exception" + ex.InnerException
                       + ", Stack Trace: " + ex.StackTrace + LogHelper.GetLogFilePath());
            }
            return shopDateReport;
        }

        private static string GetShopDateContent(List<FTBJobShopDateDetails> ftbShopDate)
        {
            string emailMessageBody = String.Empty;
            try
            {
                string emailTemplate = GetEmailTemplate(EmailPathConstants.EmailTemplatePath.FTBEmailTempPath, EmailConstants.EmailTemplates.ShopDateDetailsTemplate);
                foreach (var shopDate in ftbShopDate)
                {
                    emailMessageBody = emailMessageBody + emailTemplate;
                    if (!String.IsNullOrEmpty(emailMessageBody))
                    {
                        emailMessageBody = emailMessageBody.Replace(EmailConstants.EmailTemplatePlaceHolders.ShopDate, shopDate.ShopDate)
                     .Replace(EmailConstants.EmailTemplatePlaceHolders.ReservationCount, Convert.ToString(shopDate.ReservationCount))
                     .Replace(EmailConstants.EmailTemplatePlaceHolders.TargetAchieved, Convert.ToString(shopDate.TargetAchieved))
                      .Replace(EmailConstants.EmailTemplatePlaceHolders.RateIncreaseAchieved, Convert.ToString(shopDate.RateIncreaseAchieved));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in GetShopDateContent", ex.Message + "Inner Exception" + ex.InnerException
                    + ", Stack Trace: " + ex.StackTrace + LogHelper.GetLogFilePath());
            }
            return emailMessageBody;
        }

        static String GetEmailTemplate(string templatePath, string emailTemplateFileName)
        {
            string emailMessageBody = String.Empty;
            //string filePath = string.Empty;
            try
            {

                string emailTemplateFilePath = String.Empty;

                if (String.IsNullOrEmpty(emailMessageBody))
                {
                    string directoryLevel = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                    //directoryLevel = directoryLevel.Take(directoryLevel.Count() - 1).ToArray();

                    if (string.IsNullOrWhiteSpace(emailTemplateFilePath))
                    {
                        //if (directoryLevel.Length > 0)
                        //{
                        //    foreach (var folder in directoryLevel)
                        //    {
                        //        filePath += folder + "\\";
                        //    }
                        //}
                        emailTemplateFilePath = Path.GetDirectoryName(directoryLevel + "\\" + ConfigurationManager.AppSettings[templatePath] + "\\" + emailTemplateFileName + "\\");
                    }

                    LogHelper.WriteToLogFile("File Path is " + emailTemplateFilePath, LogHelper.GetLogFilePath());

                    if (File.Exists(emailTemplateFilePath))
                    {
                        StreamReader templateReader = File.OpenText(emailTemplateFilePath);
                        try
                        {
                            if (templateReader != null)
                            {
                                emailMessageBody = templateReader.ReadToEnd();
                            }
                        }
                        finally
                        {
                            templateReader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in GetEmailTemplate", ex.Message + "Inner Exception" + ex.InnerException
                      + ", Stack Trace: " + ex.StackTrace + LogHelper.GetLogFilePath());
            }

            return emailMessageBody;

        }

        public static MailMessage CreateEmailMessage(EmailRequest request)
        {
            MailMessage mailMessage = null;

            try
            {
                mailMessage = new MailMessage();
                if (request == null)
                {

                    return mailMessage;
                }


                MailAddress mailtoAddress = null;
                #region Region For Email Sender Address
                mailMessage = new MailMessage();

                if (request.Sender != null)
                    mailMessage.Sender = new MailAddress(request.Sender);

                #endregion

                #region Region For Email From Address
                mailMessage = new MailMessage();



                if (request.EmailFrom != null)
                    mailMessage.From = new MailAddress(request.EmailFrom);

                #endregion

                #region Region for Email To Address

                if (request.EmailTo.Count > 0)
                {
                    for (int index = 0; index < request.EmailTo.Count; index++)
                    {
                        if (!string.IsNullOrEmpty(request.EmailTo[index]))
                        {
                            mailtoAddress = new MailAddress(request.EmailTo[index]);
                            mailMessage.To.Add(mailtoAddress);
                        }
                    }
                }

                #endregion

                #region Region for Email Subject

                if (!string.IsNullOrEmpty(request.EmailSubject))
                    mailMessage.Subject = request.EmailSubject;

                #endregion

                #region Region for Email Body

                if (request.EmailBody != null)
                    mailMessage.Body = request.EmailBody;

                #endregion

                #region ConfigureMailMessage
                mailMessage.IsBodyHtml = request.IsHtml;
                #endregion

                #region Region for mail priority

                if (request.Priority != null)
                    mailMessage.Priority = request.Priority;

                #endregion

                #region Region for Sender

                if (request.Sender != null)
                    mailMessage.Sender = new MailAddress(request.Sender.ToString());

                #endregion


            }
            catch (System.Exception ex)
            {

            }
            return mailMessage;

        }
    }

    public static class EmailConstants
    {

        public struct EmailTemplates
        {
            public const string ShopDateDetailsTemplate = "ShopDateDetails.html";
            public const string GeneralTemplate = "GeneralTemplate.html";            
            public const string ShopDateMasterTemplate = "ShopDateMaster.html";
            public const string QuickViewTemplate = "QuickView.html";
        }

        public struct EmailTemplatePlaceHolders
        {
            //Shop Date wise Target Achived details

            public const string ShopDate = "#ShopDate#";
            public const string TargetAchieved = "#TargetAchieved#";
            public const string RateIncreaseAchieved = "#RateIncreaseAchieved#";
            public const string ReservationCount = "#ReservationCount#";
            public const string Message = "#Message#";

            public const string ShopDateList = "#PartialText#";
            //Common
            public const string UserName = "#UserName#";

            //FTBRates are not configured for FTB scheduled job details 
            public const string LocationBrand = "#LocationBrand#";
            public const string MonthYear = "#MonthYear#";
            public const string BlackOut = "#Blackout#";
        }

        public struct QuickViewPlaceHolders
        {
            public const string ShopDate = "#ShopDate#";
            public const string UserName = "#UserName#";
            public const string LocationBrand = "#LocationBrand#";
            public const string ChangeDates = "#ChangeDates#";
            public const string LOR = "#LOR#";
            public const string CarClasses = "#CarClasses#";
        }
    }
}

