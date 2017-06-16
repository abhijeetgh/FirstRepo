using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FTBRatesDTO
    {
        public FTBRatesDTO()
        {
            DicFTBRateDetails = new Dictionary<string, List<FTBRateDetailsDTO>>();
            //FTBRateDetailsWithOtherSplitIndex = new Dictionary<int, List<FTBRateDetailsDTO>>();
        }

        public long ID { get; set; }
        public long LocationBrandId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsSplitMonth { get; set; }
        public bool HasBlackOutPeroid { get; set; }
        public bool OldHasBlackOutPeroid { get; set; }
        public DateTime? BlackOutStartDate { get; set; }
        public DateTime? BlackOutEndDate { get; set; }
        public DateTime? OldBlackOutStartDate { get; set; }
        public DateTime? OldBlackOutEndDate { get; set; }
        public Dictionary<string, List<FTBRateDetailsDTO>> DicFTBRateDetails { get; set; }
        public List<SplitMonthDetailsDTO> SplitLabels { get; set; }
        public List<FTBCopyMonthsDTO> LstCopyMonths { get; set; }
        public string SelectedLORs { get; set; }
        public long LowestRentalLengthId { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public List<RentalLengthDTO> rentalLengths { get; set; }
    }
    public class FTBCustomRateDTO
    {      
        public string Message { get; set; }
        public FTBRatesDTO ftbRatesDTO { get; set; }
    }
    public class FTBRateDetailsDTO
    {
        public long FTBRatesId { get; set; }
        public long FTBRateDetailsId { get; set; }
        public long RentalLengthId { get; set; }
        public long CarClassId { get; set; }
        public string CarClass { get; set; }
        public decimal? Sunday { get; set; }
        public decimal? Monday { get; set; }
        public decimal? Tuesday { get; set; }
        public decimal? Wednesday { get; set; }
        public decimal? Thursday { get; set; }
        public decimal? Friday { get; set; }
        public decimal? Saturday { get; set; }
        public int SplitIndex { get; set; }
        public bool? IsSplitMonth { get; set; }
        public bool? HasBlackOutPeroid { get; set; }
        public DateTime? BlackOutStartDate { get; set; }
        public DateTime? BlackOutEndDate { get; set; }
        public string RentalLengthCode { get; set; }
    }

    public class FTBCopyMonthsDTO : FTBRatesDTO
    {
        public FTBCopyMonthsDTO()
        {
            SplitLabels = new List<SplitMonthDetailsDTO>();
        }
        public long FTBRatesId { get; set; }
        public string MonthWithYear { get; set; }
        public int SourceYear { get; set; }
        public int SourceMonth { get; set; }
        public int SourceSplitIndex { get; set; }
        public int DestinationSplitIndex { get; set; }
        public string[] Labels { get; set; }
        public List<SplitMonthDetailsDTO> SplitLabels { get; set; }
    }


}
