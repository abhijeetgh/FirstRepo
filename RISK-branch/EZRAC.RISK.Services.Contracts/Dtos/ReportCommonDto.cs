using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportCommonDto
    {
        public string Claim { get; set; }
        public string Contract { get; set; }
        public string ClaimContract { get; set; }
        public string TagNumber { get; set; }
        public string UnitNumber { get; set; }
        public string UnitDetails { get; set; }
        public string Location { get; set; }
        public string User_Agent { get; set; }
        public string Date { get; set; }
        public string LossDate { get; set; }
        public string LossType { get; set; }
    }
}
