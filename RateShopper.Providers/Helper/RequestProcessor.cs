using RateShopper.Domain.DTOs;
using RateShopper.Providers.Interface;
using RateShopper.Providers.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Helper
{
    public class RequestProcessor : IRequestProcessor
    {
        public async Task<ResponseModel> SendAsync(ProviderRequest objProviderRequest)
        {
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
