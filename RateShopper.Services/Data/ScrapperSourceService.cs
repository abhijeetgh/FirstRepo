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
    public class ScrapperSourceService : BaseService<ScrapperSource>, IScrapperSourceService
    {
        public ScrapperSourceService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScrapperSource>();
            _cacheManager = cacheManager;
        }

        public Dictionary<string, long> GetScrapperSourceDictionary()
        {
            return GetAll(false).Select(obj => new { Code = obj.Code, ID = obj.ID }).ToDictionary(obj => obj.Code, obj => obj.ID);
        }
    }
}
