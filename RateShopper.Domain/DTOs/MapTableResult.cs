using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class MapTableResult
    {
        public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        public long ScrapperSourceID { get; set; }
        public long LocationID { get; set; }
        public long RentalLengthID { get; set; }
        //public System.DateTime Date { get; set; }
        public long CarClassID { get; set; }
        public long CompanyID { get; set; }
        public Nullable<decimal> BaseRate { get; set; }
        public Nullable<decimal> TotalRate { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public DateTime ArvDt { get; set; }
        public DateTime RtrnDt { get; set; }
    }
}
