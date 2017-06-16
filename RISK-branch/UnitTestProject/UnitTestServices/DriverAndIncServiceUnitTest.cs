using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Collections.Generic;
using EZRAC.RISK.Util.Common;


namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class DriverAndIncServiceUnitTest
    {
        #region Private variables

        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskDriver> _mockDriverRepository = null;
        IGenericRepository<RiskInsurance> _mockInsuranceRepository = null;
        IDriverAndIncService _driverAndIncService = null;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockDriverRepository = new MockGenericRepository<RiskDriver>(DomainBuilder.GetRiskDrivers()).SetUpRepository();

            _mockInsuranceRepository = new MockGenericRepository<RiskInsurance>(DomainBuilder.GetRiskInsurances()).SetUpRepository();

            _driverAndIncService = new DriverService(_mockClaimRepository, _mockDriverRepository, _mockInsuranceRepository);
        }

        [TestMethod]
        public void Test_method_for_get_driver_list_by_claim_id()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var driverAndIncInfoDtoList = _driverAndIncService.GetDriverListByClaimIdAsync(claimId).Result;


            Assert.IsNotNull(driverAndIncInfoDtoList);
        }

        [TestMethod]
        public void Test_method_for_get_driver_by_id()
        {
            long driverId = _mockDriverRepository.AsQueryable.Select(d => d.Id).FirstOrDefault();

            var driverAndIncInfoDto = _driverAndIncService.GetDriverByIdAsync(driverId).Result;


            Assert.IsNotNull(driverAndIncInfoDto);
        }

        [TestMethod]
        public void Test_method_for_get_driver_by_type_id_of_claim()
        {

            ClaimsConstant.DriverTypes type = (ClaimsConstant.DriverTypes)_mockDriverRepository.AsQueryable.Select(d => d.DriverTypeId).FirstOrDefault();

            long? claimId = _mockDriverRepository.AsQueryable.Include(d => d.RiskDriverInsurance).Select(d => d.ClaimId).FirstOrDefault();

            IEnumerable<DriverInfoDto> driverAndIncInfoDto = null;

            if(claimId.HasValue)
            {
                driverAndIncInfoDto = _driverAndIncService.GetDriverByTypeIdOfClaimAsync(type, claimId.Value).Result;
            }

            Assert.IsNotNull(driverAndIncInfoDto);
        }

        [TestMethod]
        public void Test_method_for_update_driver()
        {

            var mustdriverId = _driverAndIncService.UpdateDriverAsync(DomainBuilder.Get<DriverInfoDto>(), 1).Result;

            Assert.IsNotNull(mustdriverId);
        }

        [TestMethod]
        public void Test_method_for_add_driver()
        {
            var newdriverId = _driverAndIncService.UpdateDriverAsync(DtoBuilder.GetDriverForInsert(), 1).Result;

            Assert.IsNotNull(newdriverId);
        }

        
    }
}
