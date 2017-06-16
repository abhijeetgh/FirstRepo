using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class LocationDTO
    {
        public LocationDTO()
        {
            RentalLengths = new List<long>();
            CarClasses = new List<long>();
        }

        public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string LocationBrandAlias { get; set; }
        public long BrandID { get; set; }
        public decimal? WeeklyExtraDenominator { get; set; }
        public decimal? DailyExtraDayFactor { get; set; }
        public string TSDCustomerNumber { get; set; }
        public string TSDPassCode { get; set; }
        public bool UseLORRateCode { get; set; }
        public long CreatedBy { get; set; }
        public List<long> RentalLengths { get; set; }
        public List<long> CarClasses { get; set; }
        public string BranchCode { get; set; }
        public string DeletedCompetitorCompanyIds { get; set; } //use for hard delete selected company ID from RuleSetGroupCompany
        public string CompetitorCompanyIds { get; set; }
        public string QuickViewCompetitors { get; set; }
        public string LocationPricingManagerName { get; set; }
        public bool ApplyDependantBrandLOR { get; set; }
    }
}
