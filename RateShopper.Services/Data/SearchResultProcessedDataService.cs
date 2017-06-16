using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class SearchResultProcessedDataService: BaseService<SearchResultProcessedData>, ISearchResultProcessedDataService
    {
        public SearchResultProcessedDataService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<SearchResultProcessedData>();
            _cacheManager = cacheManager;
        }

        public List<SearchResultProcessedData> GetBySearchSummaryId(long _searchSummaryID)
        {
            string cacheKey = "SearchResultProcessedDataBySearchSummaryId_" + _searchSummaryID;
            List<SearchResultProcessedData> entity = _cacheManager.Get(cacheKey) as List<SearchResultProcessedData>;
            if (entity == null)
            {
                entity = _context.SearchResultProcessedDatas.Where(obj => obj.SearchSummaryID == _searchSummaryID).ToList();
                _cacheManager.Set(cacheKey, entity, DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["SearchCacheMinutes"])));
            }
            return entity;            
        }
    }
}
