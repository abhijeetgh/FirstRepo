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
using System;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class InsuranceServiceUnitTest
    {
        #region Private variables
        IGenericRepository<RiskInsurance> _mockRiskInsuranceRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskDriverInsurance> _mockRiskdriverInsuranceRepository = null;
        IInsuranceCompanyService _insuranceService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockRiskInsuranceRepository = new MockGenericRepository<RiskInsurance>(DomainBuilder.GetRiskInsurances()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockRiskdriverInsuranceRepository = new MockGenericRepository<RiskDriverInsurance>(DomainBuilder.GetRiskDriverInsurances()).SetUpRepository();

            _insuranceService = new InsuranceService(_mockRiskInsuranceRepository, _mockClaimRepository, _mockRiskdriverInsuranceRepository);
        }

        [TestMethod]
        public void Test_method_for_get_insurance_company_detail_by_id()
        {
            long id = _mockRiskInsuranceRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var insuranceDto = _insuranceService.GetInsuranceCompanyDetailById(id).Result;

            Assert.IsNotNull(insuranceDto);

        }

        [TestMethod]
        public void Test_method_for_check_already_exist_insurance_company()
        {

            RiskInsurance insuranceCompany = _mockRiskInsuranceRepository.AsQueryable.FirstOrDefault();

            if (insuranceCompany != null)
            {
                insuranceCompany.Id = _mockRiskInsuranceRepository.AsQueryable.Select(x => x.Id).Max() + 1; 
            }            

            InsuranceDto insuranceDto = DomainBuilder.Get<InsuranceDto>();

            insuranceDto.CompanyName = insuranceCompany.CompanyName;

            bool success = _insuranceService.AddOrUpdateInsuranceCompany(insuranceDto,1).Result;

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void Test_method_for_add_insurance_company()
        {

            InsuranceDto insuranceDto = DomainBuilder.Get<InsuranceDto>();

            insuranceDto.Id = 0;

            insuranceDto.CompanyName = "Cybage";

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            bool success = _insuranceService.AddOrUpdateInsuranceCompany(insuranceDto, 1).Result;

            Assert.IsTrue(success);

            insuranceDto.Id = 0;

            insuranceDto.CompanyName = _mockRiskInsuranceRepository.AsQueryable.FirstOrDefault().CompanyName;

            bool success2 = _insuranceService.AddOrUpdateInsuranceCompany(insuranceDto, 1).Result;

            Assert.IsFalse(success2);
        }

        [TestMethod]
        public void Test_method_for_update_insurance_company()
        {

            InsuranceDto insuranceDto = DomainBuilder.Get<InsuranceDto>();

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            bool success = _insuranceService.AddOrUpdateInsuranceCompany(insuranceDto, 1).Result;

            Assert.IsTrue(success);

        }

        [TestMethod]
        public void Test_method_for_check_already_used_insurance_company_delete()
        {

            InsuranceDto insuranceDto = DomainBuilder.Get<InsuranceDto>();

            Nullable<int> insuranceId = _mockRiskdriverInsuranceRepository.AsQueryable.Select(x => x.InsuranceId).FirstOrDefault();

            if(insuranceId.HasValue)
            {
                insuranceDto.Id = insuranceId.Value;
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.
                bool success = _insuranceService.DeleteInsuranceCompany(insuranceDto, 1).Result;

            Assert.IsFalse(success);
        }

        }

        [TestMethod]
        public void Test_method_for_delete_insurance_company()
        {
            InsuranceDto insuranceDto = DomainBuilder.Get<InsuranceDto>();

            bool success = false;

            Nullable<int> MaxInsuranceId = _mockRiskdriverInsuranceRepository.AsQueryable.Select(x => x.InsuranceId).Max();


            if (MaxInsuranceId.HasValue)
            {
                insuranceDto.Id = MaxInsuranceId.Value+1;

                // User id can be any value.
                // Remind that the user id must be present in user repository while it mocked.

                success = _insuranceService.DeleteInsuranceCompany(insuranceDto, 1).Result;
            }

            Assert.IsTrue(success);
        }
    }
}
