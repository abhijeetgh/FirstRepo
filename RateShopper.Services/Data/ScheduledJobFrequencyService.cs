using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
	public class ScheduledJobFrequencyService : BaseService<ScheduledJobFrequency>, IScheduledJobFrequencyService
	{
		public ScheduledJobFrequencyService(IEZRACRateShopperContext context, ICacheManager cacheManager)
			: base(context, cacheManager)
		{
			_context = context;
			_dbset = _context.Set<ScheduledJobFrequency>();
			_cacheManager = cacheManager;
		}

	}
}
