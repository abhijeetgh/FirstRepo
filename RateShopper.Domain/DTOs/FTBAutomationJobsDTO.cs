using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FTBAutomationJobsDTO
    {
        public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public string LocationBrandAlias { get; set; }
        public string CarClassIds { get; set; }
        public string CarClasses { get; set; }
        public string RentalLengthIds { get; set; }
        public string RentalLengths { get; set; }
        public string DaysOfWeek { get; set; }
        public string Status { get; set; }
        public string CreatedDateAsString { get; set; }
        public string NextRunDateAsString { get; set; }
        public DateTime DaysToRunStartDate { get; set; }
        public DateTime DaysToRunEndDate { get; set; }
        public DateTime? LastRunDateTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Month_Year { get; set; }
        public string RunDates { get; set; }
        public bool IsEnabled { get; set; }
        public bool ExecutionInProgress { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActiveTethering { get; set; }
        public bool IsStandardShop { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? NextRunDateTime { get; set; }
        public bool IsPresentNextRunDateTime { get; set; }
        public List<long> BrandLocationsLists { get; set; }
        public long CreatedByID { get; set; }
        public string ScrapperSourceIds { get; set; }
        public long Sources { get; set; }
        public bool IsSplitMonth { get; set; }        
        public string ShopStartDate { get; set; }
        public string ShopEndDate { get; set; }
        public string SplitAndSearchDetails { get; set; }
    }
    public class FTBAutomationScenarioDTO
    {
        public FTBAutomationScenarioDTO()
        {
            IsBlackoutDatesChange = false;
            IsMenualChangeJobStatus = false;
        }
        public long LocationBrandID { get; set; }
        public long FTBJobId { get; set; }
        public long AutomationJobID { get; set; }
        public bool IsFTBJob { get; set; }
        public bool IsBlackoutDatesChange { get; set; }
        public bool IsMenualChangeJobStatus { get; set; }
        public bool IsReturnMsg { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? BlackoutStartDate { get; set; }
        public DateTime? BlackoutEndDate { get; set; }
        public string ReturnMessage { get; set; }
        public long LoggedUserID { get; set; }
    }
}
