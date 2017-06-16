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
    public class RuleSetGapSettingService : BaseService<RuleSetGapSetting>, IRuleSetGapSettingService
    {
        public RuleSetGapSettingService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RuleSetGapSetting>();
            _cacheManager = cacheManager;
        }

    }
}
