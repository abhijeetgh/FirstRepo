
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
using RateShopper.Services.Helper;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Net;
using RateShopper.Domain.DTOs;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace RateShopper.Services.Data
{
    public class TSDTransactionService : BaseService<TSDTransaction>, ITSDTransactionsService
    {

        private ILocationBrandService _locationBrandService;
        private ISearchSummaryService _searchSummaryService;
        private IRateCodeService _rateCodeService;
        private ICompanyService _companyService;
        public TSDTransactionService(IEZRACRateShopperContext context, ICacheManager cacheManager, ILocationBrandService locationBrandService, ISearchSummaryService searchSummaryService, IRateCodeService rateCodeService, ICompanyService companyService
            )
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<TSDTransaction>();
            _cacheManager = cacheManager;
            _locationBrandService = locationBrandService;
            _searchSummaryService = searchSummaryService;
            _rateCodeService = rateCodeService;
            _companyService = companyService;
        }

        public async Task<string> ProcessRateSelection(List<TSDModel> tsdModelList, string UserName, string RateSystem, long LocationBrandID,
            long UserId, long SearchSummaryID, bool IsTetheredUpdate, string BrandLocation, bool isUpdateSuggestedRate = true, bool isFTBRateUpdate = false)
        {
            string response = string.Empty;
            try
            {
                if (tsdModelList.Count > 0)
                {
                    string RatesTobeUpdated = GenerateXML(tsdModelList, UserName, RateSystem, LocationBrandID, UserId, IsTetheredUpdate, SearchSummaryID, isFTBRateUpdate);
                    if (!string.IsNullOrEmpty(RatesTobeUpdated))
                    {
                        TSDPostRatesDTO objTSDPostRatesDTO = new TSDPostRatesDTO();
                        objTSDPostRatesDTO.TSDXML = RatesTobeUpdated;
                        objTSDPostRatesDTO.LocationBrandID = LocationBrandID;
                        objTSDPostRatesDTO.UserId = UserId;
                        objTSDPostRatesDTO.SearchSummaryID = SearchSummaryID;
                        objTSDPostRatesDTO.IsTetheredUpdate = IsTetheredUpdate;
                        objTSDPostRatesDTO.BrandLocation = BrandLocation;
                        objTSDPostRatesDTO.RetryCount = 1;

                        response = await PostTSDRates(objTSDPostRatesDTO, tsdModelList[0].RezTSDModel.IsRezCentral, tsdModelList[0].IsOpaqueRates);
                    }

                    //update the suggested rate after TSD submission for tsd update information
                    if (!IsTetheredUpdate && isUpdateSuggestedRate)
                    {
                        UpdatedSuggestedRate(tsdModelList, SearchSummaryID, UserId);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                SearchResultsService.Logger.WriteToLogFile("Inside ProcessRateSelection => SearchSummaryId: " + SearchSummaryID + ",  Exception Occured,Inner Exception: " + ex.InnerException
                    + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, SearchResultsService.Logger.GetLogFilePath());
                throw;
            }
        }


        public string GenerateXML(List<TSDModel> tsdModelList, string UserName, string RateSystem, long LocationBrandID, long UserId, bool IsTetheredUpdate, long SearchSummaryID, bool isFTBRateUpdate = false)
        {
            try
            {
                TRNXML helper = new TRNXML();
                Sender senderObj = new Sender();
                Recipient recipientObj = new Recipient();
                Message messageObj = new Message();
                TrainingPartner trainingPartner = new TrainingPartner();
                List<RateDetail> rateDetailsList = new List<RateDetail>();
                Customer customerObj = new Customer();
                decimal extraDayFactor = 0;
                string branchCode = string.Empty;
                bool useLORRateCode = true;
                string locationBrandAlias = string.Empty;
                string companyName = string.Empty;
                bool isGOVShop = false;
                SearchSummary searchShop = null;
                if (!isFTBRateUpdate)
                {
                    searchShop = _searchSummaryService.GetById(SearchSummaryID, false);
                }
                if (searchShop != null)
                {
                    isGOVShop = searchShop.IsGov.HasValue ? searchShop.IsGov.Value : false;
                }
                //Fetch Customer Number and Passcode from  Location Brand Table based on LocationBrandID
                //extra day factor is dependent on Location so carry ahead this factor
                if (LocationBrandID > 0)
                {
                    LocationBrand locationBrand = _locationBrandService.GetById(LocationBrandID, false);
                    if (locationBrand != null)
                    {
                        customerObj.CustomerNumber = locationBrand.TSDCustomerNumber;
                        customerObj.Passcode = locationBrand.TSDPassCode;
                        branchCode = locationBrand.BranchCode;
                        useLORRateCode = locationBrand.UseLORRateCode;
                        locationBrandAlias = locationBrand.LocationBrandAlias;

                        if (!string.IsNullOrEmpty(locationBrandAlias) && locationBrandAlias.Split('-').Length > 0)
                        {
                            companyName = locationBrandAlias.Split('-')[1].ToUpper();
                        }
                    }
                }
                string webLinkSystem = Convert.ToString(ConfigurationManager.AppSettings["EZRateSystem"]);
                string wspanSystem = Convert.ToString(ConfigurationManager.AppSettings["ADRateSystem"]);
                senderObj.SenderID = ConfigurationManager.AppSettings["SenderId"];
                senderObj.SenderName = UserName.Trim();
                recipientObj.RecipientID = ConfigurationManager.AppSettings["RecipientID"];
                recipientObj.RecipientName = ConfigurationManager.AppSettings["RecipientName"];
                messageObj.MessageID = ConfigurationManager.AppSettings["MessageID"];
                messageObj.MessageDesc = ConfigurationManager.AppSettings["MessageDesc"];
                trainingPartner.TradingPartnerCode = ConfigurationManager.AppSettings["TradingPartnerCode"];
                trainingPartner.TradingPartnerName = ConfigurationManager.AppSettings["TradingPartnerName"];
                int modelListCount, rentalLengthCount = 0;
                string rateTypeConfigured = string.Empty;
                string rateCodeConfigured = string.Empty;
                if (isGOVShop)
                {
                    rateTypeConfigured = ConfigurationManager.AppSettings["GOVRateType"];
                    rateCodeConfigured = ConfigurationManager.AppSettings["GOVRateCode"];
                }
                else { rateTypeConfigured = ConfigurationManager.AppSettings["RateType"]; }

                string lorRateCodeConfigured = ConfigurationManager.AppSettings["LORRateCode"];
                string[] rentalLengths;
                extraDayFactor = tsdModelList[0].ExtraDayRateFactor;


                for (modelListCount = 0; modelListCount <= tsdModelList.Count - 1; modelListCount++)
                {
                    if (!string.IsNullOrEmpty(tsdModelList[modelListCount].RentalLength))
                    {
                        rentalLengths = tsdModelList[modelListCount].RentalLength.Split(',');
                        for (rentalLengthCount = 0; rentalLengthCount <= rentalLengths.Length - 1; rentalLengthCount++)
                        {
                            RateDetail rates = new RateDetail();
                            rates.RemoteID = tsdModelList[modelListCount].RemoteID;
                            rates.Branch = !string.IsNullOrEmpty(branchCode) ? branchCode : tsdModelList[modelListCount].Branch;
                            rates.ClassCode = tsdModelList[modelListCount].CarClass.Trim();
                            rates.RateType = rateTypeConfigured;
                            rates.RateCode = useLORRateCode == true ? rentalLengths[rentalLengthCount] : lorRateCodeConfigured;
                            rates.StartDate = DateTime.ParseExact(tsdModelList[modelListCount].StartDate, "yyyyMMd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                            //send rental length while creating XML to decide whether daily rate parameter need to be added.
                            rates.RentalLength = rentalLengths[rentalLengthCount];
                            rates.PerMile = Convert.ToInt32(ConfigurationManager.AppSettings["PerMile"]);
                            //rates.ExtraDayRate = Math.Round((tsdModelList[modelListCount].DailyRate) *
                            //    extraDayFactor, 2);
                            //check the rental Lenght if Day rental lenghts then RatePlan=10 and DailyRate*ExtraDayFactor
                            //if week rental length then RatePlan=30 and DailyRate/ExtraDayFactor

                            string rentalLength = rentalLengths[rentalLengthCount].ToUpper();
                            SetRateParameters(rates, tsdModelList[modelListCount], rentalLength);

                            rates.ExtraDayFree = Convert.ToInt32(ConfigurationManager.AppSettings["ExtraDayFree"]);

                            if (companyName == "EZ")
                            {
                                rates.RateSystem = webLinkSystem;
                            }
                            else if (companyName == "AD")
                            {
                                rates.RateSystem = wspanSystem;
                            }

                            if (isGOVShop)
                            {
                                rates.RateCode = rateCodeConfigured;
                                rates.ExtraDayRate = Math.Truncate(rates.ExtraDayRate);
                                if (rentalLength == "W8" || rentalLength == "W9" || rentalLength == "W10" || rentalLength == "W11")
                                {
                                    if (rates.DailyRate.HasValue)
                                    {
                                        rates.DailyRate = Math.Truncate(rates.DailyRate.Value);
                                    }
                                }
                            }

                            rateDetailsList.Add(rates);

                            //check if tethered update and uselorratecode is not configured
                            if (IsTetheredUpdate && useLORRateCode == false)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                helper.Dategmtime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone).ToString("yyyy/MM/dd HH:mm:ss");
                helper.Sender = senderObj;
                helper.Recipient = recipientObj;
                helper.TrainingPartner = trainingPartner;
                helper.Customer = customerObj;
                helper.Message = messageObj;
                helper.Payload = rateDetailsList;

                #region Create XML
                XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                // Initializes a new instance of the XmlDocument class.          
                XmlSerializer xmlSerializer = new XmlSerializer(helper.GetType());
                //Creates a stream whose backing store is memory. 
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, helper);
                    xmlStream.Position = 0;
                    //Loads the XML document from the specified string.
                    xmlDoc.Load(xmlStream);
                    XmlElement root = xmlDoc.DocumentElement;
                    root.RemoveAttribute("xmlns:xsd");
                    root.RemoveAttribute("xmlns:xsi");
                    root.Attributes.Append(xmlDoc.CreateAttribute("version")).Value = ConfigurationManager.AppSettings["TRNXMLVersion"];
                    root.FirstChild.Attributes.Append(xmlDoc.CreateAttribute("TimeZone")).Value = ConfigurationManager.AppSettings["TimeZone"];
                    foreach (XmlNode node in xmlDoc)
                    {
                        if (node.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            xmlDoc.RemoveChild(node);
                        }

                    }
                    XmlNodeList nodes;
                    //remove rental length from XML as its not part of XML generated
                    nodes = xmlDoc.SelectNodes("//RentalLength");
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        nodes[i].ParentNode.RemoveChild(nodes[i]);
                    }

                #endregion

                }
                return xmlDoc.InnerXml;
            }
            catch (Exception ex)
            {
                SearchResultsService.Logger.WriteToLogFile("Inside GenerateXML => tsdModellist count: " + tsdModelList.Count + ",LocationBrandId:" + LocationBrandID + "Exception Occured,Base Exception: " + ex.GetBaseException().ToString()
                    + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, SearchResultsService.Logger.GetLogFilePath());
                return string.Empty;
            }
        }

        public async Task<string> PostTSDRates(TSDPostRatesDTO objTSDPostRatesDTO, bool isRezCentralUpdate, bool isOpaqueUpdate)
        {
            string statusCode = string.Empty;
            XmlDocument responseXML = new XmlDocument();
            string strMessageDesc = string.Empty;
            string strMessageID = string.Empty;
            string strErrorFound = string.Empty;
            bool sendTSDUpdates = bool.Parse(ConfigurationManager.AppSettings["SendTSDUpdates"]);
            bool isResponseNull = false;
            try
            {
                var dataToPost = Encoding.ASCII.GetBytes(objTSDPostRatesDTO.TSDXML);

                string TSDPostURL = string.Empty;
                //if (!IsTetheredUpdate)
                //    TSDPostURL = Convert.ToString(ConfigurationManager.AppSettings["TSDPostURL"]);
                //else
                //    TSDPostURL = Convert.ToString(ConfigurationManager.AppSettings["TSDPostURLAD"]);
                if (!string.IsNullOrEmpty(objTSDPostRatesDTO.BrandLocation))
                {
                    TSDPostURL = ConfigurationManager.AppSettings["TSDPostURL" + "-" + objTSDPostRatesDTO.BrandLocation];
                }

                if (!string.IsNullOrEmpty(TSDPostURL))
                {
                    if (sendTSDUpdates)
                    {
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(TSDPostURL);
                        httpWebRequest.ContentType = "application/xml";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.ContentLength = dataToPost.Length;
                        using (var stream = httpWebRequest.GetRequestStream())
                        {
                            stream.Write(dataToPost, 0, dataToPost.Length);
                        }
                        httpWebRequest.Timeout = (1000 * 60) * 10;
                        var httpResponse = await httpWebRequest.GetResponseAsync();
                        statusCode = ((System.Net.HttpWebResponse)(httpResponse)).StatusCode.ToString();
                        using (WebResponse response = httpResponse)
                        {
                            HttpWebResponse httpResponses = (HttpWebResponse)response;
                            responseXML.Load(httpResponse.GetResponseStream());
                        }
                        //close HTTP response
                        httpResponse.Close();
                        if (!string.IsNullOrEmpty(responseXML.InnerXml))
                        {
                            if (responseXML.GetElementsByTagName("MessageDescription") != null && responseXML.GetElementsByTagName("MessageDescription").Count > 0)
                                strMessageDesc = responseXML.GetElementsByTagName("MessageDescription").Item(0).InnerXml;
                            else
                                strMessageDesc = "";
                            if (responseXML.GetElementsByTagName("MessageID") != null && responseXML.GetElementsByTagName("MessageID").Count > 0)
                                strMessageID = responseXML.GetElementsByTagName("MessageID").Item(0).InnerXml;
                            else
                                strMessageID = "";
                            if (responseXML.GetElementsByTagName("ErrorFound") != null && responseXML.GetElementsByTagName("ErrorFound").Count > 0)
                                strErrorFound = responseXML.GetElementsByTagName("ErrorDescription").Item(0).InnerText;
                            else
                                strErrorFound = "";
                        }
                    }
                    SaveTSDTransaction(objTSDPostRatesDTO.LocationBrandID, objTSDPostRatesDTO.SearchSummaryID, strMessageID, strMessageDesc, objTSDPostRatesDTO.TSDXML,
                        statusCode, responseXML.InnerXml, strErrorFound, objTSDPostRatesDTO.UserId, isRezCentralUpdate, isOpaqueUpdate);
                }
            }
            catch (WebException e)
            {
                Exception exe = e.GetBaseException();
                string message = "Inside PostTSDRates---> webexception => SearchSummaryId: " + Convert.ToString(objTSDPostRatesDTO.SearchSummaryID) + ",  Exception Occured, Response is NULL from the TSD------," +
                    "Base Exception: " + exe != null ? Convert.ToString(exe.GetBaseException()) : e.StackTrace + " exception Message: " + e.Message + "---inner message:" + e.InnerException != null ? e.InnerException.Message : "" +
                    "----Retry count:" + objTSDPostRatesDTO.RetryCount.ToString();
                SearchResultsService.Logger.WriteToLogFile(message, SearchResultsService.Logger.GetLogFilePath());

                if (e != null && e.Response == null)
                {
                    isResponseNull = true;
                }
                else
                {
                    isResponseNull = false;
                    using (WebResponse response = e.Response)
                    {
                        statusCode = ((System.Net.HttpWebResponse)(response)).StatusCode.ToString();
                        HttpWebResponse httpResponses = (HttpWebResponse)response;
                        responseXML.Load(httpResponses.GetResponseStream());
                    }
                    if (!string.IsNullOrEmpty(responseXML.InnerXml))
                    {
                        if (responseXML.GetElementsByTagName("MessageDescription") != null && responseXML.GetElementsByTagName("MessageDescription").Count > 0)
                            strMessageDesc = responseXML.GetElementsByTagName("MessageDescription").Item(0).InnerXml;
                        else
                            strMessageDesc = "No Message Description Found!!";
                        if (responseXML.GetElementsByTagName("MessageID") != null && responseXML.GetElementsByTagName("MessageID").Count > 0)
                            strMessageID = responseXML.GetElementsByTagName("MessageID").Item(0).InnerXml;
                        else
                            strMessageID = "";
                        if (responseXML.GetElementsByTagName("ErrorFound") != null && responseXML.GetElementsByTagName("ErrorFound").Count > 0)
                            strErrorFound = responseXML.GetElementsByTagName("ErrorDescription").Item(0).InnerText;
                        else
                            strErrorFound = "";
                    }
                    SaveTSDTransaction(objTSDPostRatesDTO.LocationBrandID, objTSDPostRatesDTO.SearchSummaryID, strMessageID, strMessageDesc, objTSDPostRatesDTO.TSDXML, statusCode,
                        responseXML.InnerXml, strErrorFound, objTSDPostRatesDTO.UserId, isRezCentralUpdate, isOpaqueUpdate);
                }
            }
            catch (Exception ex)
            {
                string message = "Inside PostTSDRates---> exception => SearchSummaryId: " + Convert.ToString(objTSDPostRatesDTO.SearchSummaryID) + ",  Exception Occured------," +
                    "Base Exception: " + Convert.ToString(ex.GetBaseException()) + " exception Message: " + ex.Message + "---inner message:" + ex.InnerException != null ? ex.InnerException.Message : "";
                SearchResultsService.Logger.WriteToLogFile(message, SearchResultsService.Logger.GetLogFilePath());

                SearchResultsService.Logger.WriteToLogFile("Inside PostTSDRates => SearchSummaryId: " + objTSDPostRatesDTO.SearchSummaryID + ",  Exception Occured,Base Exception: " + ex.GetBaseException().ToString()
                    + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, SearchResultsService.Logger.GetLogFilePath());
                //throw ex;
            }
            if (isResponseNull && objTSDPostRatesDTO.RetryCount < 3)
            {
                objTSDPostRatesDTO.RetryCount++;
                string response = await PostTSDRates(objTSDPostRatesDTO, isRezCentralUpdate, isOpaqueUpdate);
                return response;
            }
            else if (isResponseNull && objTSDPostRatesDTO.RetryCount >= 3)
            {
                statusCode = "error occured while posting rates, retry count also exceeds";
            }
            return statusCode;
        }

        public void SaveTSDTransaction(long LocationBrandID, long SearchSummaryID, string ResponseCode, string Message,
            string XMLRequest, string RequestStatus, string XMLResponse, string ErrorFound, long UserId, bool isRezCentralUpdate, bool isOpaqueUpdate)
        {

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            string estTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone).ToString("yyyy/MM/dd HH:mm:ss");
            TSDTransaction TSDTransaction = new TSDTransaction
               {
                   LocationBrandID = LocationBrandID,
                   SearchSummaryID = SearchSummaryID,
                   ResponseCode = ResponseCode,
                   Message = Message,
                   XMLRequest = XMLRequest,
                   RequestStatus = RequestStatus,
                   XMLResponse = XMLResponse,
                   ErrorFound = ErrorFound,
                   CreatedBy = UserId,
                   UpdatedBy = UserId,
                   CreatedDateTime = DateTime.ParseExact(estTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture),
                   UpdatedDateTime = DateTime.ParseExact(estTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture),
                   IsRezCentralUpdate = isRezCentralUpdate,
                   IsOpaqueUpdate = isOpaqueUpdate
               };
            _context.TSDTransactions.Add(TSDTransaction);
            _context.SaveChanges();
        }

        public List<TSDDTO> GetTSDAuditLogs(long brandID)
        {
            List<TSDDTO> lstTSDAuditLogs = (from tSDAudit in _context.TSDTransactions
                                            join user in _context.Users on tSDAudit.CreatedBy equals user.ID
                                            join locationBrand in _context.LocationBrands on tSDAudit.LocationBrandID equals locationBrand.ID
                                            where brandID == 0 || (brandID > 0 && locationBrand.BrandID == brandID)
                                            orderby tSDAudit.UpdatedDateTime descending
                                            select new
                                            {
                                                ID = tSDAudit.ID,
                                                Name = user.FirstName + " " + user.LastName,
                                                LocationCode = locationBrand.LocationBrandAlias,
                                                LogDateTime = tSDAudit.UpdatedDateTime
                                            }).ToList().Select(d => new TSDDTO
                                            {
                                                ID = d.ID,
                                                Name = d.Name,
                                                LocationCode = d.LocationCode,
                                                StrLogDateTime = d.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss.f")
                                            }).ToList();

            //lstTSDAuditLogs.ForEach(d => d.StrLogDateTime = d.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss.f"));
            return lstTSDAuditLogs;
        }

        /// <summary>
        /// Get all companies/brands
        /// </summary>
        /// <returns></returns>
        public List<CompanyDTO> GetCompany()
        {
            List<CompanyDTO> companies = _context.Companies.Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            companies.Insert(0, new CompanyDTO { ID = 0, Name = "All", Code = "All" });
            return companies;
        }

        /// <summary>
        /// Get log details using audit id
        /// </summary>
        /// <param name="auditID"></param>
        /// <returns></returns>
        public TSDDTO GetLogDetail(long auditID)
        {
            TSDTransaction objTSDTransactionEntity = _context.TSDTransactions.Find(auditID);
            TSDDTO objTSDDTO = new TSDDTO();

            if (objTSDTransactionEntity != null)
            {
                objTSDDTO.ID = objTSDTransactionEntity.ID;
                objTSDDTO.ResponseCode = objTSDTransactionEntity.ResponseCode;
                objTSDDTO.RequestStatus = objTSDTransactionEntity.RequestStatus.ToUpper() == "OK" ? "Success" : objTSDTransactionEntity.RequestStatus;
                objTSDDTO.Message = objTSDTransactionEntity.Message;
                if (!string.IsNullOrEmpty(objTSDTransactionEntity.ErrorFound))
                {
                    objTSDDTO.RequestStatus = "Fail";
                    objTSDDTO.ErrorMessage = objTSDTransactionEntity.ErrorFound;
                }

                if (!string.IsNullOrEmpty(objTSDTransactionEntity.XMLRequest))
                {
                    objTSDDTO.XMLRequest = System.Xml.Linq.XElement.Parse(objTSDTransactionEntity.XMLRequest).ToString();
                }
            }
            else
            {
                objTSDDTO.ID = 0;
            }
            return objTSDDTO;
        }

        public void UpdatedSuggestedRate(List<TSDModel> tsdModelList, long searchSummaryID, long userID)
        {
            int modelListCount;
            List<SearchResultSuggestedRate> suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryID).ToList();
            if (suggestedRates != null && suggestedRates.Count > 0)
            {
                for (modelListCount = 0; modelListCount <= tsdModelList.Count - 1; modelListCount++)
                {

                    if (tsdModelList[modelListCount].SuggestedRateId > 0)
                    {
                        SearchResultSuggestedRate suggestedRate = suggestedRates.FirstOrDefault(obj => obj.ID == tsdModelList[modelListCount].SuggestedRateId);
                        if (suggestedRate != null)
                        {
                            suggestedRate.TSDUpdateDateTime = DateTime.Now;
                            suggestedRate.TSDUpdatedBy = userID;
                            _context.Entry(suggestedRate).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
            }
            _context.SaveChanges();
        }

        public string GetLastTSDUpdate(long searchSummaryId)
        {
            string lastUpdated = string.Empty;
            if (_context.TSDTransactions.Count() > 0 && _context.Users.Count() > 0)
            {
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var TSDRow = _context.TSDTransactions.Where(TSD => TSD.SearchSummaryID == searchSummaryId && string.IsNullOrEmpty(TSD.ErrorFound))
                    .Join(_context.Users, TSD => TSD.CreatedBy, users => users.ID, (TSD, users) =>
                        new { CreatedDateTime = TSD.CreatedDateTime, UserName = users.UserName })
                    .OrderByDescending(obj => obj.CreatedDateTime).FirstOrDefault();
                if (TSDRow != null)
                {
                    lastUpdated = TSDRow.UserName + "|" + TSDRow.CreatedDateTime.ToString("MMMM dd, yyyy, hh:mm tt");
                }
            }

            return lastUpdated;
        }


        public string updateDateTimeStamp(DateTime[] arrivalDates, long rentalLengthId, long searchSummaryId, List<long> carClasses, long userID)
        {
            string response = string.Empty;
            foreach (DateTime arrivalDate in arrivalDates)
            {
                foreach (long carClassId in carClasses)
                {
                    SearchResultSuggestedRate suggestedRate = _context.SearchResultSuggestedRates
                        .Where(result => result.SearchSummaryID == searchSummaryId && result.CarClassID == carClassId &&
                            result.Date == arrivalDate.Date && result.RentalLengthID == rentalLengthId && result.RuleSetID != 0).FirstOrDefault();
                    if (suggestedRate != null)
                    {
                        suggestedRate.TSDUpdateDateTime = DateTime.Now;
                        suggestedRate.TSDUpdatedBy = userID;
                        _context.Entry(suggestedRate).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
            _context.SaveChanges();
            return response;
        }

        public List<GlobalLimitDetailsDTO> getGlobalLimits(long locationBrandId, DateTime StartDate, DateTime EndDate)
        {
            List<GlobalLimitDetailsDTO> globalLimitDetails = new List<GlobalLimitDetailsDTO>();

            if (locationBrandId > 0)
            {
                globalLimitDetails = (from globallimit in _context.GlobalLimits
                                      join globallimitdetail in _context.GlobalLimitDetails on globallimit.ID equals globallimitdetail.GlobalLimitID
                                      orderby globallimitdetail.ID
                                      where globallimit.LocationBrandID == locationBrandId
                                      && globallimit.EndDate >= StartDate && globallimit.StartDate <= EndDate
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

            return globalLimitDetails;
        }

        public Formula getLocationFormula(long locationBrandId)
        {
            Formula objFormula = new Formula();
            if (locationBrandId > 0)
            {
                objFormula = _context.Formulas.Where(formula => formula.LocationBrandID == locationBrandId).FirstOrDefault();
            }
            return objFormula;
        }

        public async Task PushOpaqueRates(OpaqueRatesConfiguration opaqueRatesConfiguration, List<TSDModel> lstTSDData, string UserName, string RateSystem,
            long locationBrandId, long UserId, long SearchSummaryID, bool IsTetheredUpdate, string BrandLocation)
        {
            List<RateCodeDTO> lstRateCodes = new List<RateCodeDTO>();
            lstRateCodes = _rateCodeService.GetRateCodesWithDateRanges();

            if (opaqueRatesConfiguration != null && lstRateCodes.Count>0 && opaqueRatesConfiguration.OpaqueRates != null && opaqueRatesConfiguration.OpaqueRates.Count > 0)
            {
                long brandId = _locationBrandService.GetById(locationBrandId, false).BrandID;

                List<TSDModel> lstOpaqueTSDData = new List<TSDModel>();
                TSDModel opaqueTSDModel = null;
                decimal percentValue = 0;
                foreach (var tsdData in lstTSDData)
                {
                    opaqueTSDModel = new TSDModel();
                    if (opaqueRatesConfiguration.IsDailyView)
                    {
                        percentValue = opaqueRatesConfiguration.OpaqueRates.Where(d => d.CarCode.ToUpper().Equals(tsdData.CarClass.Trim().ToUpper())).Select(d => d.PercentValue).FirstOrDefault();
                    }
                    else if (opaqueRatesConfiguration.IsClassicView)
                    {
                        percentValue = opaqueRatesConfiguration.OpaqueRates.Where(d => d.Date.Equals(tsdData.StartDate.Trim())).Select(d => d.PercentValue).FirstOrDefault();
                    }
                    if (percentValue == 0)
                    {
                        continue;
                    }
                    opaqueTSDModel.DailyRate = Math.Round(tsdData.DailyRate + (tsdData.DailyRate * (percentValue / 100)), 2);
                    opaqueTSDModel.CarClass = tsdData.CarClass;
                    opaqueTSDModel.Branch = tsdData.Branch;
                    opaqueTSDModel.ExtraDayRateFactor = tsdData.ExtraDayRateFactor;
                    opaqueTSDModel.ExtraDayRateValue = tsdData.ExtraDayRateValue;
                    opaqueTSDModel.StartDate = tsdData.StartDate;
                    opaqueTSDModel.RentalLength = GetApplicableRateCodes(lstRateCodes, brandId, DateTime.ParseExact(opaqueTSDModel.StartDate, "yyyyMMd", CultureInfo.CurrentCulture));
                    if (string.IsNullOrEmpty(opaqueTSDModel.RentalLength))
                    {
                        string defaultRateCode = ConfigurationManager.AppSettings["DefaultRateCode"];
                        if (BrandLocation == "AD" && !string.IsNullOrEmpty(defaultRateCode))
                        {
                            opaqueTSDModel.RentalLength = defaultRateCode;
                        }                        
                        else
                        {
                            continue;
                        }
                    }
                    opaqueTSDModel.IsOpaqueRates = true;
                    if (tsdData.RentalLength.Length > 0 && tsdData.RentalLength.IndexOf("D") >= 0)
                    {
                        opaqueTSDModel.OpaqueTSDModel.IsDaily = true;
                    }
                    else if (tsdData.RentalLength.Length > 0 && tsdData.RentalLength.IndexOf("W") >= 0)
                    {
                        opaqueTSDModel.OpaqueTSDModel.IsWeekly = true;
                    }

                    lstOpaqueTSDData.Add(opaqueTSDModel);

                    percentValue = 0;
                }
                await ProcessRateSelection(lstOpaqueTSDData, UserName, RateSystem, locationBrandId, UserId, SearchSummaryID, false, BrandLocation, false, false);
            }

        }

        public async Task<string> PushRezCentralRates(RezCentralDTO objRezCentralDTO)
        {
            List<string> applicableLocations = new List<string>();
            List<string> applicableRateCodes = new List<string>();

            List<RateCodeDTO> lstRateCodes = new List<RateCodeDTO>();
            lstRateCodes = _rateCodeService.GetRateCodesWithDateRanges();
            string rateCodes = string.Empty;

            long brandId = 0;

            DateTime StartDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;

            if (!string.IsNullOrEmpty(objRezCentralDTO.StartDate))
            {
                StartDate = DateTime.ParseExact(objRezCentralDTO.StartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            if (lstRateCodes.Count > 0)
            {
                foreach (RezCentralLocationBrandDTO objRezCentralLocationBrandDTO in objRezCentralDTO.Locations)
                {
                    if (!string.IsNullOrEmpty(objRezCentralDTO.RateCodes))
                    {
                        brandId = _locationBrandService.GetById(objRezCentralLocationBrandDTO.LocationBrandId, false).BrandID;
                        rateCodes = GetRateCodesOfSupportedBrand(lstRateCodes, brandId);

                        foreach (var rateCode in objRezCentralDTO.RateCodes.Split(','))
                        {
                            //if (objRezCentralDTO.IsOpenEndedRates || (!string.IsNullOrEmpty(objRezCentralDTO.EndDate) && string.IsNullOrEmpty(objRezCentralDTO.Days2) && objRezCentralDTO.Days1.Split(',').Length == 7))
                            // rate code date range is not applicable to open ended rates due to end is very far and each xml would tax the system.
                            if (objRezCentralDTO.IsOpenEndedRates)
                            {
                                if (objRezCentralDTO.IsOpenEndedRates)
                                {
                                    objRezCentralDTO.EndDate = DateTime.ParseExact(ConfigurationManager.AppSettings["OpenEndedDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMd");
                                }
                                else
                                {
                                    if (objRezCentralDTO.EndDate.IndexOf("/") > 0)
                                    {
                                        objRezCentralDTO.EndDate = DateTime.ParseExact(objRezCentralDTO.EndDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMd");
                                    }
                                }
                                List<TSDModel> lstTSDModels = new List<TSDModel>();

                                if (rateCodes.Split(',').Contains(rateCode))
                                {
                                    FillTsdModels(objRezCentralDTO, StartDate, rateCode, objRezCentralLocationBrandDTO.Location, ref lstTSDModels);
                                }
                                if (lstTSDModels.Count > 0)
                                {
                                    applicableRateCodes.Add(rateCode);
                                    applicableLocations.Add(objRezCentralLocationBrandDTO.LocationBrand);
                                    await ProcessRateSelection(lstTSDModels, objRezCentralDTO.Username, objRezCentralDTO.System, objRezCentralLocationBrandDTO.LocationBrandId, objRezCentralDTO.UserId, 0, false, objRezCentralLocationBrandDTO.LocationBrand.Split('-')[1], false, false);
                                }
                            }
                            else if (!string.IsNullOrEmpty(objRezCentralDTO.EndDate) && (!string.IsNullOrEmpty(objRezCentralDTO.Days1) || !string.IsNullOrEmpty(objRezCentralDTO.Days2)))
                            {
                                EndDate = DateTime.ParseExact(objRezCentralDTO.EndDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                                //Divide date array in the length of 30 days so one xml will contain data for maximum 30 days for 1 rate code and for 1 brandlocation
                                var lstArrivalDates = Enumerable.Range(0, 1 + EndDate.Date.Subtract(StartDate.Date).Days)
                                        .Select(offset => StartDate.AddDays(offset)).Select((x, i) => new { Index = i, Value = x })
                                        .GroupBy(d => d.Index / 30)
                                        .Select(e => e.Select(f => f.Value).ToList())
                                        .ToList();

                                foreach (var arrivalDates in lstArrivalDates)
                                {
                                    List<TSDModel> lstTSDModels = new List<TSDModel>();
                                    //set of 30 days
                                    foreach (DateTime tsdDate in arrivalDates)
                                    {
                                        rateCodes = GetApplicableRateCodes(lstRateCodes, brandId, tsdDate);
                                        if (rateCodes.Split(',').Contains(rateCode))
                                        {
                                            FillTsdModels(objRezCentralDTO, tsdDate, rateCode, objRezCentralLocationBrandDTO.Location, ref lstTSDModels);
                                        }
                                    }
                                    if (lstTSDModels.Count > 0)
                                    {
                                        applicableRateCodes.Add(rateCode);
                                        applicableLocations.Add(objRezCentralLocationBrandDTO.LocationBrand);
                                        await ProcessRateSelection(lstTSDModels, objRezCentralDTO.Username, objRezCentralDTO.System, objRezCentralLocationBrandDTO.LocationBrandId, objRezCentralDTO.UserId, 0, false, objRezCentralLocationBrandDTO.LocationBrand.Split('-')[1], false, false);
                                    }
                                }
                            }
                        }
                    }
                }
                if (applicableLocations.Count > 0 && applicableRateCodes.Count > 0)
                {
                    string response = "Locations: " + string.Join(", ", applicableLocations.Distinct().ToArray()) + " & RateCodes: " + string.Join(", ", applicableRateCodes.Distinct().ToArray());
                    return response;
                }
            }
            return string.Empty;
        }

        void FillTsdModels(RezCentralDTO objRezCentralDTO, DateTime tsdDate, string rateCode, string location, ref List<TSDModel> lstTSDModels)
        {
            TSDModel tsdModel = null;
            foreach (RezCentralRatesDTO rateDTO in objRezCentralDTO.Rates)
            {
                //set 7 if hashcode is 0 as db contains 7 for sunday
                int weekDay = tsdDate.DayOfWeek.GetHashCode();
                weekDay = weekDay == 0 ? 7 : weekDay;
                if (objRezCentralDTO.IsOpenEndedRates || (!string.IsNullOrEmpty(objRezCentralDTO.Days1) && objRezCentralDTO.Days1.Split(',').Any(a => Convert.ToInt32(a) == weekDay)))
                {
                    if (rateDTO.DailyLeftRate > 0)
                    {
                        tsdModel = new TSDModel();
                        tsdModel.RezTSDModel.IsRezCentral = true;
                        tsdModel.RezTSDModel.IsDaily = true;
                        tsdModel.DailyRate = rateDTO.DailyLeftRate;
                        tsdModel.CarClass = rateDTO.CarClass;
                        tsdModel.StartDate = tsdDate.ToString("yyyyMMd");
                        tsdModel.RentalLength = rateCode;
                        tsdModel.Branch = location;

                        if (objRezCentralDTO.IsOpenEndedRates)
                        {
                            tsdModel.RezTSDModel.IsOpenEnded = true;
                            tsdModel.RezTSDModel.EndDate = objRezCentralDTO.EndDate;
                        }
                        //else if (string.IsNullOrEmpty(objRezCentralDTO.Days2) && objRezCentralDTO.Days1.Split(',').Length == 7)
                        //{
                        //    tsdModel.RezTSDModel.IsRateSplitInTwoSections = false;
                        //    tsdModel.RezTSDModel.EndDate = objRezCentralDTO.EndDate;
                        //}
                        else
                        {
                            tsdModel.RezTSDModel.IsRateSplitInTwoSections = true;
                        }

                        lstTSDModels.Add(tsdModel);
                    }
                    if (rateDTO.WeeklyLeftRate > 0)
                    {
                        tsdModel = new TSDModel();
                        tsdModel.RezTSDModel.IsRezCentral = true;
                        tsdModel.RezTSDModel.IsWeekly = true;
                        tsdModel.DailyRate = rateDTO.WeeklyLeftRate;
                        tsdModel.CarClass = rateDTO.CarClass;
                        tsdModel.StartDate = tsdDate.ToString("yyyyMMd");
                        tsdModel.RentalLength = rateCode;
                        tsdModel.Branch = location;

                        if (objRezCentralDTO.IsOpenEndedRates)
                        {
                            tsdModel.RezTSDModel.IsOpenEnded = true;
                            tsdModel.RezTSDModel.EndDate = objRezCentralDTO.EndDate;
                        }
                        //else if (string.IsNullOrEmpty(objRezCentralDTO.Days2) && objRezCentralDTO.Days1.Split(',').Length == 7)
                        //{
                        //    tsdModel.RezTSDModel.IsRateSplitInTwoSections = false;
                        //    tsdModel.RezTSDModel.EndDate = objRezCentralDTO.EndDate;
                        //}
                        else
                        {
                            tsdModel.RezTSDModel.IsRateSplitInTwoSections = true;
                        }

                        lstTSDModels.Add(tsdModel);
                    }

                    if (objRezCentralDTO.IsOpenEndedRates)
                    {
                        continue;
                    }
                }
                else if (!string.IsNullOrEmpty(objRezCentralDTO.Days2) && objRezCentralDTO.Days2.Split(',').Any(a => Convert.ToInt32(a) == weekDay))
                {
                    if (rateDTO.DailyRightRate > 0)
                    {
                        tsdModel = new TSDModel();
                        tsdModel.RezTSDModel.IsRezCentral = true;
                        tsdModel.RezTSDModel.IsDaily = true;
                        tsdModel.DailyRate = rateDTO.DailyRightRate;
                        tsdModel.CarClass = rateDTO.CarClass;
                        tsdModel.StartDate = tsdDate.ToString("yyyyMMd");
                        tsdModel.RentalLength = rateCode;
                        tsdModel.Branch = location;
                        tsdModel.RezTSDModel.IsRateSplitInTwoSections = true;

                        lstTSDModels.Add(tsdModel);
                    }
                    if (rateDTO.WeeklyRightRate > 0)
                    {
                        tsdModel = new TSDModel();
                        tsdModel.RezTSDModel.IsRezCentral = true;
                        tsdModel.RezTSDModel.IsWeekly = true;
                        tsdModel.DailyRate = rateDTO.WeeklyRightRate;
                        tsdModel.CarClass = rateDTO.CarClass;
                        tsdModel.StartDate = tsdDate.ToString("yyyyMMd");
                        tsdModel.RentalLength = rateCode;
                        tsdModel.Branch = location;
                        tsdModel.RezTSDModel.IsRateSplitInTwoSections = true;

                        lstTSDModels.Add(tsdModel);
                    }
                }
            }
        }

        void SetRateParameters(RateDetail rates, TSDModel tsdModel, string rentalLength)
        {
            decimal dailyFree = Convert.ToDecimal(ConfigurationManager.AppSettings["DailyFree"]);
            decimal weeklyFree = Convert.ToDecimal(ConfigurationManager.AppSettings["WeeklyFree"]);
            rates.EndDate = DateTime.ParseExact(tsdModel.StartDate, "yyyyMMd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
            if (tsdModel.RezTSDModel.IsRezCentral)
            {
                rates.RateCode = rentalLength;
                if (tsdModel.RezTSDModel.IsOpenEnded)
                {
                    rates.EndDate = DateTime.ParseExact(tsdModel.RezTSDModel.EndDate, "yyyyMMd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                }

                if (tsdModel.RezTSDModel.IsDaily)
                {
                    rates.DailyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.DailyFree = dailyFree;
                    rates.RatePlan = 10;
                    rates.ExtraDayRate = rates.DailyRate.Value;
                }
                else if (tsdModel.RezTSDModel.IsWeekly)
                {
                    decimal weeklyExtraDayFactor = Convert.ToDecimal(ConfigurationManager.AppSettings["RezWeeklyExtraDayFactor"]);
                    rates.WeeklyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.WeeklyFree = weeklyFree;
                    rates.RatePlan = 30;
                    rates.ExtraDayRate = Math.Round(tsdModel.DailyRate /
                         weeklyExtraDayFactor, 2);
                }
            }
            else if (tsdModel.IsOpaqueRates)
            {
                rates.RateCode = rentalLength;

                if (tsdModel.OpaqueTSDModel.IsDaily)
                {
                    rates.DailyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.DailyFree = dailyFree;
                    rates.RatePlan = 10;
                    rates.ExtraDayRate = rates.DailyRate.Value;
                }
                else if (tsdModel.OpaqueTSDModel.IsWeekly)
                {
                    decimal weeklyExtraDayFactor = Convert.ToDecimal(ConfigurationManager.AppSettings["RezWeeklyExtraDayFactor"]);
                    rates.WeeklyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.WeeklyFree = weeklyFree;
                    rates.RatePlan = 30;
                    rates.ExtraDayRate = Math.Round(tsdModel.DailyRate /
                         weeklyExtraDayFactor, 2);
                }
            }
            else
            {
                if (rentalLength.IndexOf("D") >= 0)
                {
                    rates.DailyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.DailyFree = dailyFree;
                    rates.RatePlan = 10;
                    rates.ExtraDayRate = Math.Round((tsdModel.DailyRate) *
                        tsdModel.ExtraDayRateFactor, 2);

                    //IsDailyUpdate = true;
                }
                else if (rentalLength.IndexOf("W") >= 0)
                {
                    rates.WeeklyRate = Math.Round(tsdModel.DailyRate, 2);
                    rates.WeeklyFree = weeklyFree;
                    rates.RatePlan = 30;
                    rates.ExtraDayRate = Math.Round((tsdModel.DailyRate) /
                         tsdModel.ExtraDayRateFactor, 2);
                    //IsDailyUpdate = false;
                }
                if (rentalLength == "W8" || rentalLength == "W9" || rentalLength == "W10" || rentalLength == "W11")
                {
                    rates.DailyRate = Math.Round((tsdModel.DailyRate) / 7, 2);
                }
            }
        }
        public async Task<IEnumerable<RezPreloadRateDTO>> GetRatesFromPreloadAPI(RezPreloadPayloadDTO objPayload)
        {
            DateTime date = DateTime.Parse(objPayload.RequestDate);
            ProviderRequest request = CreateRequestToFetchRates(objPayload.RateSource, objPayload.LocationCode, objPayload.RateCode, date.ToString("MM-dd-yyyy"));
            IEnumerable<RezPreloadRateDTO> rates = await FetchRates(request);
            return rates;
        }

        public ProviderRequest CreateRequestToFetchRates(string RateSource, string LocationCode, string RateCode, string RequestDate)
        {
            ProviderRequest request = new ProviderRequest();
            request.URL = ConfigurationManager.AppSettings["PreloadService"];
            request.HttpMethod = Enumerations.HttpMethod.Post;
            RezPreloadPayloadDTO payload = new RezPreloadPayloadDTO();
            payload.RateSource = RateSource;
            payload.LocationCode = LocationCode;
            payload.RateCode = RateCode;
            payload.RequestDate = RequestDate;
            request.ProviderRequestData = JsonConvert.SerializeObject(payload);
            return request;
        }
        public async Task<IEnumerable<RezPreloadRateDTO>> FetchRates(ProviderRequest request)
        {
            List<RezPreloadRateDTO> tsdPreloadRates = new List<RezPreloadRateDTO>();
            try
            {
                var httpWebRequest = WebRequest.Create(request.URL);
                httpWebRequest.ContentType = "application/json";

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(request.ProviderRequestData);
                Stream objRequestStream = null;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                objRequestStream = httpWebRequest.GetRequestStream();
                objRequestStream.Write(bytes, 0, bytes.Length);
                objRequestStream.Close();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                WebResponse response = await httpWebRequest.GetResponseAsync();

                HttpWebResponse httpResponse = (HttpWebResponse)response;
                using (Stream data = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(data))
                    {
                        string responseFromServer = reader.ReadToEnd();
                        dynamic jsonObject = serializer.Deserialize<dynamic>(responseFromServer);
                        var keys = jsonObject.Keys as Dictionary<string, object>.KeyCollection;
                        if (keys.FirstOrDefault().ToString().Equals(ConfigurationManager.AppSettings["JSONKey"], StringComparison.OrdinalIgnoreCase))
                        {
                            var tsdRates = jsonObject[ConfigurationManager.AppSettings["JSONKey"]];
                            foreach (var result in tsdRates)
                            {
                                RezPreloadRateDTO tsdRate = new RezPreloadRateDTO();
                                tsdRate.CarClass = result["CarClass"];
                                tsdRate.DailyRate = Convert.ToDecimal(result["DailyRate"]);
                                tsdRate.WeeklyRate = Convert.ToDecimal(result["WeeklyRate"]);
                                tsdRate.MonthlyRate = Convert.ToDecimal(result["MonthlyRate"]);
                                tsdPreloadRates.Add(tsdRate);
                            }
                        }
                    }
                }

            }
            catch (WebException e)
            {
                LogHelper.WriteToLogFile("FetchRates => " + ", Web Exception Occured,Base Exception: " + e.GetBaseException().ToString()
                     + ", Stack Trace: " + e.StackTrace + " exception Message " + e.Message, LogHelper.GetLogFilePath());
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("FetchRates => " + ", Exception Occured,Base Exception: " + ex.GetBaseException().ToString()
                     + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, LogHelper.GetLogFilePath());
            }
            return tsdPreloadRates;
        }

        public string GetApplicableRateCodes(List<RateCodeDTO> rateCodesWithDateRange, long brandId, DateTime date)
        {
            IEnumerable<RateCodeDTO> applicableRateCodes = rateCodesWithDateRange.Where(d => d.DateRangeList.Any(e => date >= e.StartDate && date <= e.EndDate))
                .Select(d => new RateCodeDTO { Code = d.Code, SupportedBrandIDs = d.SupportedBrandIDs }).Distinct();

            return GetRateCodesOfSupportedBrand(applicableRateCodes, brandId);
        }

        public string GetRateCodesOfSupportedBrand(IEnumerable<RateCodeDTO> rateCodesWithDateRange, long brandId)
        {
            if (rateCodesWithDateRange.Count() > 0)
            {
                return string.Join(",", rateCodesWithDateRange.Where(d => d.SupportedBrandIDs.Split(',').Contains(brandId.ToString())).Select(d => d.Code));
            }
            return string.Empty;

        }
    }
}
