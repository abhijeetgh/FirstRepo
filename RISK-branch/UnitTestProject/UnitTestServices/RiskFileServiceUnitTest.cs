using EZRAC.Core.Data.EntityFramework;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
    public class RiskFileServiceUnitTest
    {
        #region Private Variables

        IGenericRepository<RiskFile> _mockRiskFileRepository = null;
        IRiskFileService _mockRiskFileService = null;
       
        #endregion
        [TestInitialize]
        public void Setup()
        {
            _mockRiskFileRepository = new MockGenericRepository<RiskFile>(DomainBuilder.GetRiskFiles()).SetUpRepository();
            _mockRiskFileService = new RiskFileService(_mockRiskFileRepository);

        }
        [TestMethod]
        public void Test_method_for_get_risk_file()
        {
            int id = _mockRiskFileRepository.AsQueryable.Select(r => r.Id).Max();
            var riskFileDto = _mockRiskFileService.GetFileAsync(id).Result;
            Assert.IsNotNull(riskFileDto);
        }
        [TestMethod]
        public void Test_method_for_get_risk_file_by_claim_id()
        {
            long claimId = _mockRiskFileRepository.AsQueryable.Select(c => c.ClaimId).FirstOrDefault();
            var listOfRiskFilesDto = _mockRiskFileService.GetFilesByClaimIdAsync(claimId);
            int count = listOfRiskFilesDto.Result.Count();
            Assert.IsNotNull(listOfRiskFilesDto);       
        }
        [TestMethod]
        public void Test_method_for_get_risk_files_by_file_id()
        {
            int[] riskFileIds = _mockRiskFileRepository.AsQueryable.Select(rs => rs.Id).ToArray();
            var riskFileDto = _mockRiskFileService.GetFilesByFileIds(riskFileIds).Result;
            Assert.IsNotNull(riskFileDto);
        }
        [TestMethod]
        public void Test_method_for_save_risk_file()
        {
            var picturesAndFilesDto = DtoBuilder.Get<PicturesAndFilesDto>();
            long claimId = _mockRiskFileRepository.AsQueryable.Select(c => c.ClaimId).FirstOrDefault();
            picturesAndFilesDto.ClaimId = claimId.ToString();
            Assert.IsTrue(_mockRiskFileService.SaveFileAsync(picturesAndFilesDto).Result);
        }
        [TestMethod]
        public void Test_method_for_delete_risk_file()
        {
            int riskFileId = _mockRiskFileRepository.AsQueryable.Select(r => r.Id).Max();
            Assert.IsTrue(_mockRiskFileService.DeleteFileAsync(riskFileId).Result);
        }
        [TestMethod]
        public void Test_method_for_delete_risk_file_by_category()
        {
            long claimId = _mockRiskFileRepository.AsQueryable.Select(c => c.ClaimId).Min();
            int riskFileTypeId = _mockRiskFileRepository.AsQueryable.Select(f => f.FileTypeId).Min();
            Assert.IsTrue(_mockRiskFileService.DeleteFilesByCategoryAsync(claimId, riskFileTypeId).Result);
        }
        [TestMethod]
        public void Test_method_for_varify_is_the_claim_exit_or_not()
        {
            long claimId = _mockRiskFileRepository.AsQueryable.Select(c => c.ClaimId).Min();
            // To check whether claim is present or not
            var claimStatus = _mockRiskFileRepository.AsQueryable.Select(c => c.Claim.ClaimTrackings.Select(t => t.ClaimId == claimId).FirstOrDefault()).FirstOrDefault();
            Assert.IsTrue(claimStatus);
        }
        [TestMethod]
        public void Test_method_for_is_the_file_type_exist_or_not()
        {
            int riskFileTypeId = _mockRiskFileRepository.AsQueryable.Select(f => f.FileTypeId).Min();
            //To check whether FileType exist or not
            var fileTypeStaus = _mockRiskFileRepository.AsQueryable.Select(c => c.FileType.Id == riskFileTypeId).FirstOrDefault();
            Assert.IsTrue(fileTypeStaus);
        }

    }
}
