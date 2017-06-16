using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RezCentralDTO
    {
        public List<RezCentralLocationBrandDTO> Locations { get; set; }
        public string System { get; set; }
        public string RateCodes { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsOpenEndedRates { get; set; }
        public string Days1 { get; set; }
        public string Days2 { get; set; }
        public List<RezCentralRatesDTO> Rates { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
    }

    public class RezCentralRatesDTO
    {
        public string CarClass { get; set; }
        public string CarClassId { get; set; }
        public decimal DailyLeftRate { get; set; }
        public decimal WeeklyLeftRate { get; set; }
        public decimal DailyRightRate { get; set; }
        public decimal WeeklyRightRate { get; set; }
    }

    public class RezCentralLocationBrandDTO
    {        
        public string LocationBrand { get; set; }
        public string Location { get; set; }
        public long LocationBrandId { get; set; }
    }
}
