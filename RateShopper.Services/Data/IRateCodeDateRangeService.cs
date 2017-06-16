using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IRateCodeDateRangeService:IBaseService<RateCodeDateRange>
    {
        void SaveRateCodeDateRange(IEnumerable<RateCodeDateRangeDTO> tempDateRangeList, long ratecodeID);
        List<RateCodeDateRangeDTO> GetRateCodeDateRangeDetails(long rateCodeID);
        void DeleteRateCodeDateRange(IEnumerable<long> deletedItemsId);
    }
}
