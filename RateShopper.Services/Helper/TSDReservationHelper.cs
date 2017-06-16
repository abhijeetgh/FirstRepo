using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RateShopper.Services.Helper
{
    public class TSDReservationHelper
    {

        internal static async Task<IEnumerable<TSDReservationDTO>> fetchTSDReservationCount(ProviderRequest objProviderReq)
        {
            IEnumerable<TSDReservationDTO> tsdReservations = null;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(objProviderReq.URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = (1000 * 60) * 10;
                if (objProviderReq.HeaderItems.Count > 0)
                {
                    AddRequestHeaders(httpWebRequest, objProviderReq.HeaderItems);
                }
                if (objProviderReq.HttpMethod == Enumerations.HttpMethod.Post)
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(objProviderReq.ProviderRequestData);
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

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                using (WebResponse response = await httpWebRequest.GetResponseAsync())
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            dynamic jsonObject = serializer.Deserialize<dynamic>(reader.ReadToEnd());
                            var keys = jsonObject.Keys as Dictionary<string, object>.KeyCollection;
                            if (keys.FirstOrDefault().ToString().Equals(ConfigurationManager.AppSettings["JSONKey"], StringComparison.OrdinalIgnoreCase))
                            {
                                List<TSDReservationDTO> tsdList = new List<TSDReservationDTO>();
                                dynamic tsdReservationResult =  serializer.Deserialize<dynamic>(jsonObject[ConfigurationManager.AppSettings["JSONKey"]]);
                                foreach (var result in tsdReservationResult)
                                {
                                    TSDReservationDTO tsdReservation = new TSDReservationDTO();
                                    tsdReservation.DayInMonth = DateTime.ParseExact(result["DayInMonth"], "M/d/yyyy", CultureInfo.InvariantCulture);
                                    tsdReservation.ReservationCount = Convert.ToInt64(result["ReservationCount"]);
                                    tsdList.Add(tsdReservation);
                                }
                                LogHelper.WriteToLogFile("fetchTSDReservationCount => " + tsdList, LogHelper.GetLogFilePath());
                                tsdReservations = tsdList.AsEnumerable<TSDReservationDTO>();

                            }
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
                        LogHelper.WriteToLogFile("fetchTSDReservationCount => " + ",Web Exception Occured,Base Exception: " + e.GetBaseException().ToString()
                     + ", Stack Trace: " + e.StackTrace + " exception Message " + e.Message, LogHelper.GetLogFilePath());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("fetchTSDReservationCount => " + ",  Exception Occured,Base Exception: " + ex.GetBaseException().ToString()
                     + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, LogHelper.GetLogFilePath());
            }

            return tsdReservations;
        }

        private static void AddRequestHeaders(HttpWebRequest request, Dictionary<string, string> headerItems)
        {
            foreach (KeyValuePair<string, string> item in headerItems)
            {
                request.Headers.Add(item.Key, item.Value);
            }
        }
    }
}
