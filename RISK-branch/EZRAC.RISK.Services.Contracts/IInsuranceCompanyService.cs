using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IInsuranceCompanyService
    {
        Task<InsuranceDto> GetInsuranceCompanyDetailById(long id);

        Task<bool> AddOrUpdateInsuranceCompany(InsuranceDto insuranceDto, long userId);

        Task<bool> DeleteInsuranceCompany(InsuranceDto insuranceDto, long userId);

        bool IsInsuranceCompanyAlreadyUsed(long id);
    }
}
