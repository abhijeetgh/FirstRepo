using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class LocationBrandDetailsDTO
    {
        public long ID { get; set; }
        public long LocationID { get; set; }
        public string Code { get; set; }
        public long BrandId { get; set; }
        public decimal? WeeklyExtraDenominator { get; set; }
        public decimal? DailyExtraDayFactor { get; set; }
        public string BrandLocation { get; set; }
        public long? PricingManager { get; set; }
    }

    public class TSDParams
    {

        [JsonProperty(PropertyName = "strStartDate")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "strEndDate")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "strBrandLocation")]
        public string BrandLocation { get; set; }
    }

}
