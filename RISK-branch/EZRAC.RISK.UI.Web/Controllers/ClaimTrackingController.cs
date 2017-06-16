using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    public class ClaimTrackingController : Controller
    {

        private ITrackingService _trackingService = null;
        
        public ClaimTrackingController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
            
        }
        //
        // GET: /ClaimTracking/
        public async Task<PartialViewResult> Index(long type, long claimId)
        {
            var trackingCategories = _trackingService.GetTrackingCategoriesAsync();
            var viewModel = TrackingHelper.GetClaimTrackingViewModel(trackingCategories, claimId);
            return PartialView("_TrackingTab", viewModel);
        }

        [HttpGet]
        public async Task<PartialViewResult> GetEventTrack(long type, long claimId)
        {
            var claimViewModel = new ClaimTrackingViewModel();

            var listDto = await _trackingService.GetAllTrackingsByTypeAsync(type, claimId);
            var trackingList = TrackingHelper.MapTrackingViewModel(listDto);
            claimViewModel.TrackingModel = trackingList;

            var timeTaken = await _trackingService.GetTotalTimeTakenAsync(claimId, type);
            claimViewModel.TotalTimeTaken = TrackingHelper.FormatTimeTaken(timeTaken);

            return PartialView("_TrackerGrid", claimViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEvent(long claimId, long trackingId, long type)
        {
            var userId = SecurityHelper.GetUserIdFromContext();
            await _trackingService.UpdateEventAsync(claimId, userId, trackingId, type);
            return RedirectToAction("GetEventTrack", new { type = type, claimId = claimId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UndoEvent(long claimTrackingId,long claimId,long type)
        {
            await _trackingService.UndoEventTrackingAsync(claimTrackingId);
            return RedirectToAction("GetEventTrack", new { type = type, claimId = claimId });
        }

        
    }
}