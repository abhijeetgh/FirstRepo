using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class SearchResultDTO
    {
        public long SearchSummaryId { get; set; }
        public long brandID { get; set; }
        public string finalData { get; set; }
        public List<SearchResultSuggestedRateDTO> suggestedRate { get; set; }
        public string lastTSDUpdated { get; set; }
    }

    public class SearchShopCSVDTO
    {
        public string Length { get; set; }
        public string Car_Class { get; set; }
        public string Date { get; set; }
        public string Vendor { get; set; }
        public decimal Base_Rate { get; set; }
        public decimal? Additional_Base { get; set; }
        public decimal? Total_Rate { get; set; }
        public decimal? Suggested_Base_Rate { get; set; }
        public decimal? Suggested_Total_Rate { get; set; }
        public decimal? Suggested_Min_Base_Rate { get; set; }
        public decimal? Suggested_Max_Base_Rate { get; set; }
        public string LocationBrandId { get; set; }
        public bool? isGOV { get; set; }
    }
    public class SearchSuggestedRate
    {
        public long SuggestedRateId { get; set; }
        public string Day { get; set; }
        public string CarClassCode { get; set; }
        public string CarClassLogo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal? TotalRate { get; set; }
        public decimal? CompanyBaseRate { get; set; }
        public decimal? CompanyTotalRate { get; set; }
        public DateTime DaySelected { get; set; }

        public decimal? TetherBaseRate { get; set; }
        public decimal? TetherTotalRate { get; set; }
    }

    public class SearchSuggestedRateMaster
    {
        //public string CompanyName { get; set; }
        //public List<SearchSuggestedRate> SearchSuggestedRates { get; set; }
        public string finalData { get; set; }
        public List<SearchResultSuggestedRateDTO> suggestedRate { get; set; }
    }
}
