using System.Collections.Generic;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;

namespace RateShopper.Services.Data
{
    public interface IRateCodeService : IBaseService<RateCode>
    {
        List<RateCodeDTO> GetAllRateCodes();
        RateCodeDTO GetRateCodeDetails(long rateCodeID);
        long SaveRateCode(RateCodeDTO objRateCodeDTO);
        bool DeleteRateCode(long rateCodeID, long userID);
        List<RateCodeDTO> GetRateCodesWithDateRanges();
        List<RateCodeDTO> GetApplicableRateCodesBetweenDateRange(DateTime startDate, DateTime endDate);
    }
}
