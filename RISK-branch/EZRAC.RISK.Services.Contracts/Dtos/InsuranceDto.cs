using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class InsuranceDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Contact { get; set; }
        public string Notes { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        //TODO:used in Document Generator Can be moved
        public string InsuranceClaimNumber { get; set; }
        public string InsurancePolicyNumber { get; set; }
     
    }
}
