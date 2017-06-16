using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class QuickViewEmailDTO
    {
        public string BrandLocation { get; set; }
        public string ShopDate { get;set; }
        public string ChangeDates { get; set; }
        public string LOR { get; set; }
        public string CarClasses { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> EmailRecipients { get; set; }
        public IList<string> BccEmailRecipients { get; set; }        
    }
}
