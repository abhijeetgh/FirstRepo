using RateShopper.Providers.Interface;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Providers
{
    public class ScrappingService : IScrappingService
    {
        ISearchSummaryService iSearchSummary;

        public ScrappingService(ISearchSummaryService _iSearchSummary)
        {
            this.iSearchSummary = _iSearchSummary;
        }

        public async Task<string> SendRequest(Domain.DTOs.SearchModelDTO objSearchModelDTO)
        {
            if (objSearchModelDTO.ScrapperSource == "CPR")
            {
                objSearchModelDTO.VendorCodes = string.Join(",", objSearchModelDTO.VendorCodes.Split(',').Where(d => d != "ZE" && d != "ZI"));
            }
            return await iSearchSummary.SaveSearchSummary(objSearchModelDTO);
        }


        public void ParseResponse(string responseString, long searchSummaryId, long shopRequestId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
