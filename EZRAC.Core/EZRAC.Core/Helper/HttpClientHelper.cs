using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EZRAC.Core.Helper
{
    public static class HttpClientHelper
    {
        #region AsyncMethods
        /// <summary>
        /// Get data from API 
        /// </summary>
        /// <param name="restServiceUri"></param>
        /// <param name="actionMethod"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetDataAsync(string restServiceUri, string actionMethod)
        {
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
            {
                client.BaseAddress = new Uri(restServiceUri); // getting the url for base address
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.GetAsync(actionMethod);
            }
            return response;
        }

        /// <summary>
        /// Post data to API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restServiceUri"></param>
        /// <param name="actionMethod"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostDataAsync<T>(string restServiceUri, string actionMethod, T inputData)
        {
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
            {
                client.BaseAddress = new Uri(restServiceUri);
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.PostAsJsonAsync(actionMethod, inputData);
            }
            return response;
        }

        /// <summary>
        /// Get data from API
        /// </summary>
        /// <param name="restServiceUri"></param>
        /// <param name="actionMethod"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetData(string restServiceUri, string actionMethod)
        {
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
            {
                client.BaseAddress = new Uri(restServiceUri); // getting the url for base address
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.GetAsync(actionMethod).Result;
            }
            return response;
        }
        #endregion
    }
}
