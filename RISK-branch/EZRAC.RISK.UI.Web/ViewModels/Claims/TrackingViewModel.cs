using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class TrackingViewModel
    {
        public long Id { get; set; }
        public long ClaimTrackingId { get; set; }
        public long TrackingTypeId { get; set; }
        public string TrackingDescription { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
        public string CreatedBy { get; set; }
        public bool CanUndo { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long ClaimId { get; set; }
        public string TimeTaken { get; set; }
        public string TotalTimeTaken { get; set; }
    }
}