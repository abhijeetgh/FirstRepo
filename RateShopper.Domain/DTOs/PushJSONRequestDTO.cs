using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class PushJSONRequestDTO
    {
        public string User { get; set; }
        public string ShopStartDate { get; set; }
        public string ShopEndDate { get; set; }
        public string DataSource { get; set; }
        public string SearchId { get; set; }
        public string TotalRate { get; set; }
        public string CarClass { get; set; }
        public string BaseRate { get; set; }
        public string VendCd { get; set; }
        public string CityCd { get; set; }
        public string LOR { get; set; }
    }
    public class CustomPushJSONDTO
    {
        public string User { get; set; }
        public string ShopStartDate { get; set; }
        public string ShopEndDate { get; set; }
    }
}
