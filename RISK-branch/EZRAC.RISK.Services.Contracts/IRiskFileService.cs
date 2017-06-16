using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IRiskFileService
    {        
        Task<bool> DeleteFileAsync(int fileId);
        Task<bool> DeleteFilesByCategoryAsync(long claimId, int fileTypeId);
        Task<bool> SaveFileAsync(PicturesAndFilesDto filesDto);
        Task<PicturesAndFilesDto> GetFileAsync(int fileId);
        Task<List<PicturesAndFilesDto>> GetFilesByClaimIdAsync(long claimId);

        Task<IEnumerable<PicturesAndFilesDto>> GetFilesByFileIds(int[] fileIds);

    }
}
