using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class VehicleSectionService : IVehicleSectionService
    {
        IGenericRepository<RiskDamageType> _damageTypeRepository = null;
        IGenericRepository<RiskDamage> _damageRepository = null;
        private IGenericRepository<RiskDamageType> _mockDamageTypeRepository;
        private IGenericRepository<RiskDamage> _mockDamageRepository;

        public VehicleSectionService(IGenericRepository<RiskDamageType> damageTypeRepository, IGenericRepository<RiskDamage> damageRepository)
        {
            _damageTypeRepository = damageTypeRepository;
            _damageRepository = damageRepository;
        }

        public async Task<DamageTypesDto> GetVehicleSection(long id)
        {
            var damageType = await _damageTypeRepository.GetByIdAsync(id);
            var damageTypeDto = MapVehicleSectionDto(damageType);
            return damageTypeDto;
        }


        public async Task<bool> AddOrUpdateVehicleSection(long id, string section, long userId)
        {
            var exist = IsVehicleSectionValid(section,id);
            if (exist)
            {
                return false;
            }

            var success = false;
            if (id != 0)
            {
                var vehicleSection = await _damageTypeRepository.GetByIdAsync(id);
                if (vehicleSection != null)
                {
                    vehicleSection.Section = section;
                    vehicleSection.UpdatedBy = userId;
                    vehicleSection.UpdatedDateTime = DateTime.Now;
                }

                await _damageTypeRepository.UpdateAsync(vehicleSection);
                success = true;
            }
            else 
            {
                await _damageTypeRepository.InsertAsync(new RiskDamageType
                {
                     Section = section,
                     CreatedBy = userId,
                     CreatedDateTime = DateTime.Now,
                     UpdatedDateTime = DateTime.Now
                });
                success = true;
            }
            return success;
        }

        public async Task<bool> DeleteVehicleSection(long id)
        {
            var success = false;

            var vehicleSectionUsed = _damageRepository.AsQueryable.Where(x => x.DamageTypeId == id).Any();

            if (vehicleSectionUsed)
            {
                return false;
            }
            var vehicleSectionToDelete = await _damageTypeRepository.GetByIdAsync(id);
            if (vehicleSectionToDelete != null)
            {
                vehicleSectionToDelete.IsDeleted = true;
                await _damageTypeRepository.UpdateAsync(vehicleSectionToDelete);
                success = true;
            }
            return success;
        }

        private bool IsVehicleSectionValid(string section,long id)
        {
            var isNotValid = _damageTypeRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.Section.Equals(section, StringComparison.InvariantCultureIgnoreCase)).Any();
            return isNotValid;
        }

        public bool IsVehicleSectionAlreadyUsed(long id)
        {
            var isDuplicate = _damageRepository.AsQueryable.Where(x => x.DamageTypeId == id).Any();
            return isDuplicate;
        }

        private DamageTypesDto MapVehicleSectionDto(RiskDamageType damageType)
        {
            var damageTypeDto = new DamageTypesDto();
            damageTypeDto.Id = damageType.Id;
            damageTypeDto.Section = damageType.Section;
            return damageTypeDto;
        }

    }
}
