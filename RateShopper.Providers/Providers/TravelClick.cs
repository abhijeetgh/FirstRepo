using RateShopper.Providers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Providers
{
    class TravelClick : ITravelClick
    {
        public Task<string> SendRequest(Domain.DTOs.SearchModelDTO objSearchModelDTO)
        {
            throw new NotImplementedException();
        }


        public void ParseResponse(string responseString, long searchSummaryId, long shopRequestId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
