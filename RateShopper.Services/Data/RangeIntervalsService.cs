﻿using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class RangeIntervalsService : BaseService<RangeInterval>, IRangeIntervalsService
    {
        public RangeIntervalsService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RangeInterval>();
            _cacheManager = cacheManager;
        }
    }
}
