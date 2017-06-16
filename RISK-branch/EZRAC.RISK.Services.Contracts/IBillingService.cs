using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IBillingService
    {
        Task<BillingsDto> GetBillingsByClaimIdAsync(Int64 id);

        Task<bool> AddOrUpdateBillingToClaimAsync(RiskBillingDto billingDto);

        Task<bool> DeleteBillingByIdAsync(Int64 id);
        
    }
}
