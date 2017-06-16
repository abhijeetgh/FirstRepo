using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Interface
{
    public interface IProviderService
    {
        Task<string> SendRequestForSearchedShop(SearchModelDTO objSearchModelDTO);

        void ParseResponse(string providerCode, string responseJSON, long searchSummaryId);
    }
}
