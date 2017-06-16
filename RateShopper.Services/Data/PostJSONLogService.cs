using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.IO;
using System.Data.Entity;
using RateShopper.Services.Helper;
using System.Net;
using System.Configuration;

namespace RateShopper.Services.Data
{
    public class PostJSONLogService : BaseService<PostJSONRequestLog>, IPostJSONLogService
    {
        public PostJSONLogService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<PostJSONRequestLog>();
            _cacheManager = cacheManager;
        }
        public async void SavePushJSONSummary(List<PushJSONRequestDTO> pushJSONRequestDTO, long searchSummaryID)
        {
            PostJSONRequestLog postJSONRequestLogs = new PostJSONRequestLog();
            postJSONRequestLogs.SearchSummaryID = searchSummaryID;
            postJSONRequestLogs.JSONRequest = Newtonsoft.Json.JsonConvert.SerializeObject(pushJSONRequestDTO.ToArray());
            postJSONRequestLogs.IsDataSent = false;
            postJSONRequestLogs.CreatedDateTime = DateTime.Now;

            Add(postJSONRequestLogs);
        }

        public async Task<string> PostJSON()
        {
            int jsonRowCount = Convert.ToInt32(ConfigurationManager.AppSettings["JSONRowCount"]);
            // long? prevHighestTSDId = _context.PostXMLLogs.OrderByDescending(d => d.CreatedDateTime).Select(d => d.HighestTSDId).FirstOrDefault();
            int iterations = 0;
            //if (prevHighestTSDId.HasValue && prevHighestTSDId.Value > 0)
            //{
            List<PostJSONRequestLog> lstPostJSONRequestLog = _context.PostJSONRequestLogs.Where(d => !(d.IsDataSent)).ToList();

            string jsonData = string.Empty;
            int rowCount = jsonRowCount;
            int totalRows = lstPostJSONRequestLog.Count;
            while (lstPostJSONRequestLog.Count > 0)
            {
                iterations++;
                //Check if records present in database to push are more than XMLRowCount than send XMLRowCount else select all records which are yet to be pushed
                rowCount = lstPostJSONRequestLog.Count > jsonRowCount ? jsonRowCount : lstPostJSONRequestLog.Count;
                List<PostJSONRequestLog> GetRowCountJSONData = lstPostJSONRequestLog.Where(d => !(d.IsDataSent)).Take(rowCount).ToList();
                //jsonData = string.Join(",", GetRowCountJSONData.Select(d => d.JSONRequest).ToArray());

                string[] JsonData1=GetRowCountJSONData.Select(obj=>obj.JSONRequest.TrimEnd(']').TrimStart('[')).ToArray();
                jsonData = string.Join(",", JsonData1);
                jsonData = "[" + jsonData + "]";
                IEnumerable<PushJSONRequestDTO> obj1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PushJSONRequestDTO>>(jsonData);

                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(obj1);

                //Push JSON data 
                await PushJSON(jsonData, string.Join(",", lstPostJSONRequestLog.Take(rowCount).Select(d => d.SearchSummaryID)), GetRowCountJSONData);

                lstPostJSONRequestLog.RemoveRange(0, rowCount);
            }
            return iterations.ToString() + " iterations for records- " + totalRows.ToString();
            //}
            //else
            //{
            //if no rows present in PostXMLLogs then get highest TSD id and insert it
            //prevHighestTSDId = _context.TSDTransactions.Max(d => d.ID);
            //PostXMLLogs postXMLLogsEntity = new PostXMLLogs();
            //postXMLLogsEntity.SearchSummaryIds = string.Empty;
            //postXMLLogsEntity.HighestTSDId = prevHighestTSDId.Value;
            //postXMLLogsEntity.CreatedDateTime = DateTime.Now;

            //Add(postXMLLogsEntity);
            //  return PostJSON().Result;
            //}
        }

        private async Task<string> PushJSON(string jsonXmlDataArray, string searchSummaryIds, List<PostJSONRequestLog> postJSONRequestLog)
        {
            string LogFileName = ConfigurationManager.AppSettings["PushJSONLogFileName"];
            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.URL = ConfigurationManager.AppSettings["PostJSONURL"];
            objProviderRequest.ProviderRequestData = jsonXmlDataArray;
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.RetryCount = 0;
            //LogHelper.WriteToLogFile("Pushing XML And Highest TSD ID ->" + highestTSDID.ToString(), LogHelper.GetLogFilePath(LogFileName));
            ResponseModel objResponseModel = await SendAsync(objProviderRequest, LogFileName);
            LogHelper.WriteToLogFile("Response from end point ->" + objResponseModel.Result + "-" + objResponseModel.StatusDescription, LogHelper.GetLogFilePath(LogFileName));
            PostJSONResponseLog postJSONResponseLog = new PostJSONResponseLog();
            postJSONResponseLog.SearchSummaryIDs = searchSummaryIds;
            postJSONResponseLog.Response = objResponseModel.Result;
            postJSONResponseLog.CreatedDateTime = DateTime.Now;
            _context.PostJSONResponseLogs.Add(postJSONResponseLog);
            _context.SaveChanges();
            long[] lstSearchSummaryIds = Common.StringToLongList(searchSummaryIds).ToArray();
            foreach (var jsonRequest in postJSONRequestLog)
            {
                PostJSONRequestLog jsonRequestLog = new PostJSONRequestLog();
                jsonRequestLog = jsonRequest;
                jsonRequestLog.IsDataSent = true;
                Update(jsonRequest);
            }


            //PostXMLLogs postXMLLogsEntity = new PostXMLLogs();
            //postXMLLogsEntity.SearchSummaryIds = searchSummaryIds;
            //postXMLLogsEntity.Response = objResponseModel.Result + "-" + objResponseModel.StatusDescription;
            //postXMLLogsEntity.HighestTSDId = highestTSDID;
            //postXMLLogsEntity.CreatedDateTime = DateTime.Now;

            //Add(postXMLLogsEntity);
            LogHelper.WriteToLogFile("Response added to database ->", LogHelper.GetLogFilePath());
            return "success";
        }

        public async Task<ResponseModel> SendAsync(ProviderRequest objProviderRequest, string LogFileName)
        {
            ResponseModel objResponseModel = null;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(objProviderRequest.URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = (1000 * 60) * 10;
                if (objProviderRequest.IsBasicAuthentionRequired)
                {
                    AddBasicAuthorizationHeader(httpWebRequest, objProviderRequest.BasicAuthenticationCredentials);
                }
                if (objProviderRequest.HeaderItems.Count > 0)
                {
                    AddRequestHeaders(httpWebRequest, objProviderRequest.HeaderItems);
                }
                if (objProviderRequest.HttpMethod == Enumerations.HttpMethod.Post)
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(objProviderRequest.ProviderRequestData);
                    Stream objRequestStream = null;
                    httpWebRequest.Method = "POST";
                    //httpWebRequest.ContentType = "application/json";
                    httpWebRequest.ContentLength = bytes.Length;
                    objRequestStream = httpWebRequest.GetRequestStream();
                    objRequestStream.Write(bytes, 0, bytes.Length);
                    objRequestStream.Close();
                }
                else
                {
                    httpWebRequest.Method = "GET";
                }

                using (WebResponse response = await httpWebRequest.GetResponseAsync())
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            objResponseModel = new ResponseModel(reader.ReadToEnd(), httpResponse.StatusCode, httpResponse.StatusDescription);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                LogHelper.WriteToLogFile("Exeption occurred ->" + e.GetBaseException(), LogHelper.GetLogFilePath(LogFileName));
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        objResponseModel = new ResponseModel(reader.ReadToEnd(), httpResponse.StatusCode, httpResponse.StatusDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Exeption occurred ->" + ex.GetBaseException(), LogHelper.GetLogFilePath(LogFileName));
                objResponseModel = new ResponseModel(null, HttpStatusCode.SeeOther, ex.Message);
            }

            //if request failed then retry for 2 times
            if (objResponseModel.StatusCode != HttpStatusCode.OK & objProviderRequest.RetryCount < 2)
            {
                LogHelper.WriteToLogFile("Retry  ->" + objProviderRequest.RetryCount.ToString(), LogHelper.GetLogFilePath(LogFileName));
                objProviderRequest.RetryCount++;
                objResponseModel = await SendAsync(objProviderRequest, LogFileName);
            }
            return objResponseModel;
        }

        private void AddBasicAuthorizationHeader(HttpWebRequest request, Credentials objCredentials)
        {
            request.PreAuthenticate = true;
            byte[] authBytes = Encoding.UTF8.GetBytes(objCredentials.Username + ":" + objCredentials.Password);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(authBytes));
        }

        private void AddRequestHeaders(HttpWebRequest request, Dictionary<string, string> headerItems)
        {
            foreach (KeyValuePair<string, string> item in headerItems)
            {
                request.Headers.Add(item.Key, item.Value);
            }
        }

    }
}
