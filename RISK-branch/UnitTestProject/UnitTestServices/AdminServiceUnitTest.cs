using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using System.Linq;

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
    public class AdminServiceUnitTest
    {
        #region Private variables

        IGenericRepository<RiskClaimStatus> _mockClaimStatusRepository = null;
        IGenericRepository<RiskLossType> _mockLossTypeRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskWriteOffType> _mockRiskWriteOffType = null;
        IGenericRepository<RiskWriteOff> _mockRiskWriteOff = null;
        IAdminService _adminService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockClaimStatusRepository = new MockGenericRepository<RiskClaimStatus>(DomainBuilder.GetRiskClaimStatusses()).SetUpRepository();

            _mockLossTypeRepository = new MockGenericRepository<RiskLossType>(DomainBuilder.GetRiskLossTypes()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockRiskWriteOffType = new MockGenericRepository<RiskWriteOffType>(DomainBuilder.GetRiskWriteOffTypes()).SetUpRepository();

            _mockRiskWriteOff = new MockGenericRepository<RiskWriteOff>(DomainBuilder.GetRiskWriteOffs()).SetUpRepository();

            _adminService = new AdminService(_mockClaimStatusRepository, _mockLossTypeRepository, _mockClaimRepository, _mockRiskWriteOffType, _mockRiskWriteOff);
        }

        [TestMethod]
        public void Test_method_for_get_claim_status()
        {
            long claimStatusId = (Int64)_mockClaimStatusRepository.AsQueryable.Select(x => x.Id).Max();

            var claimStatusDto =  _adminService.GetClaimStatus(claimStatusId).Result;

            Assert.IsNotNull(claimStatusDto);
        }

        [TestMethod]
        public void Test_method__for_update_claim_Status()
        {
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            long id = (Int64)_mockClaimStatusRepository.AsQueryable.Select(x => x.Id).Max();

            string claimStatus = Builder<string>.CreateNew().ToString();

            var success = _adminService.AddOrUpdateClaimStatus(id, claimStatus, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_add_claim_Status()
        {
            //For Adding New Claim Status Id Must Be 0

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            string claimStatus = Builder<string>.CreateNew().ToString();

            var success = _adminService.AddOrUpdateClaimStatus(0, claimStatus, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_get_loss_type_details()
        {
            long loosTypeId = (Int64)_mockLossTypeRepository.AsQueryable.Select(x => x.Id).Max();

            var lossTypeDto =  _adminService.GetLossTypeDetails(loosTypeId).Result;


            Assert.IsNotNull(lossTypeDto);
        }

        [TestMethod]
        public void Test_method_for_update_loss_type()
        {
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            long id = (Int64)_mockLossTypeRepository.AsQueryable.Select(x => x.Id).Max();

            string loostype = Builder<string>.CreateNew().ToString();

            string description = Builder<string>.CreateNew().ToString();

            var success = _adminService.AddOrUpdateLossType(id, loostype, description, 1).Result;


            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_add_loss_type()
        {
            //For Adding New Loos Type Id Must Be 0

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            string loostype = Builder<string>.CreateNew().ToString();

            string description = Builder<string>.CreateNew().ToString();

            var success = _adminService.AddOrUpdateLossType(0, loostype, description, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_delete_loss_type()
        {
            long id = (Int64)_mockLossTypeRepository.AsQueryable.Select(x => x.Id).Max();

            var success =  _adminService.DeleteLossType(id).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_delete_claim_status()
        {
            long id = (Int64) _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var success = _adminService.DeleteClaimStatus(id).Result;

            Assert.IsTrue(success);
        }
    }
}
