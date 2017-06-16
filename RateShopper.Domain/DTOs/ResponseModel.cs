using System.Net;

namespace RateShopper.Domain.DTOs
{    
    public class ResponseModel
    {
        private readonly string _result;
        private readonly HttpStatusCode _statusCode;
        private readonly string _statusDescription;

        public ResponseModel(string result, HttpStatusCode statusCode, string statusDescription)
        {
            _result = result;
            _statusCode = statusCode;
            _statusDescription = statusDescription;
        }

        public string Result
        {
            get { return _result; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        public string StatusDescription
        {
            get { return _statusDescription; }
        }
    }
}
