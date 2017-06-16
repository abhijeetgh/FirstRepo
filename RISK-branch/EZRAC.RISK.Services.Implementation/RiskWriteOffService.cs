using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class RiskWriteOffService : IRiskWriteOffService
    {
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskWriteOff> _riskWriteOffRepository = null;
        IGenericRepository<RiskWriteOffType> _riskWriteOffTypeRepository = null;
        ILookUpService _lookUpService = null;

        public RiskWriteOffService(IGenericRepository<Claim> claimRepository, IGenericRepository<RiskWriteOff> riskWriteOffRepository, IGenericRepository<RiskWriteOffType> riskWriteOffTypeRepository, ILookUpService lookUpService)
        {
            _claimRepository = claimRepository;
            _riskWriteOffRepository = riskWriteOffRepository;
            _riskWriteOffTypeRepository = riskWriteOffTypeRepository;
            _lookUpService = lookUpService;
        }
        public async Task<WriteOffDto> GetWriteOffByClaimIdAsync(Int64 claimId)
        {
            WriteOffDto writeOffDto = null;

            Claim claim = await _claimRepository.AsQueryable.Include(x => x.RiskWriteOffs).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            IEnumerable<RiskWriteOff> riskWriteOff = await _riskWriteOffRepository.AsQueryable.Where(x => x.ClaimId == claimId).ToListAsync();
            IEnumerable<RiskWriteOffType> riskWriteOffType = await _riskWriteOffTypeRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            IEnumerable<WriteOffDto> lstWriteOffDto = (from rwo in riskWriteOff
                                                       join rwot in riskWriteOffType on rwo.WriteOffTypeId equals rwot.Id
                                                       select new WriteOffDto()
                                                       {
                                                           WriteOffTypeId = rwo.WriteOffTypeId,
                                                           WriteOffType = rwot.Type,
                                                           Amount = rwo.Amount,
                                                           ClaimId = claimId,
                                                           WriteOffId = rwo.Id
                                                       }).ToList();

            writeOffDto = MapWriteOffPaymentDto(claim, lstWriteOffDto);

            return writeOffDto;
        }

        public async Task<bool> AddWriteOffInfo(WriteOffDto writeOffDto)
        {
            var success = false;
            if (writeOffDto != null)
            {
                var riskWriteOff = new RiskWriteOff()
                {
                    Amount = writeOffDto.Amount,
                    ClaimId = writeOffDto.ClaimId,
                    WriteOffDate = DateTime.Now,
                    WriteOffTypeId = writeOffDto.WriteOffTypeId

                };
                await _riskWriteOffRepository.InsertAsync(riskWriteOff);
                success = true;
            }

            var claim = await _claimRepository.AsQueryable.Where(x => x.Id == writeOffDto.ClaimId).FirstOrDefaultAsync();
            if (claim != null)
            {
                claim.TotalBilling = (claim.TotalBilling.HasValue ? claim.TotalBilling.Value : default(double)) - writeOffDto.Amount;
                await _claimRepository.UpdateAsync(claim);
            }
            return success;
        }

        public async Task<bool> DeleteWriteOffInfo(int claimId, int writeOffId)
        {
            var success = false;
            if (writeOffId > 0)
            {
                var deleteWriteOff = await _riskWriteOffRepository.AsQueryable.Include(x => x.Claim).Where(x => x.Id == writeOffId).FirstOrDefaultAsync();

                if (deleteWriteOff != null)
                {
                    deleteWriteOff.Claim.TotalBilling = (deleteWriteOff.Claim.TotalBilling.HasValue ? deleteWriteOff.Claim.TotalBilling.Value : default(double)) + deleteWriteOff.Amount;
                    await _riskWriteOffRepository.DeleteAsync(deleteWriteOff);
                }
                success = true;
            }

            return success;
        }

        private WriteOffDto MapWriteOffPaymentDto(Claim claim, IEnumerable<WriteOffDto> lstWriteOffDto)
        {
            WriteOffDto writeOffDto = null;

            if (claim != null)
            {
                writeOffDto = new WriteOffDto();
                writeOffDto.ClaimId = claim.Id;
                writeOffDto.TotalWriteOff = lstWriteOffDto.Any() ? lstWriteOffDto.Select(x => x.Amount).Sum() : default(double);
                writeOffDto.TotalDue = _lookUpService.GetTotalDue(claim.TotalBilling, claim.TotalPayment);
                writeOffDto.TotalDue = Math.Round(writeOffDto.TotalDue.HasValue ? writeOffDto.TotalDue.Value : default(double), 2);
                writeOffDto.WriteOffInfo = lstWriteOffDto;
            }
            return writeOffDto;
        }
    }
}
