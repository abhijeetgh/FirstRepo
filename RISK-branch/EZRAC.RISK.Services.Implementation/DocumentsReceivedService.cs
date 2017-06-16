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
    public class DocumentsReceivedService : IDocumentsReceivedService
    {
        GenericRepository<Claim> _claimRepository = null;
        public DocumentsReceivedService(GenericRepository<Claim> claimRepository)
        {
            _claimRepository = claimRepository;
        }

        public async Task<DocumentsReceivedDto> GetDocumentsReceivedByClaimIdAsync(long claimId)
        {
            DocumentsReceivedDto documentsReceivedDto = null;
            var claim = await _claimRepository.AsQueryable.Include(x => x.RiskDocumentsReceived).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim!=null)
            {
                documentsReceivedDto = GetDocumentsReceivedDto(claim.RiskDocumentsReceived);
            }

            return documentsReceivedDto;
        }

        private DocumentsReceivedDto GetDocumentsReceivedDto(RiskDocumentsReceived riskDocumentsReceived)
        {
            DocumentsReceivedDto documentsReceivedDto = null;
            if (riskDocumentsReceived != null)
            {
                documentsReceivedDto = new DocumentsReceivedDto();
                documentsReceivedDto.ClaimId = riskDocumentsReceived.ClaimId;
                documentsReceivedDto.ClaimFolder = riskDocumentsReceived.ClaimFolder;
                documentsReceivedDto.PoliceReport = riskDocumentsReceived.PoliceReport;
                documentsReceivedDto.EstimateApproved = riskDocumentsReceived.EstimateApproved;
                documentsReceivedDto.EstimateReceived = riskDocumentsReceived.EstimateReceived;
            }
            return documentsReceivedDto;
        }

        public async Task<bool> UpdateDocumentsReceivedAsync(DocumentsReceivedDto documentsReceivedDto)
        {
            bool isSuccess = false;
            if (documentsReceivedDto != null)
            {
                var claim = await _claimRepository.AsQueryable.Include(x => x.RiskDocumentsReceived).Where(x => x.Id == documentsReceivedDto.ClaimId).FirstOrDefaultAsync();

                if (claim.RiskDocumentsReceived != null)
                {
                    claim.RiskDocumentsReceived = GetDocumentsReceivedDomain(documentsReceivedDto, claim.RiskDocumentsReceived);
                }
                else {
                    claim.RiskDocumentsReceived = GetDocumentsReceivedDomain(documentsReceivedDto, new RiskDocumentsReceived());
                
                }
                

                await _claimRepository.UpdateAsync(claim);
                isSuccess = true;
            }
            return isSuccess;
        }

        private RiskDocumentsReceived GetDocumentsReceivedDomain(DocumentsReceivedDto documentsReceivedDto, RiskDocumentsReceived documentsReceived)
        {
            
            if (documentsReceivedDto != null)
            {
                
                documentsReceived.ClaimId = documentsReceivedDto.ClaimId;
                documentsReceived.ClaimFolder = documentsReceivedDto.ClaimFolder;
                documentsReceived.PoliceReport = documentsReceivedDto.PoliceReport;
                documentsReceived.EstimateApproved = documentsReceivedDto.EstimateApproved;
                documentsReceived.EstimateReceived = documentsReceivedDto.EstimateReceived;
            }
            return documentsReceived;
        }
    }
}
