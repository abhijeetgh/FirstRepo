using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{

    public class QuickViewDTO
    {
        public long ID { get; set; }
        public long ParentSearchSummaryId { get; set; }
        public long ChildSearchSummaryId { get; set; }
        public string CompetitorCompanyIds { get; set; }
        public DateTime LastRunDate { get; set; }
        public DateTime NextRunDate { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public bool IsExecutionInProgress { get; set; }
        public long StatusId { get; set; }
        public string UIControlId { get; set; }
        public string CarClassIds { get; set; }
        public bool IsTotal { get; set; }
        public bool IsEmailNotification { get; set; }
        public string PickupTime { get; set; }
        public string DropOffTime { get; set; }
        public List<QuickViewGroupItem> lstQuickViewGroupItem { get; set; }
    }
    public class QuickViewReportDTO
    {
        public List<RentalLenghts> LORs { get; set; }
        public List<DateRows> Dates { get; set; }
    }
    public class QuickViewGridDTO
    {
        public long ID { get; set; }
        public long SearchSummaryId { get; set; }
        public long? ChildSummaryId { get; set; }
        public string CarClassesIDs { get; set; }
        public string RentalLengthIDs { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool ExecutionInProgress { get; set; }
        public string CreatedByUserName { get; set; }
        public bool IsPresentNextRunDateTime { get; set; }
        public long CreatedByID { get; set; }
        public Nullable<DateTime> LastRunDateTime { get; set; }
        public Nullable<DateTime> NextRunDateTime { get; set; }
        public string Competitors { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public List<long> LocationBrands { get; set; }
        public string Sources { get; set; }
        public string LocationsBrandIDs { get; set; }
        public string TrackingCarClassIds { get; set; }
        public bool? IsReportReviewed { get; set; }
    }

    public class RentalLenghts
    {
        public string RentalLength { get; set; }
        public long RentalLengthId { get; set; }
    }

    public class DateRows
    {
        public string Date { get; set; }
        public List<QuickViewRow> QuickViewResult { get; set; }
    }
    public class QuickViewRow
    {
        public long ID { get; set; }
        public long SearchSummaryId { get; set; }
        public long QuickViewId { get; set; }
        public long ScrapperSourceID { get; set; }
        public long LocationBrandID { get; set; }
        public long RentalLengthId { get; set; }
        //public string Date { get; set; }
        public bool? IsMovedUp { get; set; }
        public bool? IsReviewed { get; set; }
        public string FormattedDate { get; set; }
        public bool IsPositionChange { get; set; }
    }

    public class QuickViewCarClassGroups
    {
        public long GroupID { get; set; }
        public long CarClassID { get; set; }
    }

    public class QuickViewGroupCompaniesDTO
    {
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
    }
    public class QuickViewGapDevSettingsDTO
    {
        public long CompetitorGroupId { get; set; }
        public long CarClassGroupId { get; set; }
        public decimal GapValue { get; set; }
        public decimal DeviationValue { get; set; }
    }
    public class QuickViewGroupData
    {
        public bool IsTotal { get; set; }
        public bool IsEmailNotification { get; set; }
        public string PickupTime { get; set; }
        public string DropOffTime { get; set; }
        public List<QuickViewGroupItem> GroupData { get; set; }
    }
    public class QuickViewGroupItem
    {
        public QuickViewGroupItem()
        {
            IsNewGroup = false;
        }
        public string DeleteGroupId { get; set; }
        public bool IsNewGroup { get; set; }
        public long groupId { get; set; }
        public string CompetitorIds { get; set; }
        public string DeleteCompetitorIds { get; set; }
        public string AddCompetitorIds { get; set; }
        public List<QuickViewGroupCompany> lstQuickViewGroupCompany { get; set; }
        public List<QuickViewCarClassGroup> QuickViewCarClassGroups { get; set; }
    }
    public class QuickViewGroupCompany
    {
        public long groupId { get; set; }
        public long CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
    public class QuickViewCarClassGroup
    {
        public long groupId { get; set; }
        public long Id { get; set; }
        //public decimal GapValueGroup { get; set; }
        public decimal DeviationValueGroup { get; set; }
    }
}
