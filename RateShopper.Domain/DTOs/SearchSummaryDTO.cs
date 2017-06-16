using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    class SearchSummaryDTO
    {
    }
    public class SearchSummaryResult
    {
        public List<SearchSummeryData> lstSearchSummaryData { get; set; }
        public string lastModifiedDate { get; set; }
        public DateTime currentDateTime { get; set; }
    }
    public class SearchSummeryData
    {
        public SearchSummeryData()
        {
            IsFTBSummary = false;
            IsAutomationSummary = false;
        }
        public long SearchSummaryID { get; set; }
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Added for displaying server time
        /// </summary>
        public string StartTime { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Added for displaying server time
        /// </summary>
        public string EndTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LocationCode { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public long UserID { get; set; }
        public long StatusIDs { get; set; }
        public string StatusName { get; set; }
        public string SourcesIDs { get; set; }
        public string SourceName { get; set; }
        public string RentalLengthsIDs { get; set; }
        public string RentalLengthName { get; set; }
        public string CarClassIDs { get; set; }
        public string CarClassName { get; set; }
        public string LocationsBrandIDs { get; set; }
        public string LocationIDs { get; set; }
        public string LocationName { get; set; }
        public string FailureResponse { get; set; }
        public string BrandIDs { get; set; }
        public bool? IsQuickView { get; set; }
        public bool? IsGOV { get; set; }
        public bool? HasQuickViewChild { get; set; }
        public long ProviderId { get; set; }
        public long ShopRequestId { get; set; }
        public string SummaryText { get; set; }
        public string StatusClass { get; set; }
        public string SearchTypeClass { get; set; }
        public bool IsFTBSummary { get; set; }
        public bool IsAutomationSummary { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }

    public class SearchResultSuggestedRateDTO
    {
        public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        public long LocationID { get; set; }
        public long BrandID { get; set; }
        public long RentalLengthID { get; set; }
        public System.DateTime Date { get; set; }
        public long CarClassID { get; set; }
        public decimal BaseRate { get; set; }
        public Nullable<decimal> MinBaseRate { get; set; }
        public Nullable<decimal> MaxBaseRate { get; set; }
        public Nullable<decimal> TotalRate { get; set; }
        public Nullable<long> RuleSetID { get; set; }
        public string RuleSetName { get; set; }
        public Nullable<decimal> CompanyBaseRate { get; set; }
        public Nullable<decimal> CompanyTotalRate { get; set; }
    }

    public class SearchModelDTO
    {
        public string ScrapperSourceIDs { get; set; }
        public string LocationBrandIDs { get; set; }
        public string RentalLengthIDs { get; set; }
        public string CarClassesIDs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickUpTime { get; set; }
        public string DropOffTime { get; set; }
        public long CreatedBy { get; set; }
        public string ScrapperSource { get; set; }
        public string location { get; set; }
        public string CarClasses { get; set; }
        public string SelectedAPI { get; set; }
        public string DropOffLocationBrandID { get; set; }
        public string DropOffLocation { get; set; }
        public string VendorCodes { get; set; }
        public long SearchSummaryID { get; set; }
        public string PostData { get; set; }
        public long ScheduleJobId { get; set; }
        public string UserName { get; set; }
        public Nullable<long> ProviderId { get; set; }
        public bool IsGovShop { get; set; }
        public string ShopType { get; set; }
        public long FTBScheduledJobID { get; set; }
    }
}
