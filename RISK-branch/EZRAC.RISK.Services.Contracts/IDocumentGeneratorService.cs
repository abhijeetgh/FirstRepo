using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IDocumentGeneratorService
    {
        
        Task<IEnumerable<DocumentCategoryDto>> GetAllDocumentTypesAsync();

        Task<IEnumerable<DriverDto>> GetDriversByClaimAsync(long claimId);

        Task<double> GetTotalDueAsync(long[] selectedBillings, long claimId);

        Task<DocumentHeaderDto> GetDocumentHeaderInfoAsync(long claimId, long selectedDriverId);

        Task<Nullable<DateTime>> GetDateOfLastPaymentAsync(long claimId);

        Task<string> GetPolicyNumberByDriverId(long driverId);

        Task<double> GetOriginalBalanceAsync(long[] selectedBillings, long claimId);

        Task<IEnumerable<RiskBillingDto>> GetSelectedBillingsByIdsAsync(long[] selectedBillings);

        Task<InsuranceDto> GetInsuranceInfoByDriverId(long driverId);

        Task<double> GetEstimatedBilling(long claimId);

        Task<double> GetActualCashValueForClaim(long claimId);

        Task<IncidentDto> GetIncidentByClaimIdAsync(long claimId);

        Task<double> GetSalvageBillByClaimIdAsync(long claimId);

        Task<string> GetSliPolicyNumberByClaimId(long claimId);
    }
}
