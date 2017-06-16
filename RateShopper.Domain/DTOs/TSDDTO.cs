using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class TSDDTO
    {
        //public string RemoteID { get; set; }
        //public string Branch { get; set; }
        //public string CarClass { get; set; }
        //public string RentalLength { get; set; }
        //public string RentalLengthIDs { get; set; }
        //public string StartDate { get; set; }
        //public string DailyRate { get; set; }
        //public string ExtraDayRateFactor { get; set; }
        //public decimal ExtraDayRateValue { get; set; }
        //public string RateSystem { get; set; }
        //public long SearchSummaryId { get; set; }
        //public int RentalLengthCount { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public DateTime LogDateTime { get; set; }
        public string StrLogDateTime { get; set; }
        public string XMLRequest { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public string RequestStatus { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TSDUpdateAll
    {
        public long SearchSummaryId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BaseRentalLength { get; set; }
        //public List<UpdateCars> CarClasses { get; set; }
        public UpdateAllList CarClassesFirst { get; set; }
        public UpdateAllList CarClassesSecond { get; set; }
        public List<RentalLengthList> RentalLenghts { get; set; }
        public decimal ExtraDayRateFactor { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long LocationBrandId { get; set; }
        public long LocationID { get; set; }
        public long TetherBrandId { get; set; }
        public bool IsTetherActive { get; set; }
        public string BrandLocation { get; set; }
        public string DominentBrandName { get; set; }
        public string DependentBrandName { get; set; }
    }
    public class UpdateCars
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public decimal BaseValue { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TetherValue { get; set; }
        public bool IsDollar { get; set; }
    }

    public class UpdateAllList
    {
        public List<string> CalendarDays { get; set; }
        public List<UpdateCars> CarClasses { get; set; }
    }

    public class RentalLengthList
    {
        public string LOR { get; set; }
        public string ID { get; set; }
    }
    public class SearchJobUpdateAllDTO
    {
        public long SearchSummaryId { get; set; }
        public long CarClassId { get; set; }
        public decimal? BaseEdit { get; set; }
        public decimal? BaseEditTwo { get; set; }
        public bool IsDaily { get; set; }
    }
}
