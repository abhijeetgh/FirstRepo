using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FTBJobShopDateDetails
    {
        public string ShopDate { get; set; }
        public decimal TargetAchieved { get; set; }
        public decimal RateIncreaseAchieved { get; set; }
        public long ReservationCount { get; set; }
        public DateTime ShopDay { get; set; }
    }
    public class FTBEmailCommonSettings
    {
        public string UserName { get; set; }
        public string MonthYear { get; set; }
        public string LocationBrand { get; set; }
        public string assignedToEmail { get; set; }
        public IList<string> EmailRecipients { get; set; }
        public IList<string> BccEmailRecipients { get; set; }
        public string Subject { get; set; }
        public string BlackoutPeriod { get; set; }
    }
}
