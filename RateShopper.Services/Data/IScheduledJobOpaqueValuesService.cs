using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IScheduledJobOpaqueValuesService : IBaseService<ScheduledJobOpaqueValues>
    {
        void SaveOpaqueValues(List<ScheduledJobOpaqueValuesDTO> opaqueValues, long scheduledJobId);
        void RemoveAllOpaqueValues(long scheduledJobId);
    }
}
