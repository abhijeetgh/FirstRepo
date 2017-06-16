using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RateShopper.ScrapperAPIService.Models
{
    public class ScrapperResult
    {
        public string CityCd { get; set; }
        public string DataSource { get; set; }
        public long Lor { get; set; }
        public DateTime ArvDt { get; set; }
        public DateTime RtrnDt { get; set; }
        public string VendCd { get; set; }
        public string CarTypeCd { get; set; }
        public decimal RtAmt { get; set; }
        public decimal EstRentalChrgAmt { get; set; }
        public int SearchSummaryID { get; set; }
    }
    //public class MapTableResult
    //{
    //    public long ID { get; set; }
    //    public long SearchSummaryID { get; set; }
    //    public long ScrapperSourceID { get; set; }
    //    public long LocationID { get; set; }
    //    public long RentalLengthID { get; set; }
    //    //public System.DateTime Date { get; set; }
    //    public long CarClassID { get; set; }
    //    public long CompanyID { get; set; }
    //    public Nullable<decimal> BaseRate { get; set; }
    //    public Nullable<decimal> TotalRate { get; set; }
    //    public DateTime UpdatedDateTime { get; set; }
    //    public DateTime ArvDt { get; set; }
    //    public DateTime RtrnDt { get; set; }
    //}
    public class Rate
    {
        public List<ScrapperResult> Rates { get; set; }
        public List<SearchFail> Searches { get; set; }

    }
    public class FailedSearch
    {
        public List<SearchFail> Searches { get; set; }
    }

    public class SearchFail
    {
        public int SearchSummaryID { get; set; }
        public string reason { get; set; }
    }
}
