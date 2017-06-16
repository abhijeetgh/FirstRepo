using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskDriverInsurance : BaseEntity
    {
        public Nullable<int> InsuranceId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string PolicyNumber { get; set; }
        public Nullable<double> Deductible { get; set; }
        public string InsuranceClaimNumber { get; set; }
        public Nullable<DateTime> InsuranceExpiry { get; set; }
        public string CreditCardCompany { get; set; }
        public string CreditCardPolicyNumber { get; set; }
        public Nullable<double> CreditCardCoverageAmt { get; set; }

        public RiskDriver RiskDriver { get; set; }

        public RiskInsurance RiskInsurance { get; set; }

    }
}
