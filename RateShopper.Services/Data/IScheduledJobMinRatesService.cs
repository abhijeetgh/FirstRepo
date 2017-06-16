using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IScheduledJobMinRatesService : IBaseService<ScheduledJobMinRates>
    {
        string SaveScheduledJobMinRates(List<ScheduledJobMinRates> lstScheduledJobMinRates, long ScheduleJobID);
        List<MinRatesDTO> preLoadMinRates(long[] carClassIds, string locationBrandId, long selectedJobId);

        List<ScheduledJobMinRates> GetSelectedMinRate(long ScheduleJobID);
    }
}
