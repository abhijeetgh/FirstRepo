using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IScheduledJobTetheringsService : IBaseService<ScheduledJobTetherings>
    {
        string SaveScheduledJobTethering(ScheduledJobTetheringsDTO ScheduledJobTetherings, long scheduledJobID);
        ScheduledJobTetheringsDTO GetSelectedJobTetheringData(long ScheduledJobID);
        bool DeleteScheduleJobTetheringData(long ScheduledJobID);
    }
}
