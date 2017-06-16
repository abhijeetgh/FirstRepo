using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentTypesDto>> GetAllPaymentTypesAsync();

        Task<PaymentDto> GetAllPaymentsByClaimId(long claimNumber);

        Task<bool> AddPaymentInfo(PaymentInfoDto payment);

        Task<bool> DeletePaymentInfo(PaymentInfoDto payment);

        Task<bool> UpdateCdwLdwAndLpc2(Int64 claimId);

        Task<Nullable<double>> GetTotalPayments(long claimId);

        Task<bool> IsPaymentValid(long claimId, double paymentValue);

        Task<IEnumerable<RiskPaymentReasonDto>> GetAllPaymentReasonsAsync();
    }
}
