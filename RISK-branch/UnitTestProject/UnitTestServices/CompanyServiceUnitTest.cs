using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
    public class CompanyServiceUnitTest
    {
        #region Private variables

        IGenericRepository<Company> _mockCompanyRepository = null;
        IGenericRepository<Location> _mockLocationRepository = null;
        ICompanyService _companyService = null;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockCompanyRepository = new MockGenericRepository<Company>(DomainBuilder.GetCompanies()).SetUpRepository();

            _mockLocationRepository = new MockGenericRepository<Location>(DomainBuilder.GetLocations()).SetUpRepository();


            _companyService = new CompanyService(_mockCompanyRepository, _mockLocationRepository);
        }


        [TestMethod]
        public void Test_method_for_get_company_by_id()
        {
            long companyId = Convert.ToInt64(_mockCompanyRepository.AsQueryable.Select(x => x.Id).Max());

            var status = _companyService.GetCompanyById(companyId).Result;

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Test_method_for_get_company_list()
        {
            var company = _companyService.GetCompanyListAsync().Result;


            Assert.IsNotNull(company);
        }

        [TestMethod]
        public void Test_method_for_check_is_company_mapped()
        {
            long companyId = Convert.ToInt64(_mockLocationRepository.AsQueryable.Select(x => x.Id).Max());

            var flag = _companyService.IsCompanyMapped(companyId).Result;


            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void Test_method_for_add_company()
        {
            var status = _companyService.AddCompanyAsync(DomainBuilder.Get<CompanyDto>()).Result;


            Assert.IsTrue(status);
        }

        [TestMethod]
        public void Test_method_for_update_company()
        {
            var status = _companyService.UpdateCompanyAsync(DomainBuilder.Get<CompanyDto>()).Result;


            Assert.IsTrue(status);
        }

        [TestMethod]
        public void Test_method_for_delete_company()
        {
            long companyId = Convert.ToInt64(_mockCompanyRepository.AsQueryable.Select(x => x.Id).Max());

            var status = _companyService.DeleteCompanyAsync(companyId).Result;

            Assert.IsFalse(status);

            IEnumerable<Location> locations = _mockLocationRepository.AsQueryable.Where(x => x.CompanyId == companyId);

            foreach(Location loc in locations)
            {
                loc.CompanyId = _mockCompanyRepository.AsQueryable.Select(x => x.Id).First();
            }

            status = _companyService.DeleteCompanyAsync(companyId).Result;

            Assert.IsTrue(status);
        }
        #region Private methods

        #endregion
    }
}
