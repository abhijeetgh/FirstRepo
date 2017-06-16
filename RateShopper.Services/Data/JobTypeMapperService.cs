using System;
using System.Collections.Generic;
using System.Linq;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
using RateShopper.Domain.DTOs;
using RateShopper.Services.Helper;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Configuration;
using System.Text;

namespace RateShopper.Services.Data
{
    public class JobTypeMapperService : BaseService<JobTypeFrequencyMapper>, IJobTypeMapperService
    {
        public IScheduledJobFrequencyService _scheduleJobFrequencyService;
        public JobTypeMapperService(IEZRACRateShopperContext context, ICacheManager cacheManager, IScheduledJobFrequencyService scheduleJobFrequencyService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<JobTypeFrequencyMapper>();
            _cacheManager = cacheManager;
            this._scheduleJobFrequencyService = scheduleJobFrequencyService;
        }

        public List<JobFrequencyTypesDTO> GetJobFrequencyTypes(string jobType)
        {
            if (!string.IsNullOrEmpty(jobType))
            {
                List<JobFrequencyTypesDTO> jobFrequencyTypes = new List<JobFrequencyTypesDTO>();
                jobFrequencyTypes = (from jm in base.GetAll(false).Where(jm => jm.JobType.Equals(jobType,StringComparison.OrdinalIgnoreCase))
                                     join sc in _scheduleJobFrequencyService.GetAll(false) on jm.ScheduleJobFrequencyId equals sc.ID
                                     select new JobFrequencyTypesDTO { ID = sc.ID, Name = sc.Name, UIControlID = sc.UIControlID }).ToList();

                return jobFrequencyTypes;
            }
            return null;
        }
    }
}
