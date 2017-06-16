using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ISearchJobUpdateAllService : IBaseService<SearchJobUpdateAll>
    {
        List<SearchJobUpdateAll> GetPreloaBaseRate(long SearchSummaryId, bool isDailyPreLoad);
        string InsertUpdateTSDUpdateAll(long SearchSummaryId, List<SearchJobUpdateAllDTO> SearchJobUpdateAll);
    }
}
