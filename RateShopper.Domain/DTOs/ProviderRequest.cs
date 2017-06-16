using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ProviderRequest
    {
        
        public string ProviderRequestData { get; set; }    

        public bool IsBasicAuthentionRequired { get; set; }

        public Dictionary<string,string> HeaderItems { get; set; }

        public string URL { get; set; }
                
        public Credentials BasicAuthenticationCredentials { get; set; }

        public Enumerations.HttpMethod HttpMethod { get; set; }

        public int RetryCount { get; set; }

        public ProviderRequest()
        {
            BasicAuthenticationCredentials = new Credentials();
            HeaderItems = new Dictionary<string, string>();
        }
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
