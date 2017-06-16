using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IVehicleSectionService
    {
        Task<DamageTypesDto> GetVehicleSection(long id);

        Task<bool> AddOrUpdateVehicleSection(long id, string section, long userId);

        Task<bool> DeleteVehicleSection(long id);

        bool IsVehicleSectionAlreadyUsed(long id);
    }
}
