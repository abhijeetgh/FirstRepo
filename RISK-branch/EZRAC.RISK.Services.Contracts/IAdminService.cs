using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IAdminService
    {
        Task<ClaimStatusDto> GetClaimStatus(long id);

        Task<bool> AddOrUpdateClaimStatus(long id, string claimStatus, long userId);

        Task<bool> DeleteClaimStatus(long id);

        Task<LossTypesDto> GetLossTypeDetails(long lossTypeId);

        Task<WriteOffTypeDTO> GetWriteOffTypeDetails(long writeOffTypeId);

        Task<bool> AddOrUpdateLossType(long id, string lossType, string description, long userId);

        Task<bool> AddOrUpdateWriteOffType(long id, string writeOffType, string description, long userId);

        Task<bool> DeleteLossType(long id);

        bool IsClaimStatusUsedInClaim(long id);

        bool IsLossTypeUsedInClaim(long id);

        bool IsWriteOffTypeUsedInClaim(long id);

        Task<bool> DeleteWriteOffType(long id);
    }
}
