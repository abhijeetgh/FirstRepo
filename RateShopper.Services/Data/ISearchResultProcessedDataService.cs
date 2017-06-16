using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ISearchResultProcessedDataService: IBaseService<SearchResultProcessedData>
    {
        List<SearchResultProcessedData> GetBySearchSummaryId(long _searchSummaryID);
    }
}
