using System;
using EZRAC.RISK.Services.Contracts.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using UnitTestProject.Helpers;
namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class LookUpServiceUnitTest
    {
        #region Private variables

        IGenericRepository<Location> _mockLocationRepository = null;
        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<RiskLossType> _mockLossTypeRepository = null;
        IGenericRepository<RiskClaimStatus> _mockClaimStatusesRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskPoliceAgency> _mockPoliceAgencyRepository = null;
        IGenericRepository<RiskNoteTypes> _mockNotesTypeRepository = null;
        IGenericRepository<RiskDamageType> _mockRiskDamageTypesRepository = null;
        IGenericRepository<RiskInsurance> _mockRiskInsuranceRepository = null;
        IGenericRepository<RiskFileTypes> _mockRiskFileTypeRepository = null;
        IGenericRepository<RiskBillingType> _mockRiskBillingTypeRepository = null;
        IGenericRepository<Company> _mockRiskCompanyRepository = null;
        IGenericRepository<RiskPaymentType> _mockRiskPaymentTypeRepository = null;
        IGenericRepository<RiskWriteOffType> _mockRiskWriteOffTypeRepository = null;
        ILookUpService _lookupService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockLocationRepository = new MockGenericRepository<Location>(DomainBuilder.GetLocations()).SetUpRepository();

            _mockUserRepository = new MockGenericRepository<User>(DomainBuilder.GetUsers()).SetUpRepository();

            _mockLossTypeRepository = new MockGenericRepository<RiskLossType>(DomainBuilder.GetRiskLossTypes()).SetUpRepository();

            _mockClaimStatusesRepository = new MockGenericRepository<RiskClaimStatus>(DomainBuilder.GetRiskClaimStatusses()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockPoliceAgencyRepository = new MockGenericRepository<RiskPoliceAgency>(DomainBuilder.GetRiskPoliceAgencies()).SetUpRepository();

            _mockNotesTypeRepository = new MockGenericRepository<RiskNoteTypes>(DomainBuilder.GetRiskNotesTypes()).SetUpRepository();

            _mockRiskDamageTypesRepository = new MockGenericRepository<RiskDamageType>(DomainBuilder.GetRiskDamageTypes()).SetUpRepository();

            _mockRiskInsuranceRepository = new MockGenericRepository<RiskInsurance>(DomainBuilder.GetRiskInsurances()).SetUpRepository();

            _mockRiskFileTypeRepository = new MockGenericRepository<RiskFileTypes>(DomainBuilder.GetRiskFileTypes()).SetUpRepository();

            _mockRiskBillingTypeRepository = new MockGenericRepository<RiskBillingType>(DomainBuilder.GetRiskBillingTypes()).SetUpRepository();

            _mockRiskCompanyRepository = new MockGenericRepository<Company>(DomainBuilder.GetCompanies()).SetUpRepository();

            _mockRiskPaymentTypeRepository = new MockGenericRepository<RiskPaymentType>(DomainBuilder.GetRiskPaymentTypes()).SetUpRepository();

            _mockRiskWriteOffTypeRepository = new MockGenericRepository<RiskWriteOffType>(DomainBuilder.GetRiskWriteOffTypes()).SetUpRepository();

            _lookupService = new LookupService
                                            (_mockLocationRepository, _mockUserRepository,
                                             _mockLossTypeRepository,_mockClaimStatusesRepository,
                                             _mockPoliceAgencyRepository,_mockClaimRepository,
                                             _mockNotesTypeRepository,_mockRiskInsuranceRepository,
                                             _mockRiskDamageTypesRepository,_mockRiskFileTypeRepository,
                                             _mockRiskBillingTypeRepository,_mockRiskCompanyRepository,
                                             _mockRiskPaymentTypeRepository, _mockRiskWriteOffTypeRepository);
        }

        [TestMethod]
        public void Test_method_for_get_all_locations()
        {
            var locationsToReturn =  _lookupService.GetAllLocationsAsync().Result;

            Assert.IsTrue(locationsToReturn.Count()>0 ? true : false);
            
        }

        [TestMethod]
        public void Test_method_for__get_all_users()
        {

            var usersToReturn =  _lookupService.GetAllUsersAsync().Result;

            Assert.IsTrue(usersToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_user_by_id()
        {
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            var userDto =  _lookupService.GetUserbyIdAsync(1).Result;

            Assert.IsNotNull(userDto);

        }

        [TestMethod]
        public void Test_method_for_get_all_loss_types()
        {

            var lossTypesToReturn =  _lookupService.GetAllLossTypesAsync().Result;

            Assert.IsTrue(lossTypesToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_police_agencies()
        {

            var policeAgenciesToReturn =  _lookupService.GetAllPoliceAgenciesAsync().Result;

            Assert.IsTrue(policeAgenciesToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_claim_statuses()
        {

            var claimStatusesToReturn =  _lookupService.GetAllClaimStatusesAsync().Result;

            Assert.IsTrue(claimStatusesToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_claims_by_contract_number_or_unit_number()
        {
            string contractNumber =  _mockClaimRepository.AsQueryable.Select(x => x.RiskContract.ContractNumber).FirstOrDefault();

            string unitNumber =  _mockClaimRepository.AsQueryable.Select(x => x.RiskVehicle.UnitNumber).FirstOrDefault();

            var claimDtoList =  _lookupService.GetClaimsByContractNumberOrUnitNumber(contractNumber, unitNumber).Result;

            Assert.IsTrue(claimDtoList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_users_by_role()
        {
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            var usersToReturn =  _lookupService.GetUsersByRoleAsync(1).Result;

            Assert.IsTrue(usersToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_assign_to_users()
        {
            var usersToReturn =  _lookupService.GetAssignToUsersAsync().Result;

            Assert.IsTrue(usersToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_users_by_roles()
        {
            List<long> roleIds = _mockUserRepository.AsQueryable.Select(x => x.UserRoleID).ToList();

            var usersToReturn =  _lookupService.GetUsersByRolesAsync(roleIds).Result;

            Assert.IsTrue(usersToReturn.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_note_types()
        {
            var notesTypeList =  _lookupService.GetAllNoteTypesAsync().Result;

            Assert.IsTrue(notesTypeList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_damage_types()
        {
            var damageTypeList =  _lookupService.GetAllDamageTypesAsync().Result;

            Assert.IsTrue(damageTypeList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_insurance_companies()
        {
            var insuranceDtoList =  _lookupService.GetAllInsuranceCompaniesAsync().Result;

            Assert.IsTrue(insuranceDtoList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_file_categories()
        {
            var fileCategoryDtoList =  _lookupService.GetAllFileCategoriesAsync().Result;

            Assert.IsTrue(fileCategoryDtoList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_billing_types()
        {
            var billingTypeDtoList =  _lookupService.GetBillingTypesAsync().Result;

            Assert.IsTrue(billingTypeDtoList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_payment_types()
        {
            var riskPaymentTypeDtoList =  _lookupService.GetAllPaymentTypesAsync().Result;

            Assert.IsTrue(riskPaymentTypeDtoList.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_all_user_emails()
        {
            var userEmail =  _lookupService.GetAllUserEmailsAsync().Result;

            Assert.IsTrue(userEmail.Count() > 0 ? true : false);

        }

        [TestMethod]
        public void Test_method_for_get_location_by_code()
        {
            string locationCode =  _mockLocationRepository.AsQueryable.Select(x => x.Code).FirstOrDefault();

            var locationDto =  _lookupService.GetLocationByCodeAsync(locationCode);

            Assert.IsNotNull(locationDto);

        }

        [TestMethod]
        public void Test_method_for_get_all_companies()
        {
            var companyDtoList =  _lookupService.GetAllCompaniesAsync().Result;

            Assert.IsTrue(companyDtoList.Count() > 0 ? true : false);

        }
    }
}
