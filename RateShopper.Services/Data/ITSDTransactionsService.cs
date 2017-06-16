using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface ITSDTransactionsService : IBaseService<TSDTransaction>
    {
        void SaveTSDTransaction(long LocationBrandID, long SearchSummaryID, string ResponseCode, string Message,
            string XMLRequest, string RequestStatus, string XMLResponse, string ErrorFound, long UserId, bool isRezCentralUpdate, bool isOpaqueUpdate);

        Task<string> ProcessRateSelection(List<TSDModel> tsdModels, string UserName, string RateSystem, long LocationBrandID,
            long UserId, long SearchSummaryID, bool IsTetheredUpdate, string BrandLocation, bool isUpdateSuggestedRate = true, bool isFTBRateUpdate = false);

        Task<string> PostTSDRates(TSDPostRatesDTO objTSDPostRatesDTO, bool isRezCentralUpdate, bool isOpaqueUpdate);

        string GenerateXML(List<TSDModel> tsdModelList, string UserName, string RateSystem, long LocationBrandID, long UserId, bool IsTetheredUpdate, long SearchSummaryID, bool isFTBRateUpdate = false);
        //,List<TSDModel> tetheredRates

        List<TSDDTO> GetTSDAuditLogs(long brandID);

        List<CompanyDTO> GetCompany();
        TSDDTO GetLogDetail(long auditID);
        void UpdatedSuggestedRate(List<TSDModel> tsdModelList, long searchSummaryID, long userID);

        string GetLastTSDUpdate(long searchSummaryId);

        string updateDateTimeStamp(DateTime[] arrivalDates, long rentalLengthId, long searchSummaryId, List<long> carClasses, long userID);

        List<GlobalLimitDetailsDTO> getGlobalLimits(long locationBrandId, DateTime StartDate, DateTime EndDate);

        Formula getLocationFormula(long locationBrandId);

        Task<string> PushRezCentralRates(RezCentralDTO objRezCentralDTO);
        Task PushOpaqueRates(OpaqueRatesConfiguration opaqueRatesConfiguration, List<TSDModel> lstTSDData, string UserName, string RateSystem,
            long LocationBrandID, long UserId, long SearchSummaryID, bool IsTetheredUpdate, string BrandLocation);

        string GetApplicableRateCodes(List<RateCodeDTO> rateCodesWithDateRange, long brandId, DateTime date);

        Task<IEnumerable<RezPreloadRateDTO>> GetRatesFromPreloadAPI(RezPreloadPayloadDTO objPayload);
    }
}
