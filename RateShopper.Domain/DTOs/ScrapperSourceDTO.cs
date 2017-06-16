using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ScrapperSourceDTO
    {
        public ScrapperSourceDTO()
        {
            SearchResults = new List<SearchResult>();
            UserScrapperSources = new List<UserScrapperSources>();
        }
        public long ID { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }        
        public string Code { get; set; }        
        public long ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public List<SearchResult> SearchResults { get; set; }
        public List<UserScrapperSources> UserScrapperSources { get; set; }
        public bool IsGov { get; set; }
    }
}
