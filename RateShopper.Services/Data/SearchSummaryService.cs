using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Globalization;
using RateShopper.Services.Helper;
using RateShopper.Domain.DTOs;
using System.IO;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Collections;

namespace RateShopper.Services.Data
{
    public class SearchSummaryService : BaseService<SearchSummary>, ISearchSummaryService
    {
        IStatusesService _statusService;
        ICompanyService _compnayService;
        ICarClassService _carClassService;
        IRentalLengthService _rentalLengthService;
        ILocationService _locationService;
        IScrapperSourceService _scrapperSourceService;
        ILocationBrandService _locationBrandService;
        IUserService _userService;
        IGlobalLimitService _globalLimitService;
        IProvidersService _providerService;
        ICallbackResponseService _iCallbackResponseService;
        IScrappingServersService _iScrappingServersService;
        //ISearchResultsService _searchResultService;
        public SearchSummaryService(IEZRACRateShopperContext context, ICacheManager cacheManager, IStatusesService statusService,
            ICompanyService compnayService, ICarClassService carClassService, IRentalLengthService rentalLengthService, ILocationService locationService,
            IScrapperSourceService scrapperSourceService, ILocationBrandService locationBrandService, IUserService userService,
            IGlobalLimitService globalLimitService, IProvidersService providerService, ICallbackResponseService callbackResponseService, IScrappingServersService scrappingServersService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<SearchSummary>();
            _cacheManager = cacheManager;
            _statusService = statusService;
            _compnayService = compnayService;
            _carClassService = carClassService;
            _rentalLengthService = rentalLengthService;
            _locationService = locationService;
            _scrapperSourceService = scrapperSourceService;
            _locationBrandService = locationBrandService;
            _userService = userService;
            _globalLimitService = globalLimitService;
            _providerService = providerService;
            _iCallbackResponseService = callbackResponseService;
            _iScrappingServersService = scrappingServersService;
        }


        public Dictionary<long, string> GetFailedRequestCount()
        {
            //TO DO read from app config
            int RetryCount = Convert.ToInt32(ConfigurationManager.AppSettings["RetryCount"]);

            Dictionary<long, string> failedRequests = new Dictionary<long, string>();

            try
            {
                failedRequests = _context.SearchSummaries.Join(_context.StatuseDesc, Search => Search.StatusID, ST => ST.ID, (Search, ST)
                                        => new { ID = ST.ID, Status = ST.Status, SearchSummaryID = Search.ID, Retry = Search.RetryCount, RequestURL = Search.RequestURL })
                                           .Where(ST => ST.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase) && ST.Retry < RetryCount)
                                           .Select(obj => new { ID = obj.SearchSummaryID, RequestURL = obj.RequestURL }).ToDictionary(a => a.ID, a => a.RequestURL);
            }
            catch (Exception)
            {

                throw;
            }
            return failedRequests;
        }
        public void SendAsync()
        {
            Dictionary<long, string> FailedRequests = GetFailedRequestCount();
            try
            {
                if (FailedRequests.Count > 0)
                {
                    foreach (KeyValuePair<long, string> dicItem in FailedRequests)
                    {
                        SendRequestToAPISync(dicItem.Key, dicItem.Value, null);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> SendRequestToAPI(long SearchSummaryID, string requestURL, SearchSummary searchSummarized)
        {
            string statusCode = string.Empty;
            SearchSummary searchSummary = new SearchSummary();
            if (searchSummarized == null)
            {
                searchSummary = GetById(SearchSummaryID, false);
            }
            else
            {
                searchSummary = searchSummarized;
            }
            try
            {
                string responseString = string.Empty;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestURL);
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:502/api/Scrapper/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = (1000 * 60) * 10;
                var httpResponse = await httpWebRequest.GetResponseAsync();
                WebHeaderCollection headers = httpResponse.Headers;
                statusCode = ((System.Net.HttpWebResponse)(httpResponse)).StatusCode.ToString();
                searchSummary.ID = SearchSummaryID;
                UpdateRequestStatus(searchSummary, statusCode, requestURL);
                httpResponse.Close();
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        statusCode = reader.ReadToEnd();
                    }
                }
                UpdateRequestStatus(searchSummary, statusCode, requestURL);
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
                throw ex;
            }
            //finally
            //{
            //    UpdateRequestStatus(searchSummary, statusCode, requestURL);
            //}

            //Update();
            return statusCode;
        }

        public bool SendRequestToAPISync(long SearchSummaryID, string requestURL, SearchSummary searchSummarized)
        {
            SearchSummary searchSummary = new SearchSummary();
            string statusCode = string.Empty;
            try
            {


                if (searchSummarized == null)
                {
                    searchSummary = GetById(SearchSummaryID, false);
                }
                else
                {
                    searchSummary = searchSummarized;
                }
                string responseString = string.Empty;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestURL);
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:502/api/Scrapper/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = (1000 * 30);
                var httpResponse = httpWebRequest.GetResponse();
                WebHeaderCollection headers = httpResponse.Headers;
                statusCode = ((System.Net.HttpWebResponse)(httpResponse)).StatusCode.ToString();
                searchSummary.ID = SearchSummaryID;
                UpdateRequestStatus(searchSummary, statusCode, requestURL);
                httpResponse.Close();
            }
            catch (WebException e)
            {
                try
                {
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        using (Stream data = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(data))
                            {
                                statusCode = reader.ReadToEnd();
                                data.Flush();
                                data.Close();
                            }
                        }

                    }
                }
                catch (Exception)
                {

                }
                UpdateRequestStatus(searchSummary, statusCode, requestURL);
            }
            if (!string.IsNullOrEmpty(statusCode) && statusCode.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        public void UpdateRequestStatus(SearchSummary searchSummary, string status, string requestURL)
        {
            if (searchSummary != null)
            {
                searchSummary.RetryCount = searchSummary.RetryCount + 1;
                searchSummary.UpdatedDateTime = DateTime.Now;
                searchSummary.Response = status;
                if (!string.IsNullOrEmpty(requestURL))
                {
                    searchSummary.RequestURL = requestURL;
                }
                if (status.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName("Request Sent To Scrapper");
                }
                else
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName("Failed");
                }
                Update(searchSummary);
            }
        }

        public async Task<SearchSummaryResult> GetSearchSummaryData(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId)
        {
            SearchSummaryResult searchSummaryResult = new SearchSummaryResult();
            searchSummaryResult.lstSearchSummaryData = new List<SearchSummeryData>();

            DateTime lastdate = DateTime.Now.Date.AddDays(getLastDaysRecord);
            //var searchSummaryData = _context.SearchSummaries.Where(obj => obj.StatusID != statusID && obj.ScheduledJobID == null && obj.CreatedDateTime >= lastdate);
            List<SearchSummary> searchSummaryData = null;

            DateTime fetchAfterDateTime;
            //if  objLastModifieddate is empty implies first time call to search summary
            if (string.IsNullOrEmpty(objLastModifieddate))
            {
                searchSummaryData = await _context.SearchSummaries.Where(obj => obj.CreatedDateTime >= lastdate && obj.ScheduledJobID == null &&
                    (isAdminUser ? true : obj.CreatedBy == selectedUserId) && obj.StatusID != statusID
                    && string.IsNullOrEmpty(obj.ShopType)).ToListAsync();
            }
            else
            {
                fetchAfterDateTime = Convert.ToDateTime(objLastModifieddate);
                searchSummaryData = await _context.SearchSummaries.Where(obj => obj.CreatedDateTime >= lastdate && obj.ScheduledJobID == null && (isAdminUser ? true : obj.CreatedBy == selectedUserId) && obj.UpdatedDateTime >= fetchAfterDateTime && string.IsNullOrEmpty(obj.ShopType)).ToListAsync();
            }
            if (searchSummaryData != null && searchSummaryData.Count() > 0)
            {
                List<SearchSummary> searchSummaries = searchSummaryData.OrderByDescending(obj => obj.UpdatedDateTime).ToList();
                //var lastModifiedDateFromDB = searchSummaries.FirstOrDefault().UpdatedDateTime.AddDays(getLastDaysRecord);
                var lastModifiedDateFromDB = searchSummaries.FirstOrDefault().UpdatedDateTime;

                if (string.IsNullOrEmpty(objLastModifieddate) || (lastModifiedDateFromDB.AddMilliseconds(-lastModifiedDateFromDB.Millisecond) > Convert.ToDateTime(objLastModifieddate)))
                {
                    searchSummaryResult.lastModifiedDate = Convert.ToString(searchSummaries.FirstOrDefault().UpdatedDateTime);
                    //letlast date from search summary
                    //var lastdate = _context.SearchSummaries.OrderByDescending(obj => obj.CreatedDateTime).FirstOrDefault().CreatedDateTime.AddDays(getLastDaysRecord);

                    var UserLocationBrandID = await _context.UserLocationBrands.Where(obj => obj.UserID == LoggedInUserID).Select(obj => obj.LocationBrandID).ToListAsync();

                    ///get the last 3 days search summary record
                    var last3DaysRecord = (from ss in searchSummaryData
                                           orderby ss.CreatedDateTime descending
                                           select ss).ToList();

                    //Multiple location will be implemented.
                    var finalData = (from lastrecord in last3DaysRecord
                                     join user in _context.Users on lastrecord.CreatedBy equals user.ID
                                     join userlocation in UserLocationBrandID on Convert.ToInt32(lastrecord.LocationBrandIDs) equals userlocation
                                     join locationBrand in _context.LocationBrands on Convert.ToInt32(lastrecord.LocationBrandIDs) equals locationBrand.ID
                                     where !(locationBrand.IsDeleted)
                                     select new
                                     {
                                         ID = lastrecord.ID,
                                         StartDate = lastrecord.StartDate,
                                         EndDate = lastrecord.EndDate,
                                         CreatedDateTime = lastrecord.CreatedDateTime,
                                         CreatedBy = lastrecord.CreatedBy,
                                         CarClassesIDs = lastrecord.CarClassesIDs,
                                         LocationBrandIDs = lastrecord.LocationBrandIDs,
                                         RentalLengthIDs = lastrecord.RentalLengthIDs,
                                         ScrapperSourceIDs = lastrecord.ScrapperSourceIDs,
                                         StatusID = lastrecord.StatusID,
                                         Response = lastrecord.Response,
                                         BrandID = locationBrand.BrandID,
                                         UserName = user.UserName,
                                         FullName = user.FirstName + " " + user.LastName,
                                         UserID = user.ID,
                                         IsQuickView = lastrecord.IsQuickView,
                                         HasQuickViewChild = lastrecord.HasQuickViewChild,
                                         IsGOV = lastrecord.IsGov.HasValue ? lastrecord.IsGov.Value : false
                                     }).ToList();

                    //var QuickViewSearchList = (from searchSummary in finalData
                    //                           join quickview in _context.QuickView on searchSummary.ID equals quickview.ParentSearchSummaryId
                    //                           where quickview.CreatedDateTime == currentDateTime && !(quickview.IsDeleted)
                    //                           select searchSummary).ToList();

                    IEnumerable<ScrapperSource> lstScrapperSource = await _context.ScrapperSources.ToListAsync();
                    IEnumerable<LocationBrand> lstLocationBrands = await _context.LocationBrands.ToListAsync();
                    IEnumerable<RentalLength> lstRentalLengths = await _context.RentalLengths.ToListAsync();
                    IEnumerable<CarClass> lstCarClasses = await _context.CarClasses.ToListAsync();

                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //Loop for final record and set format
                    foreach (var lastDaysRecord in finalData)
                    {
                        //bool? IsQuickViewCheck = (lastDaysRecord.IsQuickView != null) ? lastDaysRecord.IsQuickView : false;
                        ////This condition is check for if quickview is fall with current date and that quickview should display in search summary
                        //if (IsQuickViewCheck != null && IsQuickViewCheck == true)
                        //{
                        //    if (QuickViewSearchList.Where(obj => obj.ID == lastDaysRecord.ID).ToList().Count() == 0)
                        //    {
                        //        continue;
                        //    }
                        //}
                        SearchSummeryData _SearchSummary = new SearchSummeryData();
                        _SearchSummary.SearchSummaryID = lastDaysRecord.ID;
                        _SearchSummary.StartDate = lastDaysRecord.StartDate; //lastDaysRecord.StartDate.ToString("MMM dd HH:MM");						
                        _SearchSummary.EndDate = lastDaysRecord.EndDate; //lastDaysRecord.EndDate.ToString("MMM dd HH:MM");
                        _SearchSummary.StartTime = _SearchSummary.StartDate.ToString("HH:mm");
                        _SearchSummary.EndTime = _SearchSummary.EndDate.ToString("HH:mm");
                        _SearchSummary.CreatedDate = lastDaysRecord.CreatedDateTime; //lastDaysRecord.CreatedDateTime.ToString("MMM dd HH:MM");

                        //User user = new User();
                        //user = _userService.GetAll().Where(obj => obj.ID == lastDaysRecord.CreatedBy).FirstOrDefault(); //_userService.GetById(lastDaysRecord.CreatedBy); // _context.Users.Where(obj => obj.ID == item.CreatedBy).SingleOrDefault();//Get the user details 
                        _SearchSummary.UserName = lastDaysRecord.UserName;
                        _SearchSummary.FullName = lastDaysRecord.FullName;
                        _SearchSummary.UserID = lastDaysRecord.UserID;

                        #region split string value
                        IEnumerable<long> carid = Common.StringToLongList(lastDaysRecord.CarClassesIDs).OrderBy(obj => obj);
                        IEnumerable<long> locationbrandid = Common.StringToLongList(lastDaysRecord.LocationBrandIDs).OrderBy(obj => obj);
                        IEnumerable<long> lengthid = Common.StringToLongList(lastDaysRecord.RentalLengthIDs).OrderBy(obj => obj);
                        IEnumerable<long> sourceid = Common.StringToLongList(lastDaysRecord.ScrapperSourceIDs).OrderBy(obj => obj);

                        #endregion


                        //_SearchSummary.CarClassIDs = string.Join(",", carid.ToArray()).TrimEnd(',');
                        _SearchSummary.CarClassIDs = string.Join(",", (from carclasses in lstCarClasses
                                                                       join objcarid in carid on carclasses.ID equals objcarid
                                                                       orderby carclasses.DisplayOrder
                                                                       select objcarid).ToArray());
                        _SearchSummary.CarClassName = string.Join(", ", (from carclasses in lstCarClasses
                                                                         join objcarid in carid on carclasses.ID equals objcarid
                                                                         orderby carclasses.DisplayOrder
                                                                         select carclasses.Code).ToArray());

                        //-------------Length opertion-------
                        _SearchSummary.RentalLengthsIDs = string.Join(",", lengthid.ToArray()).TrimEnd(',');
                        _SearchSummary.RentalLengthName = string.Join(", ", (from rentallengths in lstRentalLengths
                                                                             join objlengthid in lengthid on rentallengths.MappedID equals objlengthid
                                                                             select rentallengths.Code).ToArray());

                        //-------------Source opertaion-------
                        _SearchSummary.SourcesIDs = string.Join(",", sourceid.ToArray()).TrimEnd(',');
                        //-------------Source opertaion-------
                        string tempSourceName = string.Join(", ", (from scrappersource in lstScrapperSource
                                                                   join objsourceid in sourceid on scrappersource.ID equals objsourceid
                                                                   select scrappersource.Name).ToArray());
                        _SearchSummary.SourceName = tempSourceName.TrimEnd(',');


                        //-----------Location operation-------                                       
                        _SearchSummary.LocationsBrandIDs = string.Join(",", locationbrandid.ToArray()).TrimEnd(',');
                        _SearchSummary.LocationName = string.Join(", ", (from locationbrands in lstLocationBrands
                                                                         join objlocationbrandid in locationbrandid on locationbrands.ID equals objlocationbrandid
                                                                         select locationbrands.LocationBrandAlias).ToArray());

                        _SearchSummary.LocationIDs = string.Join(",", (from locationbrands in lstLocationBrands
                                                                       join objlocationbrandid in locationbrandid on locationbrands.ID equals objlocationbrandid
                                                                       select locationbrands.LocationID).ToArray());

                        var utcDate = TimeZoneInfo.ConvertTimeToUtc(_SearchSummary.CreatedDate, easternZone);
                        DateTime clientDateTime = utcDate.AddMinutes(-1 * clientTimezoneOffset);

                        //monthNames[varCreateDate.getMonth()] + " " + varCreateDate.getDate() + " - " + (varCreateDate.getHours() < 10 ? '0' : '') + varCreateDate.getHours() + ":" + (varCreateDate.getMinutes() < 10 ? '0' : '') + varCreateDate.getMinutes() + " - " + tempLocationName.trim().substring(0, tempLocationName.trim().length - 1) + " - " + data.UserName;

                        _SearchSummary.SummaryText = String.Format("{0:MMMM dd - HH:mm}", clientDateTime) + " - " + _SearchSummary.LocationName + " - " + _SearchSummary.UserName;

                        if (lastDaysRecord.StatusID == 1 || lastDaysRecord.StatusID == 2 || lastDaysRecord.StatusID == 3)
                        {
                            _SearchSummary.StatusIDs = 1;
                            _SearchSummary.StatusName = "IN PROGRESS";
                            _SearchSummary.FailureResponse = Convert.ToString(lastDaysRecord.Response);
                            _SearchSummary.StatusClass = "statusip";
                        }
                        else
                        {
                            _SearchSummary.StatusIDs = lastDaysRecord.StatusID;
                            _SearchSummary.StatusName = _statusService.GetById(lastDaysRecord.StatusID).Status; // _context.StatuseDesc.Where(obj => obj.ID == item.StatusID).Select(obj=>obj.Status).SingleOrDefault();
                            _SearchSummary.FailureResponse = Convert.ToString(lastDaysRecord.Response);
                            if (lastDaysRecord.StatusID == 4)
                            {
                                _SearchSummary.StatusName = "COMPLETE";
                                _SearchSummary.StatusClass = "statusc";
                            }
                            else if (lastDaysRecord.StatusID == 5)
                            {
                                _SearchSummary.StatusName = "FAILED";
                                _SearchSummary.StatusClass = "statusf";
                            }
                        }



                        _SearchSummary.BrandIDs = Convert.ToString(lastDaysRecord.BrandID);
                        //commented to show previous day quick view shops
                        //_SearchSummary.IsQuickView = (lastDaysRecord.CreatedDateTime.Date == userSystemDate.Date) ? ((lastDaysRecord.IsQuickView != null) ? lastDaysRecord.IsQuickView : false) : false;
                        _SearchSummary.IsQuickView = lastDaysRecord.IsQuickView.HasValue ? lastDaysRecord.IsQuickView.Value : false;
                        _SearchSummary.HasQuickViewChild = lastDaysRecord.HasQuickViewChild.HasValue ? lastDaysRecord.HasQuickViewChild.Value : false;
                        _SearchSummary.IsGOV = lastDaysRecord.IsGOV;

                        if (_SearchSummary.IsQuickView.Value)
                        {
                            _SearchSummary.SearchTypeClass = "quick-view-past-search hidden";
                        }
                        else if (_SearchSummary.HasQuickViewChild.Value && !_SearchSummary.IsQuickView.Value)
                        {
                            _SearchSummary.SearchTypeClass = "has-quick-view-search hidden";
                        }
                        else
                        {
                            _SearchSummary.SearchTypeClass = "hidden";
                        }
                        searchSummaryResult.lstSearchSummaryData.Add(_SearchSummary);
                    }
                }
                else
                {
                    searchSummaryResult.lastModifiedDate = objLastModifieddate;
                }
            }
            //System.Diagnostics.Debug.WriteLine("End: " + DateTime.Now.ToString());

            return searchSummaryResult;

        }


        public async Task<SearchSummaryResult> GetSearchSummaryDataFromSP(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId)
        {

            SearchSummaryResult searchSummaryResult = new SearchSummaryResult();
            searchSummaryResult.lstSearchSummaryData = new List<SearchSummeryData>();

            DateTime lastdate = DateTime.Now.Date.AddDays(getLastDaysRecord);

            DateTime? fetchAfterDateTime = (DateTime?)null;

            const string queryFetchSummaryList = "EXEC [getSearchSummaryData] @objLastModifieddate=@objLastModifieddate,@selectedUserId=@selectedUserId,@statusID=@statusID,@getLastDaysRecord=@getLastDaysRecord,@fetchSummaryShops=@fetchSummaryShops,@isAdmin=@isAdmin";

            List<SqlParameter> queryParams = new List<SqlParameter>();
            if (string.IsNullOrEmpty(objLastModifieddate))
            {
                queryParams.Add(new SqlParameter("@objLastModifieddate", DBNull.Value));
            }
            else
            {
                fetchAfterDateTime = Convert.ToDateTime(objLastModifieddate);
                queryParams.Add(new SqlParameter("@objLastModifieddate", fetchAfterDateTime.Value));
            }

            queryParams.Add(new SqlParameter("@selectedUserId", selectedUserId));
            queryParams.Add(new SqlParameter("@statusID", statusID));
            queryParams.Add(new SqlParameter("@getLastDaysRecord", getLastDaysRecord));
            queryParams.Add(new SqlParameter("@fetchSummaryShops", false));
            queryParams.Add(new SqlParameter("@isAdmin", isAdminUser));
            var searchList = await _context.ExecuteSQLQuery<SearchSummeryData>(queryFetchSummaryList, queryParams.ToArray());

            if (searchList != null && searchList.Count() > 0)
            {
                //List<SearchSummary> searchSummaries = searchSummaryData.OrderByDescending(obj => obj.UpdatedDateTime).ToList();

                var lastModifiedDateFromDB = searchList.FirstOrDefault().UpdatedDateTime;

                if (string.IsNullOrEmpty(objLastModifieddate) || (lastModifiedDateFromDB.AddMilliseconds(-lastModifiedDateFromDB.Millisecond) > Convert.ToDateTime(objLastModifieddate)))
                {
                    searchSummaryResult.lastModifiedDate = Convert.ToString(searchList.FirstOrDefault().UpdatedDateTime);

                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime clientDateTime;
                    DateTime utcDate;
                    foreach (var search in searchList)
                    {
                        utcDate = TimeZoneInfo.ConvertTimeToUtc(search.CreatedDate, easternZone);
                        clientDateTime = utcDate.AddMinutes(-1 * clientTimezoneOffset);

                        search.SummaryText = String.Format("{0:MMMM dd - HH:mm}", clientDateTime) + " - " + search.LocationName + " - " + search.UserName;

                        if (search.StatusIDs == 1 || search.StatusIDs == 2 || search.StatusIDs == 3)
                        {
                            search.StatusIDs = 1;
                            search.StatusName = "IN PROGRESS";
                            search.FailureResponse = Convert.ToString(search.FailureResponse);
                            search.StatusClass = "statusip";
                        }
                        else
                        {
                            search.StatusIDs = search.StatusIDs;
                            search.StatusName = _statusService.GetById(search.StatusIDs).Status; // _context.StatuseDesc.Where(obj => obj.ID == item.StatusID).Select(obj=>obj.Status).SingleOrDefault();
                            search.FailureResponse = Convert.ToString(search.FailureResponse);
                            if (search.StatusIDs == 4)
                            {
                                search.StatusName = "COMPLETE";
                                search.StatusClass = "statusc";
                            }
                            else if (search.StatusIDs == 5)
                            {
                                search.StatusName = "FAILED";
                                search.StatusClass = "statusf";
                            }
                        }

                        search.IsAutomationSummary = false;
                        search.IsFTBSummary = false;
                        searchSummaryResult.lstSearchSummaryData.Add(search);
                    }
                }
                else
                {
                    searchSummaryResult.lastModifiedDate = objLastModifieddate;
                }
            }
            //System.Diagnostics.Debug.WriteLine("End: " + DateTime.Now.ToString());

            return searchSummaryResult;

        }

        public async Task<string> SaveSearchSummary(SearchModelDTO searchModel)
        {
            if (searchModel != null)
            {

                //create request URL
                string StartDate = string.Empty;
                string EndDate = string.Empty;
                if (searchModel.StartDate != null && !string.IsNullOrEmpty(searchModel.StartDate.ToString()))
                {
                    StartDate = searchModel.StartDate.ToString("yyyy-MM-dd");

                    StartDate += " " + searchModel.PickUpTime;
                }
                if (searchModel.EndDate != null && !string.IsNullOrEmpty(searchModel.EndDate.ToString()))
                {
                    EndDate = searchModel.EndDate.ToString("yyyy-MM-dd");
                    EndDate += " " + searchModel.DropOffTime;
                }
                string pickUp = DateTime.ParseExact(StartDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");
                string dropOff = DateTime.ParseExact(EndDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");

                List<string> companyCodes = _compnayService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList();

                if (searchModel.ScrapperSource == "CPR")
                {
                    companyCodes.RemoveAll(d => d == "ZI" || d == "ZE");
                }

                DateTime newStartDate = Convert.ToDateTime(StartDate);
                DateTime newEndDate = Convert.ToDateTime(EndDate);

                //Sample URL
                //requestUrl = "http://172.27.61.10/cgi-bin/dot_net_service_call_to_scraper.cgi?datasource=ARC&loc=MCO&carclasses=ECAR,CCAR,LFAR,STAR,LCAR,FCAR,PCAR,ICAR,SCAR&lor=2&strtdt=2015-07-01T17:00:00Z&enddt=2015-07-03T17:00:00Z&searchid=7111&vndr=MW,ZA,ZI,EY,FX,ZD,ZE,EZ,AC,AD,SV,ET,SX,ZL,AL,FF,ZR,ZT";

                //Replace {{SEARCHSUMMARYID}} later
                //Select the scraping server based on TSD access rights
                User user = _userService.GetAll().Where(d => d.ID == searchModel.CreatedBy).FirstOrDefault();
                string scrappingServerUrl = string.Empty;
                if (searchModel.ShopType == ShopTypes.SummaryShop)
                {
                    scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.SummaryShop);
                }
                else if (user.IsTSDUpdateAccess.HasValue && user.IsTSDUpdateAccess.Value)
                {
                    scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.NormalShop);
                }
                else
                {
                    scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.ReadOnlyShop);
                }

                string requestUrl = scrappingServerUrl + "datasource=" + searchModel.ScrapperSource + "&loc=" + searchModel.location + "&carclasses=" + searchModel.CarClasses
                    + "&lor=" + searchModel.RentalLengthIDs + "&strtdt=" + pickUp + "&enddt=" + dropOff + "&searchid={{SEARCHSUMMARYID}}" +
                    "&vndr=" + String.Join(",", companyCodes) + "&user=" + searchModel.UserName;

                SearchSummary searchSummary = new SearchSummary()
                {
                    ScrapperSourceIDs = searchModel.ScrapperSourceIDs,
                    LocationBrandIDs = searchModel.LocationBrandIDs,
                    CarClassesIDs = searchModel.CarClassesIDs,
                    RentalLengthIDs = searchModel.RentalLengthIDs,
                    StartDate = newStartDate,
                    EndDate = newEndDate,
                    //temporary changes done to avoid issue in Initiate Search
                    StatusID = 1,
                    RetryCount = 1,
                    CreatedBy = searchModel.CreatedBy,
                    UpdatedBy = searchModel.CreatedBy,
                    CreatedDateTime = DateTime.Now,
                    UpdatedDateTime = DateTime.Now,
                    RequestURL = requestUrl,
                    ProviderId = searchModel.ProviderId,
                    IsGov = Convert.ToBoolean(searchModel.IsGovShop),
                    ShopType = searchModel.ShopType,
                    ScheduledJobID = searchModel.ScheduleJobId > 0 ? searchModel.ScheduleJobId : (long?)null,
                    FTBScheduledJobID = searchModel.FTBScheduledJobID > 0 ? searchModel.FTBScheduledJobID : (long?)null
                };
                Add(searchSummary);

                //{{SEARCHSUMMARYID}} is replaced just for htting the request. For datebase record, we have to save it again.
                requestUrl = requestUrl.Replace("{{SEARCHSUMMARYID}}", searchSummary.ID.ToString());

                string response = await SendRequestToAPI(searchSummary.ID, requestUrl, searchSummary);
                return response;
            }
            //TODO: Remove this.
            return string.Empty;
        }

        public SearchModelDTO SaveSearchSummary(SearchModelDTO searchModel, string requestUrl)
        {
            if (searchModel != null)
            {

                //create request URL
                string StartDate = string.Empty;
                string EndDate = string.Empty;
                if (searchModel.StartDate != null && !string.IsNullOrEmpty(searchModel.StartDate.ToString()))
                {
                    StartDate = searchModel.StartDate.ToString("yyyy-MM-dd");

                    StartDate += " " + searchModel.PickUpTime;
                }
                if (searchModel.EndDate != null && !string.IsNullOrEmpty(searchModel.EndDate.ToString()))
                {
                    EndDate = searchModel.EndDate.ToString("yyyy-MM-dd");
                    EndDate += " " + searchModel.DropOffTime;
                }
                //string pickUp = DateTime.ParseExact(StartDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");
                //string dropOff = DateTime.ParseExact(EndDate, "yyyy-MM-dd h:mmtt", CultureInfo.CurrentCulture).ToString("yyyy-MM-ddTHH:mm:ssZ");

                //searchModel.VendorCodes = String.Join(",", _compnayService.GetAll(true).Where(a => !a.IsDeleted).Select(obj => obj.Code).ToList());

                DateTime newStartDate = Convert.ToDateTime(StartDate);
                DateTime newEndDate = Convert.ToDateTime(EndDate);


                SearchSummary searchSummary = new SearchSummary();

                searchSummary.ScrapperSourceIDs = searchModel.ScrapperSourceIDs;
                searchSummary.LocationBrandIDs = searchModel.LocationBrandIDs;
                searchSummary.CarClassesIDs = searchModel.CarClassesIDs;
                searchSummary.RentalLengthIDs = searchModel.RentalLengthIDs;
                searchSummary.StartDate = newStartDate;
                searchSummary.EndDate = newEndDate;
                searchSummary.StatusID = _statusService.GetStatusIDByName("Pending");
                searchSummary.RetryCount = 1;
                searchSummary.CreatedBy = searchModel.CreatedBy;
                searchSummary.UpdatedBy = searchModel.CreatedBy;
                searchSummary.CreatedDateTime = DateTime.Now;
                searchSummary.UpdatedDateTime = DateTime.Now;
                searchSummary.RequestURL = requestUrl;
                searchSummary.ProviderId = searchModel.ProviderId;
                searchSummary.IsGov = Convert.ToBoolean(searchModel.IsGovShop);//shop initiated with GOV source 
                if (!string.IsNullOrEmpty(searchModel.PostData))
                {
                    searchSummary.PostData = searchModel.PostData;
                }
                if (searchModel.ScheduleJobId > 0)
                {
                    searchSummary.ScheduledJobID = searchModel.ScheduleJobId;
                }


                Add(searchSummary);
                searchModel.SearchSummaryID = searchSummary.ID;
                return searchModel;
            }
            return null;
        }

        public async Task<string> InitiateQuickViewSearch(long searchSummaryId, long userId, bool isAutomatedQuickView = false)
        {

            SearchSummary objSearchSummaryEntity = GetById(searchSummaryId, false);            

            if (objSearchSummaryEntity != null)
            {
                SearchSummary objNewSearchSummaryEntity = new SearchSummary();
                QuickView quickViewShop = _context.QuickView.Where(quick => !quick.IsDeleted && (quick.ParentSearchSummaryId == searchSummaryId || quick.ChildSearchSummaryId == searchSummaryId)).FirstOrDefault();

                objNewSearchSummaryEntity.CarClassesIDs = objSearchSummaryEntity.CarClassesIDs;
                objNewSearchSummaryEntity.EndDate = objSearchSummaryEntity.EndDate;
                objNewSearchSummaryEntity.IsQuickView = true;
                objNewSearchSummaryEntity.LocationBrandIDs = objSearchSummaryEntity.LocationBrandIDs;
                objNewSearchSummaryEntity.ProviderId = objSearchSummaryEntity.ProviderId;
                objNewSearchSummaryEntity.RentalLengthIDs = objSearchSummaryEntity.RentalLengthIDs;
                objNewSearchSummaryEntity.RequestURL = objSearchSummaryEntity.RequestURL;
                objNewSearchSummaryEntity.ScrapperSourceIDs = objSearchSummaryEntity.ScrapperSourceIDs;
                objNewSearchSummaryEntity.StartDate = objSearchSummaryEntity.StartDate;
                objNewSearchSummaryEntity.StatusID = _statusService.GetStatusIDByName("Pending");
                objNewSearchSummaryEntity.ProviderId = objSearchSummaryEntity.ProviderId;
                objNewSearchSummaryEntity.PostData = objSearchSummaryEntity.PostData;
                objNewSearchSummaryEntity.CreatedBy = userId;
                objNewSearchSummaryEntity.UpdatedBy = userId;

                objNewSearchSummaryEntity.CreatedDateTime = DateTime.Now;
                objNewSearchSummaryEntity.UpdatedDateTime = DateTime.Now;

                objNewSearchSummaryEntity.IsGov = objSearchSummaryEntity.IsGov.HasValue ? objSearchSummaryEntity.IsGov.Value : false;

                if (!string.IsNullOrEmpty(quickViewShop.PickupTime) && !string.IsNullOrEmpty(quickViewShop.DropoffTime))
                {                    
                    objNewSearchSummaryEntity.StartDate = Convert.ToDateTime(objNewSearchSummaryEntity.StartDate.ToString("yyyy-MM-dd") + " " + quickViewShop.PickupTime);
                    objNewSearchSummaryEntity.EndDate = Convert.ToDateTime(objNewSearchSummaryEntity.EndDate.ToString("yyyy-MM-dd") + " " + quickViewShop.DropoffTime);                    
                }

                Add(objNewSearchSummaryEntity);
                string response = string.Empty;
                if (objSearchSummaryEntity.ProviderId.HasValue)
                {
                    Dictionary<long, string> dicProviders = _providerService.GetAll().ToDictionary(d => d.ID, d => d.Code);
                    string selectedAPI;
                    dicProviders.TryGetValue(objNewSearchSummaryEntity.ProviderId.Value, out selectedAPI);
                    switch (selectedAPI)
                    {
                        case "RH":
                            ProviderRequest objProviderRequest = new ProviderRequest();
                            objProviderRequest.ProviderRequestData = AlterRateHighwayPostData(objNewSearchSummaryEntity.PostData, objNewSearchSummaryEntity);
                            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
                            objProviderRequest.HeaderItems.Add("AccessID", RateHighwaySetting.AccessID);
                            objProviderRequest.URL = RateHighwaySetting.AdhocRequestURL; //objRateHighwayDTO.AdhocRequest.FormatWith(objRateHighwayDTO);                            
                            ResponseModel responseObj = SendRateHighwayAutomationAsync(objProviderRequest);
                            ProcessResponse(responseObj, objNewSearchSummaryEntity.ID);
                            response = responseObj.StatusDescription;
                            //response = _searchSummaryService.InitializeRateHighwayAutomationSearch(scheduledJob, requestURL, startDate, endDate, "RH");

                            break;
                        default:
                        case "SS":                            
                            string newRequestUrl = AlterScraperQueryString(objSearchSummaryEntity.RequestURL, objNewSearchSummaryEntity);

                            //Select the scraping server based on TSD access rights
                            User user = _userService.GetAll().Where(d => d.ID == userId).FirstOrDefault();
                            string scrappingServerUrl = string.Empty;
                            if (user.IsTSDUpdateAccess.HasValue && user.IsTSDUpdateAccess.Value)
                            {
                                scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.QuickViewShop);
                            }
                            else
                            {
                                scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.ReadOnlyShop);
                            }
                            newRequestUrl = scrappingServerUrl + newRequestUrl.Substring(newRequestUrl.IndexOf('?') + 1);

                            response = await SendRequestToAPI(objNewSearchSummaryEntity.ID, newRequestUrl, objNewSearchSummaryEntity);
                            break;
                    }
                }
                else
                {
                    string newRequestUrl = AlterScraperQueryString(objSearchSummaryEntity.RequestURL, objNewSearchSummaryEntity);

                    //Select the scraping server based on TSD access rights
                    User user = _userService.GetAll().Where(d => d.ID == userId).FirstOrDefault();
                    string scrappingServerUrl = string.Empty;
                    if (user.IsTSDUpdateAccess.HasValue && user.IsTSDUpdateAccess.Value)
                    {
                        scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.QuickViewShop);
                    }
                    else
                    {
                        scrappingServerUrl = _iScrappingServersService.GetScrappingUrl(EnumScrappingServers.ReadOnlyShop);
                    }
                    newRequestUrl = scrappingServerUrl + newRequestUrl.Substring(newRequestUrl.IndexOf('?') + 1);

                    response = await SendRequestToAPI(objNewSearchSummaryEntity.ID, newRequestUrl, objNewSearchSummaryEntity);
                }

                
                if (quickViewShop != null)
                {
                    long previousParentSearchId = 0;
                    if (isAutomatedQuickView)
                    {
                        quickViewShop.NextRunDate = null;
                    }
                    //check if child search is failed/Deleted if not then only swap parent and child searchSummary ID
                    if (quickViewShop.ChildSearchSummaryId.HasValue)
                    {
                        List<long> statusList = _statusService.GetAll().Where(status => status.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                            || status.Status.Equals("Deleted", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).ToList();

                        SearchSummary childSearch = GetById(quickViewShop.ChildSearchSummaryId.Value, false);
                        if (childSearch != null && !statusList.Contains(childSearch.StatusID))
                        {
                            previousParentSearchId = quickViewShop.ParentSearchSummaryId;
                            quickViewShop.ParentSearchSummaryId = quickViewShop.ChildSearchSummaryId.Value;
                            childSearch.IsQuickView = false;
                            _context.Entry(childSearch).State = EntityState.Modified;
                        }
                        //else continue with replacing child search summary id with new one
                    }
                    quickViewShop.ChildSearchSummaryId = objNewSearchSummaryEntity.ID;
                    if (response.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        quickViewShop.IsExecutionInProgress = true;
                        //set status in - progress
                        quickViewShop.StatusId = _statusService.GetAll().Where(status => status.Status.Equals("Request Sent To Scrapper", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).FirstOrDefault();
                    }
                    else
                    {
                        quickViewShop.StatusId = _statusService.GetAll().Where(status => status.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).FirstOrDefault();
                        quickViewShop.IsExecutionInProgress = false;
                    }
                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime currentESTTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                    quickViewShop.LastRunDate = currentESTTime;
                    quickViewShop.UpdatedDateTime = currentESTTime.Date;
                    _context.Entry(quickViewShop).State = EntityState.Modified;
                    _context.SaveChanges();

                    //delete previous parent search
                    if (previousParentSearchId > 0)
                    {
                        SearchSummary searchSummary = _context.SearchSummaries.Where(d => d.ID == previousParentSearchId).FirstOrDefault();
                        if (searchSummary != null)
                        {
                            searchSummary.StatusID = 6;
                            searchSummary.UpdatedDateTime = DateTime.Now;
                            Update(searchSummary);
                        }
                    }
                }
                return response;
            }
            return string.Empty;
        }

        public bool InitializeRateHighwayAutomationSearch(ScheduledJob scheduledJob, string requestUrl, DateTime startDate, DateTime endDate, string apiCode)
        {
            SearchModelDTO searchModel = new SearchModelDTO();
            searchModel.ScrapperSourceIDs = scheduledJob.ScrapperSourceIDs;
            searchModel.LocationBrandIDs = scheduledJob.LocationBrandIDs;
            searchModel.CarClassesIDs = scheduledJob.CarClassesIDs;
            searchModel.RentalLengthIDs = scheduledJob.RentalLengthIDs;
            searchModel.StartDate = startDate;
            searchModel.EndDate = endDate;
            searchModel.CreatedBy = scheduledJob.CreatedBy;
            if (!scheduledJob.IsStandardShop && !string.IsNullOrEmpty(scheduledJob.PostData))
            {
                searchModel.PostData = scheduledJob.PostData.Replace("{{StartDateOffset}}", startDate.ToString("yyyy-MM-ddTHH:mm")).Replace("{{EndDateOffset}}", endDate.ToString("yyyy-MM-ddTHH:mm"));
            }
            else
            {
                searchModel.PostData = scheduledJob.PostData;
            }
            searchModel.ScheduleJobId = scheduledJob.ID;
            searchModel.SelectedAPI = apiCode;
            searchModel.ProviderId = scheduledJob.ProviderId;
            //set GOV shop 
            searchModel.IsGovShop = scheduledJob.IsGov.HasValue ? scheduledJob.IsGov.Value : false;

            string statusCode = SendRateHighwayRequest(searchModel);

            if (!string.IsNullOrEmpty(statusCode) && statusCode.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Scheduled/ Automation Jobs
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public bool InitializeAutomationSearch(ScheduledJob scheduledJob, string requestUrl, DateTime startDate, DateTime endDate)
        {
            SearchSummary searchSummary = new SearchSummary()
            {
                ScrapperSourceIDs = scheduledJob.ScrapperSourceIDs,
                LocationBrandIDs = scheduledJob.LocationBrandIDs,
                CarClassesIDs = scheduledJob.CarClassesIDs,
                RentalLengthIDs = scheduledJob.RentalLengthIDs,
                StartDate = startDate,
                EndDate = endDate,
                StatusID = _statusService.GetStatusIDByName("Pending"),
                RetryCount = 1,
                CreatedBy = scheduledJob.CreatedBy,
                UpdatedBy = scheduledJob.CreatedBy,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now,
                RequestURL = requestUrl,
                ScheduledJobID = scheduledJob.ID,
                ProviderId = scheduledJob.ProviderId,
                //set GOV shop 
                IsGov = scheduledJob.IsGov.HasValue ? scheduledJob.IsGov.Value : false
            };

            Add(searchSummary);

            //{{SEARCHSUMMARYID}} is replaced just for htting the request. For datebase record, we have to save it again.
            requestUrl = requestUrl.Replace("{{SEARCHSUMMARYID}}", searchSummary.ID.ToString());

            return SendRequestToAPISync(searchSummary.ID, requestUrl, searchSummary);

        }

        public Dictionary<string, string> GetSearchShopSummaryForCSV(long searchSummaryID)
        {
            Dictionary<string, string> shopDetails = new Dictionary<string, string>();

            var query = (from SS in _context.SearchSummaries
                         where SS.ID == searchSummaryID
                         select
                         new { RentalLengths = SS.RentalLengthIDs, CarClasses = SS.CarClassesIDs, SourceIds = SS.ScrapperSourceIDs, StartDate = SS.StartDate, EndDate = SS.EndDate, LocationBrandID = SS.LocationBrandIDs }
                 ).SingleOrDefault();

            if (query != null)
            {
                IEnumerable<long> rentalLenghtIDs = query.RentalLengths.Split(',').Select(p => long.Parse(p));
                IEnumerable<long> carClassIDs = query.CarClasses.Split(',').Select(p => long.Parse(p));
                IEnumerable<long> sourceIds = query.SourceIds.Split(',').Select(p => long.Parse(p));
                IEnumerable<long> locationBrandIds = query.LocationBrandID.Split(',').Select(p => long.Parse(p));


                shopDetails.Add("Startdate", query.StartDate.ToString("dd-MMM-yy"));
                shopDetails.Add("Enddate", query.EndDate.ToString("dd-MMM-yy"));
                shopDetails.Add("Lengths", string.Join(",", _context.RentalLengths.Where(d => rentalLenghtIDs.Contains(d.ID)).Select(d => d.Code)));
                shopDetails.Add("CarClasses", string.Join(",", _context.CarClasses.Where(d => carClassIDs.Contains(d.ID)).Select(d => d.Code)));
                shopDetails.Add("Sources", string.Join(",", _context.ScrapperSources.Where(d => sourceIds.Contains(d.ID)).Select(d => d.Code)));
                shopDetails.Add("LocationBrand", string.Join("_", _context.LocationBrands.Where(d => locationBrandIds.Contains(d.ID)).Select(d => d.LocationBrandAlias)));
            }

            return shopDetails;
        }




        public List<GlobalLimitDetailsDTO> GetGlobalLimitTetherShop(long SearchSummaryID, long DependantBrandID, long LocationID)
        {
            SearchSummary searchSummary = base.GetById(SearchSummaryID, false);
            DateTime startDate = Convert.ToDateTime(searchSummary.StartDate);
            DateTime endDate = Convert.ToDateTime(searchSummary.EndDate) + new TimeSpan(0, 0, 0);
            long LocationBrandID = long.Parse(searchSummary.LocationBrandIDs);
            startDate = startDate.Date + new TimeSpan(0, 0, 0);
            endDate = endDate.Date + new TimeSpan(0, 0, 0);
            var datediff = (endDate.Date - startDate.Date).TotalDays;


            var lid = _locationBrandService.GetAll().FirstOrDefault(a => a.BrandID == DependantBrandID && a.LocationID == LocationID && !(a.IsDeleted));
            //List<DateRangeDiff> StartTOEnd = new List<DateRangeDiff>();
            //for (int i = 0; i < datediff; i++)
            //{
            //    DateRangeDiff dateRangeDiff = new DateRangeDiff();
            //    dateRangeDiff.LocationBrandID = LocationBrandID;
            //    dateRangeDiff.Date = startDate.AddDays(i);

            //    StartTOEnd.Add(dateRangeDiff);
            //}
            List<GlobalLimitDetailsDTO> globalLimitDetails = new List<GlobalLimitDetailsDTO>();
            if (lid != null && lid.ID > 0)
            {
                globalLimitDetails = (from globallimit in _context.GlobalLimits
                                      join globallimitdetail in _context.GlobalLimitDetails on globallimit.ID equals globallimitdetail.GlobalLimitID
                                      orderby globallimitdetail.ID
                                      where globallimit.LocationBrandID == lid.ID && globallimit.EndDate >= startDate && globallimit.StartDate <= endDate
                                      select new GlobalLimitDetailsDTO
                                      {
                                          StartDate = globallimit.StartDate,
                                          EndDate = globallimit.EndDate,
                                          CarClassID = globallimitdetail.CarClassID,
                                          DayMax = globallimitdetail.DayMax,
                                          DayMin = globallimitdetail.DayMin,
                                          WeekMax = globallimitdetail.WeekMax,
                                          WeekMin = globallimitdetail.WeekMin,
                                          MonthlyMax = globallimitdetail.MonthMax,
                                          MonthlyMin = globallimitdetail.MonthMin,
                                          BrandLocation = globallimit.LocationBrandID,
                                          GlobalDetailsID = globallimitdetail.ID,
                                          GlobalLimitID = globallimit.ID
                                      }).ToList();
            }

            //List<GlobalLimit> GlobalLists = from globallimit in _context.GlobalLimits
            //                                where Convert.ToDateTime(globallimit.StartDate) >= startDate && Convert.ToDateTime(globallimit.EndDate) < endDate 
            //                                select globallimit;

            return globalLimitDetails;
        }

        public void UpdateSearchStatus(long searchSummaryId, long shopRequestId = 0, bool isRequestFromAPI = false, bool isSearchFailed = false, string response = "")
        {
            SearchSummary searchSummary = GetById(searchSummaryId, false);
            if (searchSummary != null && searchSummary.StatusID != _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["DeletedRequest"])))
            {
                searchSummary.UpdatedDateTime = DateTime.Now;
                if (isSearchFailed)
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["FailedRequest"]));
                    searchSummary.Response = response;
                    if (searchSummary.ScheduledJobID.HasValue)
                    {
                        searchSummary.IsReviewed = false;
                    }
                    //Update(searchSummary);

                    //if it is quick view update quick View status
                    if (searchSummary.IsQuickView.HasValue && searchSummary.IsQuickView.Value)
                    {
                        QuickView quickViewRow = _context.QuickView.Where(quick => quick.ChildSearchSummaryId == searchSummary.ID).FirstOrDefault();
                        if (quickViewRow != null)
                        {
                            quickViewRow.IsExecutionInProgress = false;
                            quickViewRow.StatusId = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["FailedRequest"]));
                            _context.Entry(quickViewRow).State = System.Data.Entity.EntityState.Modified;
                            _cacheManager.Remove(typeof(QuickView).ToString());
                        }
                    }
                    //if it is scheduled job update scheduled job status
                    if (searchSummary.ScheduledJobID.HasValue && searchSummary.ScheduledJobID.Value > 0)
                    {
                        ScheduledJob scheduledJob = _context.ScheduledJobs.Where(job => job.ID == searchSummary.ScheduledJobID.Value).SingleOrDefault();
                        if (scheduledJob != null)
                        {
                            scheduledJob.ExecutionInProgress = false;
                            _context.Entry(scheduledJob).State = System.Data.Entity.EntityState.Modified;
                            _cacheManager.Remove(typeof(ScheduledJob).ToString());
                        }
                    }
                    _context.Entry(searchSummary).State = System.Data.Entity.EntityState.Modified;
                    _cacheManager.Remove(typeof(SearchSummary).ToString());
                    _context.SaveChanges();
                }
                else if (isRequestFromAPI)
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["DataReceived"]));
                    Update(searchSummary);

                    //call data processing service to generate search result JSON.
                    //Task.Run(() => _searchResultService.GenerateSeachResultProcesssedData(searchSummary.ID));
                }
                else
                {
                    if (shopRequestId > 0)
                    {
                        searchSummary.ShopRequestId = shopRequestId;
                    }
                    searchSummary.Response = response;
                    searchSummary.StatusID = _statusService.GetStatusIDByName(Convert.ToString(ConfigurationManager.AppSettings["RequestSent"]));
                    Update(searchSummary);
                }
            }
        }

        public void UpdateQuickViewStatus(long searchSummaryId, long userId)
        {
            if (searchSummaryId > 0)
            {
                SearchSummary searchSummary = GetById(searchSummaryId, false);
                if (searchSummary != null)
                {
                    searchSummary.HasQuickViewChild = true;
                    searchSummary.UpdatedBy = userId;
                    searchSummary.UpdatedDateTime = DateTime.Now;
                    Update(searchSummary);
                }
            }
        }

        public long GetSummaryIdByShopId(long shopRequestId)
        {
            if (shopRequestId > 0)
            {
                SearchSummary objSearchSummary = _context.SearchSummaries.Where(d => d.ShopRequestId == shopRequestId).FirstOrDefault();
                if (objSearchSummary != null)
                {
                    return objSearchSummary.ID;
                }
            }
            return 0;
        }

        public string SendRateHighwayRequest(SearchModelDTO objSearchModelDTO)
        {
            RateHighwayDTO objRateHighwayDTO = JsonConvert.DeserializeObject<RateHighwayDTO>(objSearchModelDTO.PostData);
            objSearchModelDTO.StartDate = Convert.ToDateTime(objRateHighwayDTO.PickupDateTime).Date;
            objSearchModelDTO.PickUpTime = Convert.ToDateTime(objRateHighwayDTO.PickupDateTime).ToString("HH:mm");
            objSearchModelDTO.EndDate = Convert.ToDateTime(objRateHighwayDTO.ReturnDateTime).Date;
            objSearchModelDTO.DropOffTime = Convert.ToDateTime(objRateHighwayDTO.ReturnDateTime).ToString("HH:mm");

            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.ProviderRequestData = objSearchModelDTO.PostData;
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.HeaderItems.Add("AccessID", objRateHighwayDTO.AccessID);
            objProviderRequest.URL = objRateHighwayDTO.AdhocRequest; //objRateHighwayDTO.AdhocRequest.FormatWith(objRateHighwayDTO);
            SaveSearchSummary(objSearchModelDTO, objProviderRequest.URL);
            ResponseModel response = SendRateHighwayAutomationAsync(objProviderRequest);
            ProcessResponse(response, objSearchModelDTO.SearchSummaryID);
            return response.StatusDescription;
        }

        private ResponseModel SendRateHighwayAutomationAsync(ProviderRequest objProviderRequest)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(objProviderRequest.URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = (1000 * 60) * 10;
                if (objProviderRequest.HeaderItems.Count > 0)
                {
                    AddRequestHeaders(httpWebRequest, objProviderRequest.HeaderItems);
                }
                if (objProviderRequest.HttpMethod == Enumerations.HttpMethod.Post)
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(objProviderRequest.ProviderRequestData);
                    Stream objRequestStream = null;
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentLength = bytes.Length;
                    objRequestStream = httpWebRequest.GetRequestStream();
                    objRequestStream.Write(bytes, 0, bytes.Length);
                    objRequestStream.Close();
                }
                else
                {
                    httpWebRequest.Method = "GET";
                }

                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            return new ResponseModel(reader.ReadToEnd(), httpResponse.StatusCode, httpResponse.StatusDescription);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        return new ResponseModel(reader.ReadToEnd(), httpResponse.StatusCode, httpResponse.StatusDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, HttpStatusCode.SeeOther, ex.Message);
            }
        }

        private void AddRequestHeaders(HttpWebRequest request, Dictionary<string, string> headerItems)
        {
            foreach (KeyValuePair<string, string> item in headerItems)
            {
                request.Headers.Add(item.Key, item.Value);
            }
        }

        void ProcessResponse(ResponseModel response, long searchSummaryID, long shopRequestID = 0)
        {
            if (response.StatusCode == HttpStatusCode.SeeOther)
            {
                UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: response.StatusDescription);
                return;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            dynamic jsonObject = serializer.Deserialize<dynamic>(response.Result);
            var keys = jsonObject.Keys as Dictionary<string, object>.KeyCollection;

            if (searchSummaryID == 0)
            {
                long shopRequestId;
                if (shopRequestID > 0)
                {
                    shopRequestId = shopRequestID;
                }
                else
                {
                    long.TryParse(Convert.ToString(jsonObject["ShopRequestId"]), out shopRequestId);
                }

                if (shopRequestId > 0)
                {
                    searchSummaryID = GetSummaryIdByShopId(shopRequestId);
                    if (!(searchSummaryID > 0))
                    {
                        //if shop request id does not exists in database then return
                        return;
                    }
                }
            }

            if (keys.Contains("FaultString"))
            {
                var reason = jsonObject["FaultString"];

                //Update SearchSummary Table to reflect status
                UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: reason);

            }
            else if (keys.Contains("ErrorCode") && Convert.ToInt64(jsonObject["ErrorCode"]) > 0)
            {
                // if error occured in callback 
                var reason = Convert.ToString(jsonObject["ErrorMessage"]);

                //Update SearchSummary Table to reflect status
                UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: reason);
            }
            else if (keys.FirstOrDefault().Equals("ShopRequestId", StringComparison.OrdinalIgnoreCase))
            {
                long shopRequestId;
                long.TryParse(Convert.ToString(jsonObject["ShopRequestId"]), out shopRequestId);
                UpdateSearchStatus(searchSummaryID, shopRequestId, response: "OK");
                //request for the callback method
                RateHighwayCallBackDTO objRateHighwayCallBackDTO = new RateHighwayCallBackDTO();
                objRateHighwayCallBackDTO.ShopRequestId = shopRequestId;
                objRateHighwayCallBackDTO.RateShopperEndPoint = ConfigurationManager.AppSettings["RateHighwayEndPoint"];
                SendCallBackRequest(objRateHighwayCallBackDTO);
            }
            else if (keys.FirstOrDefault().Equals("RequestId", StringComparison.OrdinalIgnoreCase))
            {
                _iCallbackResponseService.SaveResponse(searchSummaryID, shopRequestID, response.Result);
            }
        }

        void SendCallBackRequest(RateHighwayCallBackDTO objRateHighwayCallBackDTO)
        {
            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.ProviderRequestData = JsonConvert.SerializeObject(objRateHighwayCallBackDTO);
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.HeaderItems.Add("AccessId", objRateHighwayCallBackDTO.AccessID);
            objProviderRequest.URL = objRateHighwayCallBackDTO.CallBackURL;
            ResponseModel response = SendRateHighwayAutomationAsync(objProviderRequest);
            ProcessResponse(response, 0, objRateHighwayCallBackDTO.ShopRequestId);

        }

        private string AlterScraperQueryString(string oldUri, SearchSummary objNewSearchSummaryEntity)
        {
            var uri = new Uri(oldUri);
            var qs = System.Web.HttpUtility.ParseQueryString(uri.Query, System.Text.Encoding.UTF8);
            qs.Set("searchid", objNewSearchSummaryEntity.ID.ToString());
            qs.Set("strtdt", objNewSearchSummaryEntity.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            qs.Set("enddt", objNewSearchSummaryEntity.EndDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = System.Web.HttpUtility.UrlDecode(qs.ToString());
            return uriBuilder.Uri.ToString();
        }

        private string AlterRateHighwayPostData(string postData, SearchSummary objNewSearchSummaryEntity)
        {

            RateHighwayDTO objRateHighwayDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<RateHighwayDTO>(postData);
            if (objNewSearchSummaryEntity.StartDate != null && objNewSearchSummaryEntity.StartDate.ToShortDateString() != "1/1/0001")
            {
                objRateHighwayDTO.PickupDateTime = objNewSearchSummaryEntity.StartDate.ToString("yyyy-MM-ddTHH:mm");
            }
            if (objNewSearchSummaryEntity.EndDate != null && objNewSearchSummaryEntity.EndDate.ToShortDateString() != "1/1/0001")
            {
                objRateHighwayDTO.ReturnDateTime = objNewSearchSummaryEntity.EndDate.ToString("yyyy-MM-ddTHH:mm");
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(objRateHighwayDTO);
        }

        //-----FTB Summary Shop operation

        public async Task<SearchSummaryResult> GetFTBSearchSummaryData(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId)
        {

            SearchSummaryResult searchSummaryResult = new SearchSummaryResult();
            searchSummaryResult.lstSearchSummaryData = new List<SearchSummeryData>();

            DateTime lastdate = DateTime.Now.Date.AddDays(getLastDaysRecord);
            //var searchSummaryData = _context.SearchSummaries.Where(obj => obj.StatusID != statusID && obj.ScheduledJobID == null && obj.CreatedDateTime >= lastdate);
            List<SearchSummary> searchSummaryData = null;

            DateTime fetchAfterDateTime;
            //if  objLastModifieddate is empty implies first time call to search summary
            if (string.IsNullOrEmpty(objLastModifieddate))
            {
                searchSummaryData = await _context.SearchSummaries.Where(obj => obj.CreatedDateTime >= lastdate && (isAdminUser ? true : obj.CreatedBy == selectedUserId) && obj.StatusID != statusID
                    && !string.IsNullOrEmpty(obj.ShopType) && obj.ShopType.TrimEnd() == "SummaryShop").ToListAsync();
            }
            else
            {
                fetchAfterDateTime = Convert.ToDateTime(objLastModifieddate);
                searchSummaryData = await _context.SearchSummaries.Where(obj => obj.UpdatedDateTime >= fetchAfterDateTime && (isAdminUser ? true : obj.CreatedBy == selectedUserId) && obj.StatusID != statusID && !string.IsNullOrEmpty(obj.ShopType)
                    && obj.ShopType.TrimEnd() == "SummaryShop" && obj.CreatedDateTime >= lastdate && (!obj.IsQuickView.HasValue || (obj.IsQuickView.HasValue && !obj.IsQuickView.Value)) && (!obj.IsGov.HasValue || (obj.IsGov.HasValue && !obj.IsGov.Value))).ToListAsync();
            }
            if (searchSummaryData != null && searchSummaryData.Count() > 0)
            {
                List<SearchSummary> searchSummaries = searchSummaryData.OrderByDescending(obj => obj.UpdatedDateTime).ToList();
                //var lastModifiedDateFromDB = searchSummaries.FirstOrDefault().UpdatedDateTime.AddDays(getLastDaysRecord);
                var lastModifiedDateFromDB = searchSummaries.FirstOrDefault().UpdatedDateTime;

                if (string.IsNullOrEmpty(objLastModifieddate) || (lastModifiedDateFromDB.AddMilliseconds(-lastModifiedDateFromDB.Millisecond) > Convert.ToDateTime(objLastModifieddate)))
                {
                    searchSummaryResult.lastModifiedDate = Convert.ToString(searchSummaries.FirstOrDefault().UpdatedDateTime);
                    //letlast date from search summary
                    //var lastdate = _context.SearchSummaries.OrderByDescending(obj => obj.CreatedDateTime).FirstOrDefault().CreatedDateTime.AddDays(getLastDaysRecord);

                    var UserLocationBrandID = await _context.UserLocationBrands.Where(obj => obj.UserID == LoggedInUserID).Select(obj => obj.LocationBrandID).ToListAsync();

                    ///get the last 3 days search summary record
                    var last3DaysRecord = (from ss in searchSummaryData
                                           orderby ss.CreatedDateTime descending
                                           select ss).ToList();

                    //Multiple location will be implemented.
                    var finalData = (from lastrecord in last3DaysRecord
                                     join user in _context.Users on lastrecord.CreatedBy equals user.ID
                                     join userlocation in UserLocationBrandID on Convert.ToInt32(lastrecord.LocationBrandIDs) equals userlocation
                                     join locationBrand in _context.LocationBrands on Convert.ToInt32(lastrecord.LocationBrandIDs) equals locationBrand.ID
                                     where !(locationBrand.IsDeleted)
                                     select new
                                     {
                                         ID = lastrecord.ID,
                                         StartDate = lastrecord.StartDate,
                                         EndDate = lastrecord.EndDate,
                                         CreatedDateTime = lastrecord.CreatedDateTime,
                                         CreatedBy = lastrecord.CreatedBy,
                                         CarClassesIDs = lastrecord.CarClassesIDs,
                                         LocationBrandIDs = lastrecord.LocationBrandIDs,
                                         RentalLengthIDs = lastrecord.RentalLengthIDs,
                                         ScrapperSourceIDs = lastrecord.ScrapperSourceIDs,
                                         StatusID = lastrecord.StatusID,
                                         Response = lastrecord.Response,
                                         BrandID = locationBrand.BrandID,
                                         UserName = user.UserName,
                                         FullName = user.FirstName + " " + user.LastName,
                                         UserID = user.ID,
                                         IsQuickView = lastrecord.IsQuickView,
                                         HasQuickViewChild = lastrecord.HasQuickViewChild,
                                         IsGOV = lastrecord.IsGov.HasValue ? lastrecord.IsGov.Value : false,
                                         IsAutomationSummary = lastrecord.ScheduledJobID.HasValue ? true : false,
                                         IsFTBSummary = lastrecord.FTBScheduledJobID.HasValue ? true : false
                                     }).ToList();

                    //var QuickViewSearchList = (from searchSummary in finalData
                    //                           join quickview in _context.QuickView on searchSummary.ID equals quickview.ParentSearchSummaryId
                    //                           where quickview.CreatedDateTime == currentDateTime && !(quickview.IsDeleted)
                    //                           select searchSummary).ToList();

                    IEnumerable<ScrapperSource> lstScrapperSource = await _context.ScrapperSources.ToListAsync();
                    IEnumerable<LocationBrand> lstLocationBrands = await _context.LocationBrands.ToListAsync();
                    IEnumerable<RentalLength> lstRentalLengths = await _context.RentalLengths.ToListAsync();
                    IEnumerable<CarClass> lstCarClasses = await _context.CarClasses.ToListAsync();

                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //Loop for final record and set format
                    foreach (var lastDaysRecord in finalData)
                    {
                        //bool? IsQuickViewCheck = (lastDaysRecord.IsQuickView != null) ? lastDaysRecord.IsQuickView : false;
                        ////This condition is check for if quickview is fall with current date and that quickview should display in search summary
                        //if (IsQuickViewCheck != null && IsQuickViewCheck == true)
                        //{
                        //    if (QuickViewSearchList.Where(obj => obj.ID == lastDaysRecord.ID).ToList().Count() == 0)
                        //    {
                        //        continue;
                        //    }
                        //}
                        SearchSummeryData _SearchSummary = new SearchSummeryData();
                        _SearchSummary.SearchSummaryID = lastDaysRecord.ID;
                        _SearchSummary.StartDate = lastDaysRecord.StartDate; //lastDaysRecord.StartDate.ToString("MMM dd HH:MM");						
                        _SearchSummary.EndDate = lastDaysRecord.EndDate; //lastDaysRecord.EndDate.ToString("MMM dd HH:MM");
                        _SearchSummary.StartTime = _SearchSummary.StartDate.ToString("HH:mm");
                        _SearchSummary.EndTime = _SearchSummary.EndDate.ToString("HH:mm");
                        _SearchSummary.CreatedDate = lastDaysRecord.CreatedDateTime; //lastDaysRecord.CreatedDateTime.ToString("MMM dd HH:MM");
                        _SearchSummary.IsFTBSummary = lastDaysRecord.IsFTBSummary;
                        _SearchSummary.IsAutomationSummary = lastDaysRecord.IsAutomationSummary;

                        //User user = new User();
                        //user = _userService.GetAll().Where(obj => obj.ID == lastDaysRecord.CreatedBy).FirstOrDefault(); //_userService.GetById(lastDaysRecord.CreatedBy); // _context.Users.Where(obj => obj.ID == item.CreatedBy).SingleOrDefault();//Get the user details 
                        _SearchSummary.UserName = lastDaysRecord.UserName;
                        _SearchSummary.FullName = lastDaysRecord.FullName;
                        _SearchSummary.UserID = lastDaysRecord.UserID;

                        #region split string value
                        IEnumerable<long> carid = Common.StringToLongList(lastDaysRecord.CarClassesIDs).OrderBy(obj => obj);
                        IEnumerable<long> locationbrandid = Common.StringToLongList(lastDaysRecord.LocationBrandIDs).OrderBy(obj => obj);
                        IEnumerable<long> lengthid = Common.StringToLongList(lastDaysRecord.RentalLengthIDs).OrderBy(obj => obj);
                        IEnumerable<long> sourceid = Common.StringToLongList(lastDaysRecord.ScrapperSourceIDs).OrderBy(obj => obj);

                        #endregion


                        //_SearchSummary.CarClassIDs = string.Join(",", carid.ToArray()).TrimEnd(',');
                        _SearchSummary.CarClassIDs = string.Join(",", (from carclasses in lstCarClasses
                                                                       join objcarid in carid on carclasses.ID equals objcarid
                                                                       orderby carclasses.DisplayOrder
                                                                       select objcarid).ToArray());
                        _SearchSummary.CarClassName = string.Join(", ", (from carclasses in lstCarClasses
                                                                         join objcarid in carid on carclasses.ID equals objcarid
                                                                         orderby carclasses.DisplayOrder
                                                                         select carclasses.Code).ToArray());

                        //-------------Length opertion-------
                        _SearchSummary.RentalLengthsIDs = string.Join(",", lengthid.ToArray()).TrimEnd(',');
                        _SearchSummary.RentalLengthName = string.Join(", ", (from rentallengths in lstRentalLengths
                                                                             join objlengthid in lengthid on rentallengths.MappedID equals objlengthid
                                                                             select rentallengths.Code).ToArray());

                        //-------------Source opertaion-------
                        _SearchSummary.SourcesIDs = string.Join(",", sourceid.ToArray()).TrimEnd(',');
                        //-------------Source opertaion-------
                        string tempSourceName = string.Join(", ", (from scrappersource in lstScrapperSource
                                                                   join objsourceid in sourceid on scrappersource.ID equals objsourceid
                                                                   select scrappersource.Name).ToArray());
                        _SearchSummary.SourceName = tempSourceName.TrimEnd(',');


                        //-----------Location operation-------                                       
                        _SearchSummary.LocationsBrandIDs = string.Join(",", locationbrandid.ToArray()).TrimEnd(',');
                        _SearchSummary.LocationName = string.Join(", ", (from locationbrands in lstLocationBrands
                                                                         join objlocationbrandid in locationbrandid on locationbrands.ID equals objlocationbrandid
                                                                         select locationbrands.LocationBrandAlias).ToArray());

                        _SearchSummary.LocationIDs = string.Join(",", (from locationbrands in lstLocationBrands
                                                                       join objlocationbrandid in locationbrandid on locationbrands.ID equals objlocationbrandid
                                                                       select locationbrands.LocationID).ToArray());

                        var utcDate = TimeZoneInfo.ConvertTimeToUtc(_SearchSummary.CreatedDate, easternZone);
                        DateTime clientDateTime = utcDate.AddMinutes(-1 * clientTimezoneOffset);

                        //monthNames[varCreateDate.getMonth()] + " " + varCreateDate.getDate() + " - " + (varCreateDate.getHours() < 10 ? '0' : '') + varCreateDate.getHours() + ":" + (varCreateDate.getMinutes() < 10 ? '0' : '') + varCreateDate.getMinutes() + " - " + tempLocationName.trim().substring(0, tempLocationName.trim().length - 1) + " - " + data.UserName;

                        _SearchSummary.SummaryText = String.Format("{0:MMMM dd - HH:mm}", clientDateTime) + " - " + _SearchSummary.LocationName + " - " + _SearchSummary.UserName;

                        if (lastDaysRecord.StatusID == 1 || lastDaysRecord.StatusID == 2 || lastDaysRecord.StatusID == 3)
                        {
                            _SearchSummary.StatusIDs = 1;
                            _SearchSummary.StatusName = "IN PROGRESS";
                            _SearchSummary.FailureResponse = Convert.ToString(lastDaysRecord.Response);
                            _SearchSummary.StatusClass = "statusip";
                        }
                        else
                        {
                            _SearchSummary.StatusIDs = lastDaysRecord.StatusID;
                            _SearchSummary.StatusName = _statusService.GetById(lastDaysRecord.StatusID).Status; // _context.StatuseDesc.Where(obj => obj.ID == item.StatusID).Select(obj=>obj.Status).SingleOrDefault();
                            _SearchSummary.FailureResponse = Convert.ToString(lastDaysRecord.Response);
                            if (lastDaysRecord.StatusID == 4)
                            {
                                _SearchSummary.StatusName = "COMPLETE";
                                _SearchSummary.StatusClass = "statusc";
                            }
                            else if (lastDaysRecord.StatusID == 5)
                            {
                                _SearchSummary.StatusName = "FAILED";
                                _SearchSummary.StatusClass = "statusf";
                            }
                        }



                        _SearchSummary.BrandIDs = Convert.ToString(lastDaysRecord.BrandID);
                        //commented to show previous day quick view shops
                        //_SearchSummary.IsQuickView = (lastDaysRecord.CreatedDateTime.Date == userSystemDate.Date) ? ((lastDaysRecord.IsQuickView != null) ? lastDaysRecord.IsQuickView : false) : false;
                        _SearchSummary.IsQuickView = lastDaysRecord.IsQuickView.HasValue ? lastDaysRecord.IsQuickView.Value : false;
                        _SearchSummary.HasQuickViewChild = lastDaysRecord.HasQuickViewChild.HasValue ? lastDaysRecord.HasQuickViewChild.Value : false;
                        _SearchSummary.IsGOV = lastDaysRecord.IsGOV;

                        if (_SearchSummary.IsQuickView.Value)
                        {
                            _SearchSummary.SearchTypeClass = "quick-view-past-search hidden";
                        }
                        else if (_SearchSummary.HasQuickViewChild.Value && !_SearchSummary.IsQuickView.Value)
                        {
                            _SearchSummary.SearchTypeClass = "has-quick-view-search hidden";
                        }
                        else
                        {
                            _SearchSummary.SearchTypeClass = "hidden";
                        }
                        searchSummaryResult.lstSearchSummaryData.Add(_SearchSummary);
                    }
                }
                else
                {
                    searchSummaryResult.lastModifiedDate = objLastModifieddate;
                }
            }
            //System.Diagnostics.Debug.WriteLine("End: " + DateTime.Now.ToString());

            return searchSummaryResult;

        }


        public async Task<SearchSummaryResult> GetFTBSearchSummaryDataFromSP(string objLastModifieddate, int clientTimezoneOffset, long LoggedInUserID, int statusID, int getLastDaysRecord, DateTime userSystemDate, bool isAdminUser, long selectedUserId)
        {

            SearchSummaryResult searchSummaryResult = new SearchSummaryResult();
            searchSummaryResult.lstSearchSummaryData = new List<SearchSummeryData>();

            DateTime lastdate = DateTime.Now.Date.AddDays(getLastDaysRecord);

            DateTime? fetchAfterDateTime = (DateTime?)null;

            const string queryFetchSummaryList = "EXEC [getSearchSummaryData] @objLastModifieddate=@objLastModifieddate,@selectedUserId=@selectedUserId,@statusID=@statusID,@getLastDaysRecord=@getLastDaysRecord,@fetchSummaryShops=@fetchSummaryShops,@isAdmin=@isAdmin";

            List<SqlParameter> queryParams = new List<SqlParameter>();
            if (string.IsNullOrEmpty(objLastModifieddate))
            {
                queryParams.Add(new SqlParameter("@objLastModifieddate", DBNull.Value));
            }
            else
            {
                fetchAfterDateTime = Convert.ToDateTime(objLastModifieddate);
                queryParams.Add(new SqlParameter("@objLastModifieddate", fetchAfterDateTime.Value));
            }

            queryParams.Add(new SqlParameter("@selectedUserId", selectedUserId));
            queryParams.Add(new SqlParameter("@statusID", statusID));
            queryParams.Add(new SqlParameter("@getLastDaysRecord", getLastDaysRecord));
            queryParams.Add(new SqlParameter("@fetchSummaryShops", true));
            queryParams.Add(new SqlParameter("@isAdmin", isAdminUser));

            var searchSummaryList = await _context.ExecuteSQLQuery<SearchSummeryData>(queryFetchSummaryList, queryParams.ToArray());

            if (searchSummaryList != null && searchSummaryList.Count() > 0)
            {
                //List<SearchSummary> searchSummaries = searchSummaryData.OrderByDescending(obj => obj.UpdatedDateTime).ToList();

                var lastModifiedDateFromDB = searchSummaryList.FirstOrDefault().UpdatedDateTime;

                if (string.IsNullOrEmpty(objLastModifieddate) || (lastModifiedDateFromDB.AddMilliseconds(-lastModifiedDateFromDB.Millisecond) > Convert.ToDateTime(objLastModifieddate)))
                {
                    searchSummaryResult.lastModifiedDate = Convert.ToString(searchSummaryList.FirstOrDefault().UpdatedDateTime);

                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime clientDateTime;
                    DateTime utcDate;
                    foreach (var summary in searchSummaryList)
                    {
                        utcDate = TimeZoneInfo.ConvertTimeToUtc(summary.CreatedDate, easternZone);
                        clientDateTime = utcDate.AddMinutes(-1 * clientTimezoneOffset);

                        summary.SummaryText = String.Format("{0:MMMM dd - HH:mm}", clientDateTime) + " - " + summary.LocationName + " - " + summary.UserName;

                        if (summary.StatusIDs == 1 || summary.StatusIDs == 2 || summary.StatusIDs == 3)
                        {
                            summary.StatusIDs = 1;
                            summary.StatusName = "IN PROGRESS";
                            summary.FailureResponse = Convert.ToString(summary.FailureResponse);
                            summary.StatusClass = "statusip";
                        }
                        else
                        {
                            summary.StatusIDs = summary.StatusIDs;
                            summary.StatusName = _statusService.GetById(summary.StatusIDs).Status; // _context.StatuseDesc.Where(obj => obj.ID == item.StatusID).Select(obj=>obj.Status).SingleOrDefault();
                            summary.FailureResponse = Convert.ToString(summary.FailureResponse);
                            if (summary.StatusIDs == 4)
                            {
                                summary.StatusName = "COMPLETE";
                                summary.StatusClass = "statusc";
                            }
                            else if (summary.StatusIDs == 5)
                            {
                                summary.StatusName = "FAILED";
                                summary.StatusClass = "statusf";
                            }
                        }
                        searchSummaryResult.lstSearchSummaryData.Add(summary);
                    }
                }
                else
                {
                    searchSummaryResult.lastModifiedDate = objLastModifieddate;
                }
            }
            //System.Diagnostics.Debug.WriteLine("End: " + DateTime.Now.ToString());

            return searchSummaryResult;

        }


        //----End FTB Summary Shop operation

    }
}
