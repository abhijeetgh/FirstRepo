using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class SearchViewModel
    {
        public FinalData finalData { get; set; }

        public class HeaderInfo
        {
            public string CompanyID { get; set; }
            public string CompanyName { get; set; }
            public string Logo { get; set; }
            public bool? Inside { get; set; }
        }

        public class CompanyDetail
        {
            public long CompanyID { get; set; }
            public string CompanyName { get; set; }
            public decimal? TotalValue { get; set; }
            public decimal? BaseValue { get; set; }
            public bool? IsMovedUp { get; set; }
            public bool Islowest { get; set; }
            public bool IslowestAmongCompetitors { get; set; }

            //added for classic manipulation
            public bool IslowestAmongCompetitorsClassic { get; set; }
            public long CompanyRankBasedOnMCcarRate { get; set; }

            //added for classic manipulation
            public DateTime ArrivalDate { get; set; }

            public bool? IsPositionChanged { get; set; }
        }

        public class RatesInfo
        {
            public long CarClassID { get; set; }
            public string CarClass { get; set; }
            public string CarClassLogo { get; set; }
            //public double RuleSetID { get; set; }
            //public string RuleSetName { get; set; }
            //public double SuggestedBase { get; set; }
            //public double SuggestedTotal { get; set; }
            public List<CompanyDetail> CompanyDetails { get; set; }
        }

        public class FinalData
        {
            public List<HeaderInfo> HeaderInfo { get; set; }
            public List<RatesInfo> RatesInfo { get; set; }
        }
    }
}
