using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;

namespace RateShopper.Services.Data
{
    public class SearchResultRawDataService : BaseService<SearchResultRawData>, ISearchResultRawDataService
    {
        public SearchResultRawDataService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<SearchResultRawData>();
            _cacheManager = cacheManager;
        }

        public void SaveRawData(long searchSummaryId, string response)
        {
            try
            {
                SearchResultRawData rawData = new SearchResultRawData
                {
                    SearchSummaryID = searchSummaryId,
                    JSON = response,
                    CreatedDateTime = DateTime.Now
                };
                Add(rawData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
