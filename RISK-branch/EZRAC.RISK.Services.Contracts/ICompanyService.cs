using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace EZRAC.RISK.Services.Contracts
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetCompanyListAsync();
        Task<CompanyDto> GetCompanyById(long companyId);
        Task<bool> AddCompanyAsync(CompanyDto companyDto);
        Task<bool> UpdateCompanyAsync(CompanyDto companyDto);
        Task<bool> DeleteCompanyAsync(long companyId);
        Task<bool> IsCompanyMapped(long companyId);
    }
}
