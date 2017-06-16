using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ClaimTrackingViewModel
    {
        public IEnumerable<TrackingViewModel> TrackingModel { get; set; }
        public string TimeTaken { get; set; }
        public long ClaimId { get; set; }
        public string TotalTimeTaken { get; set; }
        public IEnumerable<TrackerCategoryViewModel> TrackingCategories { get; set; }
    }
}