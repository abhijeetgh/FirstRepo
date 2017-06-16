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
    public interface ISearchResultsService : IBaseService<SearchResult>
    {
        List<SearchResult> GetBySeachSummaryID(long SeachSummaryID);
        Task<string> GenerateSeachResultProcesssedData(long searchSummaryID);
        List<ScrapperSourceDTO> GetScrapperSource(long UserId);
        List<LocationBrandModel> GetBrandLocation(long userId);
        List<CarClassDTO> GetCarClass();
        List<RentalLengthDTO> GetRentalLength();
        List<Company> GetCompany();
        SearchViewAppliedRuleSetDTO SearchViewAppliedRuleSet(long RuleSetID);
        List<long> GetLocationCarClasses(long locationBrandId);

        Task<SearchResultDTO> GetSearchGridDailyViewDataDefault(long? LoggedInUserID,bool isAdmin);
        Task<SearchResultDTO> GetSearchGridDailyViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string arrivalDate);
        Task<SearchResultDTO> GetSearchGridClassicViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string carClassId);
        string GetLastUpdateOnTSD(long searchSummaryID, long scrapperSourceID, long locationBrandID, long locationID, long brandID, long rentallengthID, long carClassId, string arrivalDate, string view);

        void BulkInsert(List<MapTableResult> searchResult);
        decimal CalculateSuggestedBaseRate(long locationBrandID, Formula objFormula, decimal totalRate, string rentalLengthCode, string rangeInterval, bool isGOV = false);
        decimal CalculateSuggestedTotalRate(long locationBrandID, Formula objFormula, decimal baseRate, string rentalLengthCode, string rangeInterval, bool isGOV = false);
        System.Data.DataTable GetSearchShopDetailsForCSV(long searchSummaryID);

        Task<SearchResultDTO> GetFTBSummaryReportDefault(long? LoggedInUserID, bool isAdmin);
        Task<SearchResultDTO> GetFTBSummaryReport(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID);
    }
}
