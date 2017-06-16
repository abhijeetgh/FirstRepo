using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IDocumentsReceivedService
    {
        Task<DocumentsReceivedDto> GetDocumentsReceivedByClaimIdAsync(long claimId);

        Task<bool> UpdateDocumentsReceivedAsync(DocumentsReceivedDto documentsReceivedDto);
    }
}
