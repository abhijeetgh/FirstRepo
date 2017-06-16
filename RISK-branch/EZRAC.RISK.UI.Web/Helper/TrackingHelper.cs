using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    internal static class TrackingHelper
    {
        internal static string FormatTimeTaken(TimeSpan span)
        {
            string timeTaken = string.Empty;
            if (span != null && span.Ticks > 0)
                timeTaken = string.Format("{0} days {1} hour {2} min", span.Days, span.Hours, span.Minutes);
            return timeTaken;
        }

        internal static IEnumerable<TrackingViewModel> MapTrackingViewModel(IEnumerable<TrackingDto> trackingDto)
        {
            var viewModel = new List<TrackingViewModel>();
            TrackingViewModel trackingModel = null;

            foreach (var item in trackingDto)
            {
                trackingModel = new TrackingViewModel();
                trackingModel.Id = item.Id;
                trackingModel.ClaimTrackingId = item.ClaimTrackingId;
                trackingModel.IsCompleted = item.IsCompleted;
                trackingModel.IsCurrent = item.IsCurrent;
                trackingModel.TrackingDescription = item.TrackingDescription;
                trackingModel.TrackingTypeId = item.TrackingTypeId;
                trackingModel.CreatedBy = item.CreatedBy;
                trackingModel.CreatedDateTime = item.CreatedDateTime;
                trackingModel.TimeTaken = FormatTimeTaken(item.TimeTaken);
                trackingModel.CanUndo = item.CanUndo;
                trackingModel.ClaimId = item.ClaimId;
                viewModel.Add(trackingModel);
            }
            return viewModel;
        }


        internal static IEnumerable<TrackerCategoryViewModel> MapTrackingCategoriesViewModel(IEnumerable<TrackingTypeDto> trackingTypeDto)
        {
            var viewModel = new List<TrackerCategoryViewModel>();
            TrackerCategoryViewModel trackingType = null;

            foreach (var item in trackingTypeDto)
            {
                trackingType = new TrackerCategoryViewModel();
                trackingType.Id = item.Id;
                trackingType.Key = item.Key;
                trackingType.Category = item.Category;
                viewModel.Add(trackingType);
            }
            return viewModel;
        }

        internal static ClaimTrackingViewModel GetClaimTrackingViewModel(IEnumerable<TrackingTypeDto> trackerCategories, long claimId)
        {
            var claimViewModel = new ClaimTrackingViewModel();
            claimViewModel.TrackingModel = new List<TrackingViewModel>();
            claimViewModel.ClaimId = claimId;
            claimViewModel.TrackingCategories = TrackingHelper.MapTrackingCategoriesViewModel(trackerCategories);
            return claimViewModel;
        }
    }
}