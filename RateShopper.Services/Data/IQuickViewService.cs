using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IQuickViewService : IBaseService<QuickView>
    {
        QuickViewDTO SaveQuickView(long searchSummaryId, string scheduleTime, string competitorsIds, long userId, string clientUTCDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData, long quickViewId = 0);
        List<QuickViewGridDTO> GetQuickViews(long userId, string brandLocationIds, string clientCurrentUTCDate);
        DateTime GetESTDate(string utcDate);
        QuickViewGridDTO UpdateQuickView(long quickViewId, string scheduleTime, string competitorsIds, long userId, string clientUTCDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData);
        bool DeleteQuickView(long quickViewId, long userId, string clientUTCDate);
        Task<string> RunQuickViewShop();
        void SetQuickViewReview(DateTime SearchDate, long rentalLengthID, long searchSummaryId);

        QuickViewDTO GetQuickViewDetails(long searchSummaryId);
    }
}
