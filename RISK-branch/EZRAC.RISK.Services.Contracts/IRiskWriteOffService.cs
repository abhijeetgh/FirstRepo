using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IRiskWriteOffService
    {
        Task<WriteOffDto> GetWriteOffByClaimIdAsync(Int64 id);
        Task<bool> DeleteWriteOffInfo(int claimId, int writeOffId);
        Task<bool> AddWriteOffInfo(WriteOffDto writeOffDto);
    }
}
