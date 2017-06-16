using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class AutomationConsoleViewJobsDTO
    {
        public long Id { get; set; }
        public string LocationBrands { get; set; }
        public string CarClasses { get; set; }
        public string RentalLengths { get; set; }
        public string DaysOfWeek { get; set; }
        public bool IsWideGap { get; set; }
        public bool IsGovTemplate { get; set; }
        public bool IsGov { get; set; }
        public string Status { get; set; }
        public string CreatedDateAsString { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ShopDates { get; set; }
        public string RunDates { get; set; }
        public bool IsEnabled { get; set; }
        public bool ExecutionInProgress { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActiveTethering { get; set; }
        public bool IsStandardShop { get; set; }
        public string CreatedBy { get; set; }
        public bool IsPresentNextRunDateTime { get; set; }
        public List<long> BrandLocationsLists { get; set; }
        public bool AreReviewButtonsRequired { get; set; }
        public long CreatedByID { get; set; }
        public bool IsReviewPending { get; set; }
        public string Source { get; set; }
        public bool IsStopByFTB { get; set; }
        public bool CanInititateShop { get; set; }
        public bool IsInitiateShopEnable { get; set; }
        public bool IsShopComplete { get; set; }
        public string SearchSummaryStatus { get; set; }
        public long ProviderId { get; set; }
        public string ScrapperSourceIDs { get; set; }
        public string RentalLengthIDs { get; set; }
        public string StartDate { get; set; }
        public bool IsReadOnly { get; set; }
        public string EndDate { get; set; }

        public long SearchSummaryId { get; set; }

        public long ShopCreatedBy { get; set; }

        public string LastRunDate { get; set; }
        public string NextRunDate { get; set; }
    }
}
