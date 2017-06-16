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
using UnitTestProject.Helpers;
using System.Collections.Generic;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class RiskReportServiceUnitTest
    {
        #region Private variables

        IGenericRepository<RiskReport> _mockRiskReportRepository = null;
        IGenericRepository<RiskReportCategory> _mockRiskReportCategoryRepository = null;
        IGenericRepository<RiskVehicle> _mockVehicleRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<UserRole> _mockUserRoleRepository = null;
        ITSDService _tsdService = null;
        IEFHelper _iEFHelper = null;

        IRiskReport _riskReportService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockRiskReportRepository = new MockGenericRepository<RiskReport>(DomainBuilder.GetRiskReports()).SetUpRepository();

            _mockRiskReportCategoryRepository = new MockGenericRepository<RiskReportCategory>(DomainBuilder.GetRiskReportCategory()).SetUpRepository();

            _mockVehicleRepository = new MockGenericRepository<RiskVehicle>(DomainBuilder.GetRiskVehicles()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockUserRepository = new MockGenericRepository<User>(DomainBuilder.GetUsers()).SetUpRepository();

            _mockUserRoleRepository = new MockGenericRepository<UserRole>(DomainBuilder.GetUserRoleInfo()).SetUpRepository();

            _tsdService = new MockTsdService();

            _iEFHelper = new MockEFHelper();

            _riskReportService =  new RiskReportService(_mockClaimRepository,_mockRiskReportRepository,
                                             _mockRiskReportCategoryRepository,_mockVehicleRepository,
                                             _mockUserRepository, _mockUserRoleRepository,
                                             _iEFHelper, _tsdService);

        }

        [TestMethod]
        public void Test_method_for_get_report_list()
        {
            long userId = _mockUserRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            IEnumerable<RiskReportCategoryDto> riskReportDto = _riskReportService.GetReportListAsync(userId).Result;

            Assert.IsNotNull(riskReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_admin_report()
        {
            ReportCriteriaDto reportCriteriaDto = DtoBuilder.Get<ReportCriteriaDto>();

            reportCriteriaDto.ReportTypeKey = "useractivity";

            ReportAdminDto reportAdminDto = _riskReportService.GetAdminReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportAdminDto.UserActionLogDtos);

            reportCriteriaDto.ReportTypeKey = "calldates";

            reportAdminDto = _riskReportService.GetAdminReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportAdminDto.ReportClaimDtos);

            reportCriteriaDto.ReportTypeKey = "assignedcontracts";

            reportAdminDto = _riskReportService.GetAdminReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportAdminDto.ReportClaimDtos);

            reportCriteriaDto.ReportTypeKey = "pendingamountdue";

            reportAdminDto = _riskReportService.GetAdminReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportAdminDto.ReportPaymentDtos);

            reportCriteriaDto.ReportTypeKey = "customerservice";

            reportAdminDto = _riskReportService.GetAdminReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportAdminDto.ReportClaimDtos);
        }

        [TestMethod]
        public void Test_method_for_get_basic_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<BasicReportDtos> reportResult = _riskReportService.GetBasicReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(reportResult);
        }

        [TestMethod]
        public void Test_method_for_get_admin_fee_commission_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            AdminFeeCommissionReportDto adminFeeCommissionReportDto = _riskReportService.GetAdminFeeCommissionReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(adminFeeCommissionReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_charge_type_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            reportCriteriaDto.GreaterThanLessThan = 0;

            IEnumerable<ChargeTypeReportDto> chargeTypeReportDtoResult = _riskReportService.GetChargeTypeReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(chargeTypeReportDtoResult);

            reportCriteriaDto.GreaterThanLessThan = 1;

            chargeTypeReportDtoResult = _riskReportService.GetChargeTypeReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(chargeTypeReportDtoResult);
        }

        [TestMethod]
        public void Test_method_for_get_stolen_recovered_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<StolenRecoveredReportDto> stolenRecoveredReportResult = _riskReportService.GetStolenRecoveredReportsAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(stolenRecoveredReportResult);
        }

        [TestMethod]
        public void Test_method_for_get_deposit_date_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<DepositDateReportDto> depositDateReportDto = _riskReportService.GetDepositDateReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(depositDateReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_released_report_for_vehicle()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<VehicleToBeReleasedReportDto> vehicleToBeReleasedDto = _riskReportService.GetVahicleToBeReleasedReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(vehicleToBeReleasedDto);
        }

        [TestMethod]
        public void Test_method_for_get_writeoff_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<WriteOffCustomReportDto> WriteOffCustomReportDto =_riskReportService.GetWriteOffReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(WriteOffCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_account_receivable_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<AccountReceivableCustomReportDto> accountReceivableCustomReportDto = _riskReportService.GetAccountReceivableReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(accountReceivableCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_AR_aging_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<ARAgingCustomReportDto> aRAgingCustomReportDto = _riskReportService.GetARAgingReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(aRAgingCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_charge_back_loss_customer_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<ChargeBackLossCustomReportDto> chargeBackLossCustomReportDto = _riskReportService.GetChargeBackLossCustomReportAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(chargeBackLossCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_vehicle_damage_selection_custom_report_dto()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<VehicleDamageSectionCustomReportDto> vehicleDamageSectionCustomReportDto = _riskReportService.GetVehicleDamageSectionCustomReportDtoAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(vehicleDamageSectionCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_get_collection_report()
        {
            ReportCriteriaDto reportCriteriaDto = DomainBuilder.Get<ReportCriteriaDto>();

            IEnumerable<CollectionCustomReportDto> collectionCustomReportDto = _riskReportService.GetCollectionReportsAsync(reportCriteriaDto).Result;

            Assert.IsNotNull(collectionCustomReportDto);
        }

        [TestMethod]
        public void Test_method_for_valid_authorized_reports()
        {
            long userId = _mockUserRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            int i = 1;

            IEnumerable<RiskReport> riskReport =  _mockRiskReportRepository.GetAllAsync().Result;

            foreach (RiskReport _riskReport in riskReport)
            {
                _riskReport.ReportKey = "Key"+i;

                i++;
            }

            bool allowedReports = _riskReportService.IsAuthorizedForReports(userId);

            Assert.IsTrue(allowedReports);
        } 
    }
}
