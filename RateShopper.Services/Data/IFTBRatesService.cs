using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IFTBRatesService : IBaseService<FTBRate>
    {
        FTBRatesDTO GetFTBRates(long brandLocationID, long rentalLengthId, int year, int month, int selectedFTBRateId);
        FTBCustomRateDTO SaveFTBRates(FTBRatesDTO objFTBRatesDTO);
        Task<List<FTBRateDetailsDTO>> fetchFTBRatesDetail(long ftbRatesId);
        Task<int> CopyFTBRates(FTBCopyMonthsDTO objFTBCopyMonthsDTO, bool isRequestFromFTBSchedular);
        List<int> GetYears();
    }
}
