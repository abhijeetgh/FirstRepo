using Newtonsoft.Json.Linq;
using RateShopper.Domain.DTOs;
using RateShopper.Providers.Interface;
using RateShopper.Providers.Model;
using RateShopper.Providers.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers
{
    public class ProviderService : IProviderService
    {
        IRateHighway iRateHighway;
        IScrappingService iScrappingService;
        public ProviderService(IRateHighway _iRateHighway, IScrappingService _iScrappingService)
        {
            iRateHighway = _iRateHighway;
            iScrappingService = _iScrappingService;
        }

        public async Task<string> SendRequestForSearchedShop(SearchModelDTO searchModel)
        {
            string response = string.Empty;
            switch (searchModel.SelectedAPI)
            {    
                default:
                case "SS":
                    response = await iScrappingService.SendRequest(searchModel);
                    break;
                case "RH":                    
                    response = await iRateHighway.SendRequest(searchModel);
                    break;
                case "TC":
                    ITravelClick iTravelClick = new TravelClick();
                    response = await iTravelClick.SendRequest(searchModel);
                    break;
            }
            return response;
        }

        public void ParseResponse(string providerCode, string responseJSON, long searchSummaryId)
        {
            switch (providerCode)
            {
                default:
                case "SS":
                    //response = iScrappingService.SendRequest(searchModel);
                    break;
                case "RH":
                    iRateHighway.ParseResponse(responseJSON, searchSummaryId);
                    break;                
            }
        }
    }
}
