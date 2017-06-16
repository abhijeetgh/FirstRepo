using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using EZRAC.RISK.Util;

namespace EZRAC.RISK.Services.Implementation
{
    public class RiskFileService : IRiskFileService
    {
        IGenericRepository<RiskFile> _riskFileRepository = null;        

        public RiskFileService(IGenericRepository<RiskFile> riskFileRepository)
        {
            _riskFileRepository = riskFileRepository;            
        }
        /// <summary>
        /// This function delete the file information from database.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var success = false;
            var riskFile = await _riskFileRepository.GetByIdAsync(fileId);
            if (riskFile != null)
            {
                await _riskFileRepository.DeleteAsync(riskFile);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// This function delete all the files by FileType
        /// </summary>
        /// <param name="claimId"></param>
        /// <param name="fileTypeId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFilesByCategoryAsync(long claimId, int fileTypeId)
        {
            var success = false;
            var riskFiles = await _riskFileRepository.AsQueryable.Where(x=>x.ClaimId== claimId && x.FileTypeId==fileTypeId).ToListAsync();
            if (riskFiles != null)
            {
                foreach (var riskFile in riskFiles)
                {
                    await _riskFileRepository.DeleteAsync(riskFile);
                }
                success = true;
            }
            return success;
        }

        /// <summary>
        /// This function save file information to database
        /// </summary>
        /// <param name="filesDto"></param>
        /// <returns></returns>
        public async Task<bool> SaveFileAsync(PicturesAndFilesDto filesDto)
        {
            var success = false;
            
                RiskFile riskFile = new RiskFile()
                {
                    FileTypeId = (int)filesDto.RiskFilesCategory,
                    ClaimId = Convert.ToInt64(filesDto.ClaimId),
                    FileName = filesDto.FileName,
                    FilePath = filesDto.FilePath
                };
                var file = await _riskFileRepository.InsertAsync(riskFile);
                if (file != null)
                    success = true;
            
            return success;
        }

        /// <summary>
        /// This function is used to get the information of file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>

        public async Task<PicturesAndFilesDto> GetFileAsync(int fileId)
        {
            PicturesAndFilesDto riskFileDto = null;

            var file = await _riskFileRepository.GetByIdAsync(fileId);

            if(file!=null)
            {
                riskFileDto = new PicturesAndFilesDto();
                riskFileDto.ClaimId = file.ClaimId.ToString();
                riskFileDto.FileName = file.FileName;
                riskFileDto.FilePath = file.FilePath;
            }
            return riskFileDto;
        }

        /// <summary>
        /// This function is used to get the file list by Category or file type
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public async Task<List<PicturesAndFilesDto>> GetFilesByClaimIdAsync(long claimId)
        {
            var riskFileDto = new List<PicturesAndFilesDto>();
            var riskFiles = await _riskFileRepository.AsQueryable.Include(x=>x.FileType).Where(x => x.ClaimId == claimId).ToListAsync();

            riskFileDto= riskFiles.Select(x => new PicturesAndFilesDto
            {                
                CategoryId = x.FileTypeId,
                CategoryName=x.FileType.Type,
                ClaimId = x.ClaimId.ToString(),
                FileId=x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath
            }).ToList();            
            return riskFileDto;
        }


        /// <summary>
        /// This function is used to get the file list by list of file ids
        /// </summary>
        /// <param name="fileIds"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PicturesAndFilesDto>> GetFilesByFileIds(int[] fileIds)
        {
            List<PicturesAndFilesDto> riskFileDto=null;
            if (fileIds!=null && fileIds.Any())
            {

                var predicate = PredicateBuilder.False<RiskFile>();

                foreach (var id in fileIds)
                    predicate = predicate.Or(y => y.Id == id);

                var riskFiles = await _riskFileRepository.AsQueryable.Include(x => x.FileType).Where(predicate).ToListAsync();

                riskFileDto = riskFiles.Select(x => new PicturesAndFilesDto
                {
                    CategoryId = x.FileTypeId,
                    CategoryName = x.FileType.Type,
                    ClaimId = x.ClaimId.ToString(),
                    FileId = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                }).ToList();
            }
            
            return riskFileDto;
        }
    }
}
