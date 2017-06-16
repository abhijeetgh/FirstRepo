using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class JobHistoryDTO
    {
        public string RunDate { get; set; }
        public string RunTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long SearchSummaryID { get; set; }
        public string RentalLengthIds { get; set; }
        public string LocationBrandIds { get; set; }
        public string ScrapperSourceIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long BrandId { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsReviewed { get; set; }
        public long StatusId { get; set; }
        public string Response { get; set; }
        public string CarClassIds { get; set; }
        public bool IsGOVShop { get; set; }
    }
    public class ResultDays
    {
        public string StartDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string StartDateVal { get; set; }
    }
    public class RentalLengthsFilter
    {
        public long RentalLengthId { get; set; }
        public string RentalLength { get; set; }
    }
    public class ScrapperSourceFilter
    {
        public string Source { get; set; }
        public long ScrapperSourceId { get; set; }
    }
    public class LocationFilter
    {
        public long BrandId { get; set; }
        public string Code { get; set; }
        public long LocationId { get; set; }
    }
    public class CarClassFilter
    {
        public long ID { get; set; }
        public string Code { get; set; }
    }
    public class MarkResultFilters
    {
        public List<ResultDays> daysFilter;
        public List<RentalLengthsFilter> lorFilters;
        public List<ScrapperSourceFilter> scrapperSources;
        public List<LocationFilter> locations;
        public List<CarClassFilter> carClassFilters;
    }

    public class FirstTimeResult
    {
        public List<JobHistoryDTO> jobs;
        //public SearchSuggestedRateMaster suggestedRateMaster;
        public JobHistoryDTO resultFilters;
        public MarkResultFilters allFilters;
        public SearchResultDTO searchResults;
    }
}
