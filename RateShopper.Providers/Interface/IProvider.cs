using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Interface
{
    public interface IProvider
    {
        Task<string> SendRequest(SearchModelDTO objSearchModelDTO);

        void ParseResponse(string responseString, long searchSummaryId, long shopRequestId = 0);
    }
}
