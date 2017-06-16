using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
namespace RateShopper.Services.Data
{
    public class FTBRatesSplitMonthsService : BaseService<FTBRatesSplitMonthDetails>, IFTBRatesSplitMonthsService
    {
        public FTBRatesSplitMonthsService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<FTBRatesSplitMonthDetails>();
            _cacheManager = cacheManager;
        }
    }
}
