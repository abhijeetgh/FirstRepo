using System;
using EZRAC.RISK.Services.Contracts.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using System.Linq;

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
     [TestClass]
    public class LocationServiceUnitTest
    {
        #region Private variables

        IGenericRepository<RiskClaimStatus> _mockRiskClaimStatusRepository = null;
        IGenericRepository<RiskLossType> _mockRiskLossTypeRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<Location> _mockLocationRepository = null;
        IGenericRepository<Company> _mockCompanyRepository = null;       
        IGenericRepository<RiskIncident> _mockIncidentRepository = null;
        
        ILocationService _locationService = null;

        #endregion
         [TestInitialize]
         public void Setup()
         {
             _mockRiskClaimStatusRepository = new MockGenericRepository<RiskClaimStatus>(DomainBuilder.GetRiskClaimStatusses()).SetUpRepository();

             _mockRiskLossTypeRepository = new MockGenericRepository<RiskLossType>(DomainBuilder.GetRiskLossTypes()).SetUpRepository();

             _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

             _mockLocationRepository = new MockGenericRepository<Location>(DomainBuilder.GetLocations()).SetUpRepository();

             _mockCompanyRepository = new MockGenericRepository<Company>(DomainBuilder.GetCompanies()).SetUpRepository();             

             _mockIncidentRepository = new MockGenericRepository<RiskIncident>(DomainBuilder.GetRiskIncidents()).SetUpRepository();



             _locationService = new LocationService(_mockRiskClaimStatusRepository, _mockRiskLossTypeRepository,
                                                _mockClaimRepository, _mockLocationRepository,
                                                _mockCompanyRepository, _mockIncidentRepository);
         }

         [TestMethod]
         public void Test_method_for_get_location_by_id()
         {
             long locationId = _mockLocationRepository.AsQueryable.Select(x => x.Id).Max();

             var location = _locationService.GetLocationByIdAsync(locationId).Result;

             Assert.IsNotNull(location);
         }

         [TestMethod]
         public void Test_method_for_update_location()
         {
             var location = _locationService.AddOrUpdateLocationAsync(DtoBuilder.Get<LocationDto>()).Result;

             Assert.IsNotNull(location);
         }

         [TestMethod]
         public void Test_method_for_add_location()
         {
             var location = _locationService.AddOrUpdateLocationAsync(DtoBuilder.GetLocationForInsert()).Result;


             Assert.IsNotNull(location);
         }

         [TestMethod]
         public void Test_method_for_check_is_location_valid()
         {
             var location = _locationService.IsLocationValidAsync(DtoBuilder.Get<LocationDto>()).Result;


             Assert.IsTrue(location);
         }

         [TestMethod]
         public void Test_method_for_delete_by_id()
         {
             long locationId = _mockLocationRepository.AsQueryable.Select(x => x.Id).Max();

             var location = _locationService.DeleteByIdAsync(locationId).Result;

             Assert.IsTrue(location);
         }

         [TestMethod]
         public void Test_method_for_check_is_location_used()
         {
             long locationId = _mockLocationRepository.AsQueryable.Select(x => x.Id).Max();

             var location = _locationService.IsLocationUsed(locationId).Result;

             Assert.IsTrue(location);
         }
    }
}
