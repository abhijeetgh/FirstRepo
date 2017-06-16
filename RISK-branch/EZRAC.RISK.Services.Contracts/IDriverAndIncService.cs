using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IDriverAndIncService
    {
        Task<IEnumerable<DriverInfoDto>> GetDriverListByClaimIdAsync(Int64 Id);

        Task<IEnumerable<DriverInfoDto>> GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes type, long claimId);

        Task<DriverInfoDto> GetDriverByIdAsync(Int64 Id);

        Task<Nullable<Int64>> UpdateDriverAsync(DriverInfoDto driverAndIncInfoDto, Int64 claimId);
    }
}
