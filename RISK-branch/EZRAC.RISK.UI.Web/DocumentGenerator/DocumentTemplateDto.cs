using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.DocumentGenerator
{
    public class DocumentTemplateDto
    {
        

        public DriverInfoDto DriverDto { get; set; }

        public UserDto User { get; set; }

        public CompanyDto CompanyDto { get; set; }

        public string ContractNumber { get; set; }

        public double TotalDue { get; set; }

        public long ClaimId { get; set; }
    }
}