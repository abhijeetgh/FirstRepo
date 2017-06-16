using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ICompanyService : IBaseService<Company>
    {
        List<CompanyDTO> GetAllCompanies();
        CompanyDTO GetCompanyDetails(long companyID);
        long SaveCompany(CompanyDTO objCompanyDTO);
        void SaveCompanyLogo(long companyID, string logoPath);
        bool DeleteCompany(long companyID, long userID);
        Dictionary<string, long> GetCompaniesDictionary();
        List<CompanyDTO> GetCompanies(string companyIds);
    }
}
