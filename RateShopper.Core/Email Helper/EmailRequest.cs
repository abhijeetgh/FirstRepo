using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Core.Email_Helper
{
    public class EmailRequest
    {
        public EmailRequest()
        {
            this.EmailTo = new List<string>();
            this.CCEmailAddress = new List<string>();
            this.BccEmailAddress = new List<string>();
        }

        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        /// <summary>
        /// Used to set the sender for delivery report
        /// </summary>
        public string Sender { get; set; }
        public string EmailBody { get; set; }
        public IList<string> EmailTo { get; private set; }
        public IList<string> CCEmailAddress { get; private set; }
        public IList<string> BccEmailAddress { get; private set; }
        public bool IsHtml { get; set; }
        public MailPriority Priority { get; set; }
    }
}
