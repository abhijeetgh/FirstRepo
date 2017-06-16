using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface ITrackingService
    {
        Task<IEnumerable<TrackingDto>> GetAllTrackingsByTypeAsync(long trackingType, long claimId);

        Task<bool> UpdateEventAsync(long claimId, long userId, long trackingId, long type);

        Task<bool> UndoEventTrackingAsync(long claimTrackingId);

        Task<TimeSpan> GetTotalTimeTakenAsync(long claimId, long type);

        IEnumerable<TrackingTypeDto> GetTrackingCategoriesAsync();

    }
}
