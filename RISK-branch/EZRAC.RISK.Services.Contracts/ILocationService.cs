using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface ILocationService
    {
        Task<LocationDto> GetLocationByIdAsync(long id);

        Task<bool> AddOrUpdateLocationAsync(LocationDto locationDto);

        Task<bool> IsLocationValidAsync(LocationDto locationDto);



        Task<bool> DeleteByIdAsync(long id);

        Task<bool> IsLocationUsed(long id);
    }
}
