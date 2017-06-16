using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface ISearchSummaryService : IBaseService<SearchSummary>
    {
        Dictionary<long, string> GetFailedRequestCount();
        void SendAsync();
        Task<SearchSummaryResult> GetSearchSummaryData(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int lastDaysRecord, DateTime UserSystemDate, bool isAdminUser, long selectedUserId);
        Task<SearchSummaryResult> GetFTBSearchSummaryData(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int lastDaysRecord, DateTime UserSystemDate, bool isAdminUser, long selectedUserId);
        Task<string> SendRequestToAPI(long SearchSummaryID, string requestURL, SearchSummary searchSummarized);
        Task<string> SaveSearchSummary(SearchModelDTO searchModel);
        SearchModelDTO SaveSearchSummary(SearchModelDTO searchModel, string requestUrl);
        long GetSummaryIdByShopId(long shopRequestId);
        bool SendRequestToAPISync(long SearchSummaryID, string requestURL, SearchSummary searchSummarized);

        Dictionary<string, string> GetSearchShopSummaryForCSV(long searchSummaryID);


        bool InitializeAutomationSearch(ScheduledJob scheduledJob, string requestURL, DateTime startDate, DateTime endDate);
        List<GlobalLimitDetailsDTO> GetGlobalLimitTetherShop(long SearchSummaryID, long DependantBrandID, long LocationID);
        void UpdateSearchStatus(long searchSummaryId, long shopRequestId = 0, bool isRequestFromAPI = false, bool isSearchFailed = false, string response = "");

        void UpdateQuickViewStatus(long searchSummaryId, long userId);
        Task<string> InitiateQuickViewSearch(long searchSummaryId, long userId, bool isAutomatedQuickView = false);

        bool InitializeRateHighwayAutomationSearch(ScheduledJob scheduledJob, string requestUrl, DateTime startDate, DateTime endDate, string apiCode);

        Task<SearchSummaryResult> GetSearchSummaryDataFromSP(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId);
        Task<SearchSummaryResult> GetFTBSearchSummaryDataFromSP(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId);
    }
}
