using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface IScheduledJobService : IBaseService<ScheduledJob>
    {
        FirstTimeResult GetScheduledRuns(long scheduledJobId, bool newPage, long searchSummaryId);
        SearchResultDTO FilterSuggestedRates(long searchSummaryId, long locationId, long brandId, long rentalLengthId, string selectedDate, long carClassId, bool isDailyView);
        string SaveScheduledJob(AutoConsoleJobEditDTO autoConsoleJobEditDTO);
        List<AutomationConsoleViewJobsDTO> GetAllScheduledJobs(string userLocationBrandIds);
		void RunAutomationShops();
        bool MarkResultsReviewed(long searchSummaryId, long UserId);
        void SetNextRunDateTime(ScheduledJob scheduledJob);
        IEnumerable<ScheduledJobOpaqueValuesDTO> getScheduledJobOpaqueRates(long scheduledJobId);        
    }
}
