using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class SearchViewModelClassic
    {
        public FinalData finalData { get; set; }

        public class FinalData
        {
            public long BrandID { get; set; }
            public string BrandCode { get; set; }
            public long CarClassID { get; set; }
            public List<RatesInfo> RatesInfo { get; set; }
        }

        public class RatesInfo
        {
            public DateTime DateInfo { get; set; }
            public List<CompanyDetail> CompanyDetails { get; set; }
        }

        public class CompanyDetail
        {
            public long CompanyID { get; set; }
            public bool? Inside { get; set; }
            public string CompanyCode { get; set; }
            public decimal? BaseValue { get; set; }
            public decimal? TotalValue { get; set; }
            public bool? IsMovedUp { get; set; }
            public bool IslowestAmongCompetitors { get; set; }
        }
    }
}
