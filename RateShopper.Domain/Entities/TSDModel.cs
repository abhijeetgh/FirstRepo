using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class TSDModel
    {
        public TSDModel()
        {
            RezTSDModel = new RezTSDModel();
            OpaqueTSDModel = new OpaqueTSDModel();
        }
        public string RemoteID { get; set; }
        public string Branch { get; set; }
        public string CarClass { get; set; }
        public string RentalLength { get; set; }
        public string RentalLengthIDs { get; set; }
        public string StartDate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal ExtraDayRateFactor { get; set; }
        public decimal ExtraDayRateValue { get; set; }
        public int RentalLengthCount { get; set; }
        public long SuggestedRateId { get; set; }
        public decimal TotalRate { get; set; }
        public RezTSDModel RezTSDModel { get; set; }
        public OpaqueTSDModel OpaqueTSDModel { get; set; }
        public bool IsOpaqueRates { get; set; }
    }

    public class RezTSDModel
    {
        public bool IsRezCentral { get; set; }
        public bool IsDaily { get; set; }
        public bool IsWeekly { get; set; }
        public string EndDate { get; set; }
        public bool IsOpenEnded { get; set; }
        public bool IsRateSplitInTwoSections { get; set; }
    }

    public class OpaqueTSDModel
    {        
        public bool IsDaily { get; set; }
        public bool IsWeekly { get; set; }
    }

    public class OpaqueRatesDTO
    {        
        public long CarClassId { get; set; }
        public decimal PercentValue { get; set; }
        public string CarCode { get; set; }
        public string Date { get; set; }
    }

    public class OpaqueRatesConfiguration
    {
        public string RateCodes { get; set; }
        public bool IsDailyView { get; set; }
        public bool IsClassicView { get; set; }
        public List<OpaqueRatesDTO> OpaqueRates { get; set; }
    }

    public class TSDPostRatesDTO
    {
        public string TSDXML { get; set; }
        public long LocationBrandID { get; set; }
        public long UserId { get; set; }
        public long SearchSummaryID { get; set; }
        public bool IsTetheredUpdate { get; set; }
        public string BrandLocation { get; set; }
        public int RetryCount { get; set; }
    }
}
