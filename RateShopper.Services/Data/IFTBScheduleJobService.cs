using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;


namespace RateShopper.Services.Data
{
    public interface IFTBScheduleJobService : IBaseService<FTBScheduledJob>
    {
        string SaveScheduledJob(FTBJobEditDTO ftbJobEditDTO);
        Task<List<FTBMonthsDTO>> GetLocationBrandJobMonths(long locationBrandId);
        Task<IEnumerable<FTBAutomationJobsDTO>> GetFTBAutomationJobList(long loggedUserId, long locationBrandId,bool isAdminUser);
        Task<string> RunFTBAutomationShops();
        void logFTBRateUpdate(long scheduledJobID, DateTime shopDate, long reservationCount, decimal target, long targetDetailsId);
        void SetNextRunDateTime(FTBScheduledJob ftbscheduledJob);
        //FtbJob operation
        FTBAutomationScenarioDTO CommonFTBJobUpdateScenarios(FTBAutomationScenarioDTO ftbAutomationScenarioDTO);
        Task<string> GetSearchDetails(long ftbJobId);
    }
}
