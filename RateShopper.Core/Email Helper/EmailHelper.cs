using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Core.Email_Helper
{
    public static class EmailHelper
    {
        /// <summary>
        /// Method for sending the mail
        /// </summary>
        /// <param name="email">MailMessage containing all the details of the mail</param>
        /// <returns></returns>
        public static async Task<bool> SendEmailAsync(MailMessage mailMessage)
        {
            bool result = new bool();

            if (mailMessage == null)
            {

                return result;
            }

            try
            {
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    result = true;
                    return result;
                }
            }
            catch (System.Exception ex)
            {

                return result;
            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        /// <summary>
        /// Method is used to build MailMessage object. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                MailAddress mailCC = null;
                MailAddress mailBCC = null;

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

                #region Region for Email CC Address

                if (request.CCEmailAddress != null)
                {
                    for (int index = 0; index < request.CCEmailAddress.Count; index++)
                    {
                        if (!string.IsNullOrEmpty(request.CCEmailAddress[index]))
                        {
                            mailCC = new MailAddress(request.CCEmailAddress[index]);
                            mailMessage.CC.Add(mailCC);
                        }
                    }
                }

                #endregion

                #region Region for Email BCC Address

                if (request.BccEmailAddress != null)
                {
                    for (int index = 0; index < request.BccEmailAddress.Count; index++)
                    {
                        if (!string.IsNullOrEmpty(request.BccEmailAddress[index]))
                        {
                            mailBCC = new MailAddress(request.BccEmailAddress[index]);
                            mailMessage.Bcc.Add(mailBCC);
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

        /// <summary>
        /// Method for sending the mail
        /// </summary>
        /// <param name="email">MailMessage containing all the details of the mail</param>
        /// <returns></returns>
        public static bool SendEmail(MailMessage mailMessage)
        {
            bool result = new bool();

            if (mailMessage == null)
            {

                return result;
            }

            try
            {

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.SendAsync(mailMessage, mailMessage);
                result = true;
                return result;

            }
            catch (System.Exception ex)
            {

                return result;
            }
        }
    }
}
