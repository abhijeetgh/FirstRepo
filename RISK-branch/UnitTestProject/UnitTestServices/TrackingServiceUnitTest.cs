using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using System.Linq;
namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class TrackingServiceUnitTest
    {
        #region Private variables
        IGenericRepository<RiskTrackings> _mockTrackingRepository = null;
        IGenericRepository<ClaimTrackings> _mockClaimTrackingRepository = null;
        IGenericRepository<Permission> _mockPermissionRepository = null;
        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<UserRole> _mockUserRoleRepository = null;
        IGenericRepository<RiskTrackingTypes> _mockTrackingTypesRepository = null;

        ITrackingService _mocktrackingService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockTrackingRepository = new MockGenericRepository<RiskTrackings>(DomainBuilder.GetRiskTrackings()).SetUpRepository();
            _mockClaimTrackingRepository = new MockGenericRepository<ClaimTrackings>(DomainBuilder.GetClaimTrackings()).SetUpRepository();
            _mockPermissionRepository = new MockGenericRepository<Permission>(DomainBuilder.GetPermissions()).SetUpRepository();
            _mockUserRepository = new MockGenericRepository<User>(DomainBuilder.GetUsers()).SetUpRepository();
            _mockUserRoleRepository = new MockGenericRepository<UserRole>(DomainBuilder.GetList<UserRole>()).SetUpRepository();
            _mockTrackingTypesRepository = new MockGenericRepository<RiskTrackingTypes>(DomainBuilder.GetList<RiskTrackingTypes>()).SetUpRepository();
            _mocktrackingService = new TrackingService(_mockTrackingRepository, _mockClaimTrackingRepository, _mockPermissionRepository, _mockUserRepository, _mockUserRoleRepository, _mockTrackingTypesRepository);
        }

        [TestMethod]
        public void Test_method_for_get_all_trackings_by_type()
        {
            long trackingTypeId = _mockTrackingRepository.AsQueryable.Select(t => t.TrackingTypeId).FirstOrDefault();     
            long claimId = _mockTrackingRepository.AsQueryable.Select(c => c.ClaimTrackings.Select(ct => ct.ClaimId).FirstOrDefault()).FirstOrDefault();
            Assert.IsNotNull(_mocktrackingService.GetAllTrackingsByTypeAsync(trackingTypeId, claimId).Result);
        }
        [TestMethod]
        public void Test_method_for_get_total_time_taken()
        {
            long claimTrackingTypeId = _mockClaimTrackingRepository.AsQueryable.Select(c => c.TrackingTypeId).FirstOrDefault();
            long claimId = _mockClaimTrackingRepository.AsQueryable.Select(c => c.ClaimId).FirstOrDefault();
            Assert.IsNotNull(_mocktrackingService.GetTotalTimeTakenAsync(claimId, claimTrackingTypeId).Result);
        }
        [TestMethod]
        public void Test_method_for_get_tracking_categories()
        {
            Assert.IsNotNull(_mocktrackingService.GetTrackingCategoriesAsync());
        }
        [TestMethod]
        public void Test_method_for_update_event()
        {
            long claimId = _mockClaimTrackingRepository.AsQueryable.Select(c => c.ClaimId).FirstOrDefault();
            long userId = _mockUserRepository.AsQueryable.Select(u => u.ClaimTrackings.Select(ui => ui.CreatedBy).FirstOrDefault()).Max();
            long trackingId = _mockClaimTrackingRepository.AsQueryable.Select(t=>t.TrackingId).FirstOrDefault();
             long claimTrackingTypeId = _mockClaimTrackingRepository.AsQueryable.Select(c => c.TrackingTypeId).FirstOrDefault();
            Assert.IsTrue(_mocktrackingService.UpdateEventAsync(claimId,userId,trackingId,claimTrackingTypeId).Result);
        }
        [TestMethod]
        public void Test_method_for_undo_event_tracking()
        {
            long claimTrackingId = _mockClaimTrackingRepository.AsQueryable.Select(c => c.TrackingId).Max();
            Assert.IsTrue(_mocktrackingService.UndoEventTrackingAsync(claimTrackingId).Result);
        }


    }
}
