using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RateShopper.Services.Data
{
    public class PostXMLLogsService : BaseService<PostXMLLogs>, IPostXMLLogsService
    {
        public PostXMLLogsService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<PostXMLLogs>();
            _cacheManager = cacheManager;
        }

        public async Task<string> PostXML()
        {   
            int xmlRowCount = Convert.ToInt32(ConfigurationManager.AppSettings["XMLRowCount"]);
            long? prevHighestTSDId = _context.PostXMLLogs.OrderByDescending(d => d.CreatedDateTime).Select(d => d.HighestTSDId).FirstOrDefault();
            int iterations = 0;
            if (prevHighestTSDId.HasValue && prevHighestTSDId.Value > 0)
            {
                List<TSDTransaction> lstTransactions = _context.TSDTransactions.Where(d => d.ID > prevHighestTSDId).ToList();
                
                string xmlData = string.Empty;
                int rowCount = xmlRowCount;
                int totalRows = lstTransactions.Count;
                while (lstTransactions.Count > 0)
                {
                    iterations++;
                    //Check if records present in database to push are more than XMLRowCount than send XMLRowCount else select all records which are yet to be pushed
                    rowCount = lstTransactions.Count > xmlRowCount ? xmlRowCount : lstTransactions.Count;
                    //xmlData = jsonSerializer.Serialize(lstTransactions.Take(rowCount).Select(d=>d.XMLRequest).ToArray());
                    xmlData = Newtonsoft.Json.JsonConvert.SerializeObject(lstTransactions.Take(rowCount).Select(d => d.XMLRequest).ToArray());
                    
                    //Push xml data 
                    await PushXML(xmlData, lstTransactions.ElementAt(rowCount - 1).ID, string.Join(",", lstTransactions.Take(rowCount).Select(d => d.SearchSummaryID)));

                    lstTransactions.RemoveRange(0, rowCount);
                }
                return iterations.ToString() + " iterations for records- " + totalRows.ToString();
            }
            else
            {
                //if no rows present in PostXMLLogs then get highest TSD id and insert it
                prevHighestTSDId = _context.TSDTransactions.Max(d => d.ID);
                PostXMLLogs postXMLLogsEntity = new PostXMLLogs();
                postXMLLogsEntity.SearchSummaryIds = string.Empty;
                postXMLLogsEntity.HighestTSDId = prevHighestTSDId.Value;
                postXMLLogsEntity.CreatedDateTime = DateTime.Now;

                Add(postXMLLogsEntity);
                return PostXML().Result;
            }
        }
        
        private async Task<string> PushXML(string jsonXmlDataArray, long highestTSDID, string searchSummaryIds)
        {            
            ProviderRequest objProviderRequest = new ProviderRequest();
            objProviderRequest.URL = ConfigurationManager.AppSettings["PostURL"];
            objProviderRequest.ProviderRequestData = jsonXmlDataArray;
            objProviderRequest.HttpMethod = Enumerations.HttpMethod.Post;
            objProviderRequest.RetryCount = 0;
            WriteToLogFile("Pushing XML And Highest TSD ID ->" + highestTSDID.ToString(), GetLogFilePath());
            ResponseModel objResponseModel = await SendAsync(objProviderRequest);
            WriteToLogFile("Response from end point ->" + objResponseModel.Result + "-" + objResponseModel.StatusDescription, GetLogFilePath());
            PostXMLLogs postXMLLogsEntity = new PostXMLLogs();
            postXMLLogsEntity.SearchSummaryIds = searchSummaryIds;
            postXMLLogsEntity.Response = objResponseModel.Result + "-" + objResponseModel.StatusDescription;
            postXMLLogsEntity.HighestTSDId = highestTSDID;
            postXMLLogsEntity.CreatedDateTime = DateTime.Now;

            Add(postXMLLogsEntity);
            WriteToLogFile("Response added to database ->", GetLogFilePath());
            return "success";
        }

        private async Task<ResponseModel> SendAsync(ProviderRequest objProviderRequest)
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
                WriteToLogFile("Exeption occurred ->" + e.GetBaseException(), GetLogFilePath());
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
                WriteToLogFile("Exeption occurred ->" + ex.GetBaseException(), GetLogFilePath());
                objResponseModel = new ResponseModel(null, HttpStatusCode.SeeOther, ex.Message);
            }

            //if request failed then retry for 2 times
            if (objResponseModel.StatusCode != HttpStatusCode.OK & objProviderRequest.RetryCount < 2)
            {
                WriteToLogFile("Retry  ->" + objProviderRequest.RetryCount.ToString(), GetLogFilePath());
                objProviderRequest.RetryCount++;
                objResponseModel = await SendAsync(objProviderRequest);
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

        static void WriteToLogFile(string strMessage, string outputFile)
        {

            try
            {
                string line = DateTime.Now.ToString() + " | ";

                line += strMessage;

                FileStream fs = new FileStream(outputFile, FileMode.Append, FileAccess.Write, FileShare.None);

                StreamWriter swFromFileStream = new StreamWriter(fs);

                swFromFileStream.WriteLine(line);

                swFromFileStream.Flush();

                swFromFileStream.Close();

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private static string GetLogFilePath()
        {
            try
            {
                // get the base directory

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // search the file below the current directory
                string retFilePath = baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\" + "LogFile-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                // Console.WriteLine("Relative file path " + ConfigurationManager.AppSettings["LogFilePath"]);
                // if exists, return the path
                if (File.Exists(retFilePath) == true)
                    return retFilePath;
                //create a text file
                else
                {
                    if (CheckDirectory(baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\") == false)
                        return string.Empty;

                    FileStream fs = new FileStream(retFilePath,
                          FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }

                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                string strDirectoryname =
                           strLogPath.Trim().Substring(0, nFindSlashPos);

                if (false == Directory.Exists(strDirectoryname))
                    Directory.CreateDirectory(strDirectoryname);
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }    
    }
}
