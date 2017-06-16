using RateShopper.Domain.DTOs;
using RateShopper.Providers.Interface;
using RateShopper.Providers.Model;
using RateShopper.Providers.Model.ModelFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Providers.Helper;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using RateShopper.Services.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;

namespace RateShopper.Providers.Providers
{
    public class RateHighway : IRateHighway
    {
        IRequestProcessor iRequestProcessor;
        ISearchSummaryService iSearchSummary;
        ISearchResultRawDataService iSearchResultRawData;
        ISearchResultsService iSearchResults;
        ILocationService iLocation;
        ICompanyService iCompany;
        IRentalLengthService iRentalLength;
        ICarClassService iCarClass;
        IScrapperSourceService iScrapperSource;
        ICallbackResponseService iCallbackResponseService;
        private const string FaultCode = "FaultString";
        private const string ShopId = "ShopRequestId";
        private const string Rates = "ShoppedRates";
        private const string ErrorCode = "ErrorCode";
        private const string ErrorMessage = "ErrorMessage";
        private const string RequestId = "RequestId";

        public RateHighway(IRequestProcessor _requestProcessor
            , ISearchSummaryService _iSearchSummary, ISearchResultRawDataService _iSearchResultRawData, ISearchResultsService _iSearchResults,
           ILocationService _iLocation, ICompanyService _iCompany, IRentalLengthService _iRentalLength, ICarClassService _iCarClass, IScrapperSourceService _iScrapperSource, ICallbackResponseService _iCallbackResponseService)
        {
            this.iRequestProcessor = _requestProcessor;
            this.iSearchSummary = _iSearchSummary;
            this.iSearchResultRawData = _iSearchResultRawData;
            this.iSearchResults = _iSearchResults;
            iLocation = _iLocation;
            iCompany = _iCompany;
            iRentalLength = _iRentalLength;
            iCarClass = _iCarClass;
            iScrapperSource = _iScrapperSource;
            iCallbackResponseService = _iCallbackResponseService;
        }

        public async Task<string> SendRequest(SearchModelDTO objSearchModelDTO)
        {
            RateHighwayDTO objRateHighwayDTO = new RateHighwayDTO(objSearchModelDTO);
            
            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.ProviderRequestData = JsonConvert.SerializeObject(objRateHighwayDTO);
            objSearchModelDTO.PostData = objProviderRequest.ProviderRequestData;
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.HeaderItems.Add("AccessId", objRateHighwayDTO.AccessID);
            objProviderRequest.URL = objRateHighwayDTO.AdhocRequest; //objRateHighwayDTO.AdhocRequest.FormatWith(objRateHighwayDTO);
            iSearchSummary.SaveSearchSummary(objSearchModelDTO, objProviderRequest.URL);
            ResponseModel response = await iRequestProcessor.SendAsync(objProviderRequest);
            ProcessResponse(response, objSearchModelDTO.SearchSummaryID, Enumerations.RateHighwayRequestType.AdHocRequest);
            return response.StatusDescription;
        }

        public void ParseResponse(string responseString, long searchSummaryID, long shopRequestID = 0)
        {


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            dynamic jsonObject = serializer.Deserialize<dynamic>(responseString);
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
                    long.TryParse(Convert.ToString(jsonObject[ShopId]), out shopRequestId);
                }

                if (shopRequestId > 0)
                {
                    searchSummaryID = iSearchSummary.GetSummaryIdByShopId(shopRequestId);
                    if (!(searchSummaryID > 0))
                    {
                        //if shop request id does not exists in database then return
                        return;
                    }
                }
            }

            if (keys.Contains(FaultCode))
            {
                //if error occured while requesting api
                var reason = Convert.ToString(jsonObject[FaultCode]);

                //Update SearchSummary Table to reflect status
                iSearchSummary.UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: reason);
            }
            else if (keys.Contains(ErrorCode) && Convert.ToInt64(jsonObject[ErrorCode]) > 0)
            {
                // if error occured in callback 
                var reason = Convert.ToString(jsonObject[ErrorMessage]);

                //Update SearchSummary Table to reflect status
                iSearchSummary.UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: reason);
            }
            else if (keys.FirstOrDefault().Equals(ShopId, StringComparison.OrdinalIgnoreCase))
            {
                long shopRequestId;
                long.TryParse(Convert.ToString(jsonObject[ShopId]), out shopRequestId);
                iSearchSummary.UpdateSearchStatus(searchSummaryID, shopRequestId, response: "OK");
                //request for the callback method
                RateHighwayCallBackDTO objRateHighwayCallBackDTO = new RateHighwayCallBackDTO();
                objRateHighwayCallBackDTO.ShopRequestId = shopRequestId;
                objRateHighwayCallBackDTO.RateShopperEndPoint = ConfigurationManager.AppSettings["RateHighwayEndPoint"];
                Task.Run(() => SendCallBackRequest(objRateHighwayCallBackDTO));

            }            
            else if (keys.Contains(Rates))
            {
                #region Insert Raw Json Response Data in SearchResultRawData
                iSearchResultRawData.SaveRawData(searchSummaryID, responseString);
                #endregion

                Dictionary<string, long> locations = new Dictionary<string, long>();
                Dictionary<string, long> carClasses = new Dictionary<string, long>();
                Dictionary<long, long> rentalLengths = new Dictionary<long, long>();
                Dictionary<string, long> scrapperSources = new Dictionary<string, long>();
                Dictionary<string, long> vendorCodes = new Dictionary<string, long>();
                long scrapperSourceID, carClassId, locationId, vendorId, rentalLengthId;

                //Remove all cache object
                //_cacheManager.RemoveAllCacheObjects();

                locations = iLocation.GetLocationDictionary();
                carClasses = iCarClass.GetCarClassDictionary();
                rentalLengths = iRentalLength.GetRentalLengthDictionary();
                scrapperSources = iScrapperSource.GetScrapperSourceDictionary();
                vendorCodes = iCompany.GetCompaniesDictionary();

                DateTimeOffset dtoArrivalDate;
                DateTimeOffset dtoReturnDate;
                List<MapTableResult> searchSuccess = new List<MapTableResult>();
                MapTableResult result = null;
                foreach (var searchresult in jsonObject[Rates])
                {
                    result = new MapTableResult();
                    result.SearchSummaryID = searchSummaryID;
                    result.ScrapperSourceID = scrapperSources.TryGetValue(searchresult["DataSource"], out scrapperSourceID) ? scrapperSources[searchresult["DataSource"]] : 0;//iGetId.GetScrapperSourceId(searchresult["DataSource"]);
                    result.LocationID = locations.TryGetValue(searchresult["CityCd"], out locationId) ? locations[searchresult["CityCd"]] : 0;//iGetId.GetLocationId(searchresult["CityCd"]);
                    result.RentalLengthID = rentalLengths.TryGetValue(Convert.ToInt64(searchresult["Lor"]), out rentalLengthId) ? rentalLengths[Convert.ToInt64(searchresult["Lor"])] : 0;//iGetId.GetRentalLengthId(searchresult["Lor"]);
                    result.CarClassID = carClasses.TryGetValue(searchresult["CarTypeCd"], out carClassId) ? carClasses[searchresult["CarTypeCd"]] : 0; //iGetId.GetCarClassId(searchresult["CarTypeCd"]);
                    result.CompanyID = vendorCodes.TryGetValue(Convert.ToString(searchresult["VendCd"]).Trim(), out vendorId) ? vendorCodes[Convert.ToString(searchresult["VendCd"]).Trim()] : 0;//iGetId.GetVenderCodeId(searchresult["VendCd"]);
                    result.BaseRate = Convert.ToDecimal(searchresult["RtAmt"]);
                    result.TotalRate = Convert.ToDecimal(searchresult["EstRentalChrgAmt"]);
                    result.UpdatedDateTime = DateTime.Now;
                    dtoArrivalDate = DateTimeOffset.Parse(searchresult["ArvDt"].ToString());
                    dtoReturnDate = DateTimeOffset.Parse(searchresult["RtrnDt"].ToString());
                    result.ArvDt = dtoArrivalDate.UtcDateTime.Date;
                    result.RtrnDt = dtoArrivalDate.UtcDateTime.Date;
                    searchSuccess.Add(result);
                }

                iSearchResults.BulkInsert(searchSuccess);

                //Update SearchSummary Table to reflect status
                iSearchSummary.UpdateSearchStatus(searchSummaryID, isRequestFromAPI: true);

                //call data processing service to generate search result JSON.
                Task.Run(() => iSearchResults.GenerateSeachResultProcesssedData(searchSummaryID));
            }
            else if (keys.FirstOrDefault().Equals(RequestId, StringComparison.OrdinalIgnoreCase))
            {
                iCallbackResponseService.SaveResponse(searchSummaryID, shopRequestID, responseString);
            }
        }

        void ProcessResponse(ResponseModel response, long searchSummaryID, Enumerations.RateHighwayRequestType rateHighwayRequestType)
        {
            if (response.StatusCode == HttpStatusCode.SeeOther)
            {
                iSearchSummary.UpdateSearchStatus(searchSummaryID, isSearchFailed: true, response: response.StatusDescription);
                return;
            }
            ParseResponse(response.Result, searchSummaryID);

            //switch (rateHighwayRequestType)
            //{
            //    case Enumerations.RateHighwayRequestType.AdHocRequest:

            //        break;
            //    case Enumerations.RateHighwayRequestType.ShopStatus:

            //        break;
            //    case Enumerations.RateHighwayRequestType.ShopResult:

            //        break;
            //}
        }

        async Task SendCallBackRequest(RateHighwayCallBackDTO objRateHighwayCallBackDTO)
        {
            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.ProviderRequestData = JsonConvert.SerializeObject(objRateHighwayCallBackDTO);
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.HeaderItems.Add("AccessId", objRateHighwayCallBackDTO.AccessID);
            objProviderRequest.URL = objRateHighwayCallBackDTO.CallBackURL;
            ResponseModel response = await iRequestProcessor.SendAsync(objProviderRequest);
            ParseResponse(response.Result, 0, objRateHighwayCallBackDTO.ShopRequestId);
        }
    }    
}
