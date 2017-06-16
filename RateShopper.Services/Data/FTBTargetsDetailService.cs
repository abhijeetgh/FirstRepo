using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
namespace RateShopper.Services.Data
{
    public class FTBTargetsDetailService:BaseService<FTBTargetsDetail>,IFTBTargetsDetailService
    {
        public FTBTargetsDetailService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<FTBTargetsDetail>();
            _cacheManager = cacheManager;
        }
    }
}
