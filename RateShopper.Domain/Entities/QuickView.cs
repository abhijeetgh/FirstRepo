using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class QuickView : BaseAuditableEntity
    {
        [Required]
        public long ParentSearchSummaryId { get; set; }        
        public Nullable<long> ChildSearchSummaryId { get; set; }
        [Required]
        public string CompetitorCompanyIds { get; set; }

        public string CarClassIds { get; set; }

        public Nullable<DateTime> LastRunDate { get; set; }
        
        public Nullable<DateTime> NextRunDate { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public bool IsExecutionInProgress { get; set; }

        [Required]
        public long StatusId { get; set; }

        public string UIControlId { get; set; }

        [Required]
        public bool MonitorBase { get; set; }

        public Nullable<bool> NotifyEmail { get; set; }

        public string PickupTime { get; set; }
        public string DropoffTime { get; set; }
    }
}
