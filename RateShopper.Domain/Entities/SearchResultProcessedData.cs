using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class SearchResultProcessedData: BaseEntity
    {
        //public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        [Required]
        public long ScrapperSourceID { get; set; }
        [Required]
        public long LocationID { get; set; }
        [Required]
        public long RentalLengthID { get; set; }
   
        public Nullable<long> CarClassID { get; set; }
        
        public Nullable<System.DateTime> DateFilter { get; set; }

        [Required]
        public string DailyViewJSONResult { get; set; }

        [Required]
        public string ClassicViewJSONResult { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public virtual SearchSummary SearchSummary { get; set; }
    }
}
