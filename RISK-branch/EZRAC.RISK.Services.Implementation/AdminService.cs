using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
namespace EZRAC.RISK.Services.Implementation
{
    public class AdminService :IAdminService
    {
        IGenericRepository<RiskClaimStatus> _claimStatusRepository = null;
        IGenericRepository<RiskLossType> _lossTypeRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskWriteOffType> _writeOffTypeRepository = null;
        IGenericRepository<RiskWriteOff> _writeOffRepository = null;


        public AdminService(IGenericRepository<RiskClaimStatus> claimStatusRepository,
                            IGenericRepository<RiskLossType> lossTypeRepository,
                            IGenericRepository<Claim> claimRepository,
            IGenericRepository<RiskWriteOffType> writeOffTypeRepository,
            IGenericRepository<RiskWriteOff> writeOffRepository)
        {
            _claimStatusRepository = claimStatusRepository;
            _lossTypeRepository = lossTypeRepository;
            _claimRepository = claimRepository;
            _writeOffTypeRepository = writeOffTypeRepository;
            _writeOffRepository = writeOffRepository;
        }

        public async Task<ClaimStatusDto> GetClaimStatus(long id)
        {
            var claimStatus = await _claimStatusRepository.GetByIdAsync(id);
            var claimStatusDto = MapClaimStatusDto(claimStatus);
            return claimStatusDto;
        }

        public async Task<bool> AddOrUpdateClaimStatus(long id, string claimStatus, long userId)
        {
            var success = false;

            var exist = IsClaimStatusValid(claimStatus,id);

            if (exist)
            {
                return false;
            }

            if (claimStatus != null && !string.IsNullOrWhiteSpace(claimStatus))
            {
                if (id != 0)
                {
                    var claimStausObject = await _claimStatusRepository.GetByIdAsync(id);
                    claimStausObject.Description = claimStatus;
                    claimStausObject.UpdatedBy = userId;
                    claimStausObject.UpdatedDateTime = DateTime.Now;
                    await _claimStatusRepository.UpdateAsync(claimStausObject);
                    success = true;
                }
                else
                {
                    await _claimStatusRepository.InsertAsync(
                        new RiskClaimStatus
                        {
                            Description = claimStatus,
                            CreatedBy = userId,
                            CreatedDateTime = DateTime.Now,
                            UpdatedDateTime = DateTime.Now
                        });
                    success = true;
                }
            }
            return success;
        }


        public async Task<LossTypesDto> GetLossTypeDetails(long lossTypeId)
        {
            var lossType = await _lossTypeRepository.GetByIdAsync(lossTypeId);
            var lossTypeDto = MapLossTypeDto(lossType);
            return lossTypeDto;
        }

        public async Task<WriteOffTypeDTO> GetWriteOffTypeDetails(long writeOffTypeId)
        {
            var writeOffType = await _writeOffTypeRepository.GetByIdAsync(writeOffTypeId);
            var writeOffTypeDto = MapWriteOffTypeDto(writeOffType);
            return writeOffTypeDto;
        }


        public async Task<bool> AddOrUpdateLossType(long id, string lossType, string description, long userId)
        {
            var success = false;

            var exist = IsLossTypeValid(lossType,id);

            if (exist)
            {
                return false;
            }

            if (id != 0)
            {
                var lossTypeToUpdate = await _lossTypeRepository.GetByIdAsync(id);
                lossTypeToUpdate.Type = lossType;
                lossTypeToUpdate.Description = description;
                lossTypeToUpdate.UpdatedBy = userId;
                lossTypeToUpdate.UpdatedDateTime = DateTime.Now;
                await _lossTypeRepository.UpdateAsync(lossTypeToUpdate);
                success = true;
            }
            else {
                await _lossTypeRepository.InsertAsync(
                    new RiskLossType
                    {
                        Type = lossType,
                        Description = description,
                        CreatedBy = userId,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now
                    });
                success = true;
            }
            
            return success;
        }

        public async Task<bool> AddOrUpdateWriteOffType(long id, string writeOffType, string description, long userId)
        {
            var success = false;

            var exist = IsWriteOffTypeValid(writeOffType, id);

            if (exist)
            {
                return false;
            }

            if (id != 0)
            {
                var writeOffTypeToUpdate = await _writeOffTypeRepository.GetByIdAsync(id);
                writeOffTypeToUpdate.Type = writeOffType;
                writeOffTypeToUpdate.Description = description;
                writeOffTypeToUpdate.UpdatedBy = userId;
                writeOffTypeToUpdate.UpdatedDateTime = DateTime.Now;
                await _writeOffTypeRepository.UpdateAsync(writeOffTypeToUpdate);
                success = true;
            }
            else
            {
                await _writeOffTypeRepository.InsertAsync(
                    new RiskWriteOffType
                    {
                        Type = writeOffType,
                        Description = description,
                        CreatedBy = userId,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now
                    });
                success = true;
            }

            return success;
        }

        public async Task<bool> DeleteLossType(long id)
        {
            var success = false;
            var lossTypeToDelete = await _lossTypeRepository.GetByIdAsync(id);
            if (lossTypeToDelete != null)
            {
                lossTypeToDelete.IsDeleted = true;
                await _lossTypeRepository.UpdateAsync(lossTypeToDelete);
                success = true; 
            }
            return success;
        }

        public async Task<bool> DeleteClaimStatus(long id)
        {
           var success = false;
           var statusToDelete =  await _claimStatusRepository.GetByIdAsync(id);
           if (statusToDelete != null)
           {
               statusToDelete.IsDeleted = true;
               await _claimStatusRepository.UpdateAsync(statusToDelete);
               success = true;
           }
           return success;
        }

        public async Task<bool> DeleteWriteOffType(long id)
        {
            var success = false;
            var writeOffTypeToDelete = await _writeOffTypeRepository.GetByIdAsync(id);
            if (writeOffTypeToDelete != null)
            {
                writeOffTypeToDelete.IsDeleted = true;
                await _writeOffTypeRepository.UpdateAsync(writeOffTypeToDelete);
                success = true;
            }
            return success;
        }

        private bool IsLossTypeValid(string lossType,long id)
        {
            var valid = _lossTypeRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.Type.Equals(lossType, StringComparison.InvariantCultureIgnoreCase)).Any();
            return valid;
        }

        private bool IsWriteOffTypeValid(string writeOff, long id)
        {
            var valid = _writeOffTypeRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.Type.Equals(writeOff, StringComparison.InvariantCultureIgnoreCase)).Any();
            return valid;
        }

        private bool IsClaimStatusValid(string claimStatus,long id)
        {
            var valid = _claimStatusRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.Description.Equals(claimStatus, StringComparison.InvariantCultureIgnoreCase)).Any();
            return valid;
        }


        public bool IsClaimStatusUsedInClaim(long id)
        {
            var claimStatusUsed = _claimRepository.AsQueryable.Where(x => x.ClaimStatusId == id).Any();
            return claimStatusUsed;
        }

        public bool IsLossTypeUsedInClaim(long id)
        {
            var lossTypeUsed = _claimRepository.AsQueryable.Where(x => x.LossTypeId == id).Any();
            return lossTypeUsed;
        }

        public bool IsWriteOffTypeUsedInClaim(long id)
        {
            var writeOffTypeUsed = _writeOffRepository.AsQueryable.Where(x => x.WriteOffTypeId == id).Any();
            return writeOffTypeUsed;
        }

        private ClaimStatusDto MapClaimStatusDto(RiskClaimStatus status)
        {
            var claimStatusDto = new ClaimStatusDto();
            claimStatusDto.Id = status.Id;
            claimStatusDto.Description = status.Description;
            return claimStatusDto;
        }

        private LossTypesDto MapLossTypeDto(RiskLossType lossType)
        {
            var lossTypeDto = new LossTypesDto();
            lossTypeDto.Id = lossType.Id;
            lossTypeDto.Status = lossType.Status;
            lossTypeDto.Type = lossType.Type;
            lossTypeDto.Description = lossType.Description;
            return lossTypeDto;
        }

        private WriteOffTypeDTO MapWriteOffTypeDto(RiskWriteOffType writeOffType)
        {
            var writeOffTypeDto = new WriteOffTypeDTO();
            writeOffTypeDto.Id = writeOffType.Id;
            writeOffTypeDto.Status = writeOffType.Status;
            writeOffTypeDto.Type = writeOffType.Type;
            writeOffTypeDto.Description = writeOffType.Description;
            return writeOffTypeDto;
        }
    }
}
