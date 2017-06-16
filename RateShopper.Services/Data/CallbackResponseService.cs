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
    public class CallbackResponseService : BaseService<CallbackResponse>, ICallbackResponseService
    {
        public CallbackResponseService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<CallbackResponse>();
            _cacheManager = cacheManager;
        }

        public void SaveResponse(long searchSummaryId, long shopRequestId, string rawData)
        {
            CallbackResponse objCallbackResponseEntity = new CallbackResponse();
            objCallbackResponseEntity.RawData = rawData;
            objCallbackResponseEntity.SearchSummaryId = searchSummaryId;
            objCallbackResponseEntity.ShopRequestId = shopRequestId;
            objCallbackResponseEntity.CreatedDate = DateTime.Now;

            Add(objCallbackResponseEntity);
        }
    }
}
