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
using System.Data.Entity;
using System.Collections.Generic;
using UnitTestProject.Helpers;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class ClaimServiceUnitTest
    {
        #region Private variables

        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<Location> _mockLocationRepository = null;
        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<RiskContract> _mockRiskContractRepository = null;
        IGenericRepository<RiskNonContract> _mockRiskNonContractRepository = null;
        IGenericRepository<RiskVehicle> _mockVehicleRepository = null;
        IGenericRepository<RiskClaimApproval> _mockRiskClaimApprovalRepository = null;
        IGenericRepository<RiskIncident> _mockRiskIncidentRepository = null;
        IGenericRepository<RiskPoliceAgency> _mockRiskPoliceAgencyRepository = null;
        IGenericRepository<RiskDamage> _mockRiskDamageRepository = null;
        IGenericRepository<RiskDriver> _mockDriverRepository = null;
        IGenericRepository<RiskDriverInsurance> _mockRiskDriverInsuranceRepository = null;


        //_mockclaimRepository
        IGenericRepository<RiskBilling> _mockBillingRepository = null;
        IGenericRepository<RiskAdminCharge> _mockAdminChargesRepository = null;

        //_mockclaimRepository
        IGenericRepository<RiskPayment> _mockPaymentRepository = null;
        IGenericRepository<RiskPaymentType> _mockPaymentTypesRepository = null;
        IGenericRepository<RiskPaymentReason> _mockPaymentReasonRepository = null;

        IBillingService _billingService = null;
        ITSDService _tsdService = null;
        IPaymentService _paymentService = null;

        IClaimService _claimService = null;
        ILookUpService _lookUpService = null;
        IGenericRepository<RiskWriteOff> _riskWriteOffRepository = null;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockDriverRepository = new MockGenericRepository<RiskDriver>(DomainBuilder.GetRiskDrivers()).SetUpRepository();

            //Call Payment Service
            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockPaymentRepository = new MockGenericRepository<RiskPayment>(DomainBuilder.GetRiskPayments()).SetUpRepository();

            _mockPaymentTypesRepository = new MockGenericRepository<RiskPaymentType>(DomainBuilder.GetList<RiskPaymentType>()).SetUpRepository();

            _mockPaymentReasonRepository = new MockGenericRepository<RiskPaymentReason>(DomainBuilder.GetList<RiskPaymentReason>()).SetUpRepository();

            _paymentService = new PaymentService(_mockClaimRepository, _mockPaymentRepository, _mockPaymentTypesRepository, _mockPaymentReasonRepository);

            //Call Billing Service
            _mockBillingRepository = new MockGenericRepository<RiskBilling>(DomainBuilder.GetRiskBillings()).SetUpRepository();

            _mockAdminChargesRepository = new MockGenericRepository<RiskAdminCharge>(DomainBuilder.GetList<RiskAdminCharge>()).SetUpRepository();

            _billingService = new BillingService(_mockClaimRepository, _mockBillingRepository, _mockAdminChargesRepository, _paymentService, _lookUpService, _riskWriteOffRepository);

            //Call Claim Service
            _mockLocationRepository = new MockGenericRepository<Location>(DomainBuilder.GetLocations()).SetUpRepository();

            _mockUserRepository = new MockGenericRepository<User>(DomainBuilder.GetUsers()).SetUpRepository();

            _mockRiskContractRepository = new MockGenericRepository<RiskContract>(DomainBuilder.GetRiskContracts()).SetUpRepository();

            _mockVehicleRepository = new MockGenericRepository<RiskVehicle>(DomainBuilder.GetRiskVehicles()).SetUpRepository();

            _mockRiskNonContractRepository = new MockGenericRepository<RiskNonContract>(DomainBuilder.GetRiskNonContracts()).SetUpRepository();

            _mockRiskClaimApprovalRepository = new MockGenericRepository<RiskClaimApproval>(DomainBuilder.GetRiskClaimApprovals()).SetUpRepository();

            _mockRiskIncidentRepository = new MockGenericRepository<RiskIncident>(DomainBuilder.GetRiskIncidents()).SetUpRepository();

            _mockRiskPoliceAgencyRepository = new MockGenericRepository<RiskPoliceAgency>(DomainBuilder.GetRiskPoliceAgencies()).SetUpRepository();

            _mockRiskDamageRepository = new MockGenericRepository<RiskDamage>(DomainBuilder.GetRiskDamages()).SetUpRepository();

            _mockRiskDriverInsuranceRepository = new MockGenericRepository<RiskDriverInsurance>(DomainBuilder.GetRiskDriverInsurances()).SetUpRepository();

            _tsdService = new MockTsdService();

            _claimService = new ClaimService(
                                        _mockClaimRepository, _mockLocationRepository,
                                        _mockUserRepository, _mockRiskContractRepository,
                                        _mockVehicleRepository, _mockRiskClaimApprovalRepository,
                                        _mockRiskIncidentRepository, _mockRiskDamageRepository,
                                        _mockRiskPoliceAgencyRepository, _mockDriverRepository,
                                        _mockRiskDriverInsuranceRepository, _mockRiskNonContractRepository,
                                        _billingService, _tsdService, _paymentService
                                        );
        }

        [TestMethod]
        public void Test_method__for_get_claims_by_criteria()
        {
            SearchClaimsCriteria claimsCriteria = new SearchClaimsCriteria();

            claimsCriteria.ClaimType = ClaimType.FollowupClaim;

            claimsCriteria.UserId = _mockClaimRepository.AsQueryable.Select(x => x.AssignedTo).FirstOrDefault();

            claimsCriteria.SortType = "RiskDriver.FirstName";

            claimsCriteria.PageSize = 2;

            IEnumerable<ClaimDto> claimDtoList = _claimService.GetClaimsByCriteria(claimsCriteria).Result;

            Assert.IsNotNull(claimDtoList);

            //Claim Sort Type : Unit Number of Risk Vehicle.

            claimsCriteria.SortType = "RiskVehicle.UnitNumber";

            claimDtoList = _claimService.GetClaimsByCriteria(claimsCriteria).Result;

            Assert.IsNotNull(claimDtoList);
        }

        [TestMethod]
        public void Test_method_for_get_pending_approved_claims_by_criteria()
        {
            SearchClaimsCriteria claimsCriteria = new SearchClaimsCriteria();

            claimsCriteria.ClaimType = ClaimType.PendingApproval;

            claimsCriteria.SortType = "RiskDriver.FirstName";

            claimsCriteria.PageSize = 2;

            claimsCriteria.UserId = _mockClaimRepository.AsQueryable.Select(x => x.AssignedTo).FirstOrDefault();

            IEnumerable<ClaimDto> claimDtoList = _claimService.GetPendingApprovedClaimsByCriteria(claimsCriteria).Result;

            Assert.IsNotNull(claimDtoList);

            //Claim Type : Pending Approval

            claimsCriteria.ClaimType = ClaimType.FollowupClaim;

            claimsCriteria.SortType = "RiskVehicle.UnitNumber";

            claimDtoList = _claimService.GetPendingApprovedClaimsByCriteria(claimsCriteria).Result;

            Assert.IsNotNull(claimDtoList);
        }

        [TestMethod]
        public void Test_method_for_create_claim()
        {

            ClaimDto createClaimRequest = DtoBuilder.GetCliamForInsert();

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            var claim = _claimService.CreateClaimAsync(createClaimRequest, 1).Result;

            Assert.IsNotNull(claim);
        }

        [TestMethod]
        public void Test_method_for_get_claim_basic_info_by_specific_claimId()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var claimBasicInfo = _claimService.GetClaimBasicInfoByClaimIdAsync(claimId).Result;

            Assert.IsNotNull(claimBasicInfo);
        }

        [TestMethod]
        public void Test_method_for_get_claim_info_by_specific_claimId()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var claimInfo = _claimService.GetClaimInfoByClaimIdAsync(claimId).Result;

            Assert.IsNotNull(claimInfo);
        }

        [TestMethod]
        public void Test_method_for_get_claim_info_for_header()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var claimInfo = _claimService.GetClaimInfoForHeaderAsync(claimId).Result;

            Assert.IsNotNull(claimInfo);
        }

        [TestMethod]
        public void Test_method_for_get_contract_info_by_specific_id()
        {
            long id = _mockRiskContractRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var contractDto = _claimService.GetContractInfoByIdAsync(id).Result;

            Assert.IsNotNull(contractDto);
        }

        [TestMethod]
        public void Test_method_for_get_vehicle_info_by_specific_id()
        {
            long vehicleId = _mockVehicleRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            string contractNumber = _mockRiskContractRepository.AsQueryable.Select(x => x.ContractNumber).FirstOrDefault();

            var vehicleDto = _claimService.GetVehicleInfoByIdAsync(vehicleId, contractNumber).Result;

            Assert.IsNotNull(vehicleDto);
        }

        [TestMethod]
        public void Test_method_for_get_incident_info_by_specific_id()
        {
            long incidentId = _mockRiskIncidentRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var incidentDto = _claimService.GetIncidentInfoByIdAsync(incidentId).Result;

            Assert.IsNotNull(incidentDto);
        }

        [TestMethod]
        public void Test_method_for_update_vehicle_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var vehicleId = _claimService.AddOrUpdateVehicleInfoAsync(DtoBuilder.Get<VehicleDto>(), claimId).Result;

            Assert.IsNotNull(vehicleId);
        }

        [TestMethod]
        public void Test_method_for_update_claim_info()
        {
            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            var claimId = _claimService.UpdateClaimInfoAsync(DtoBuilder.Get<ClaimDto>(), 1).Result;

            Assert.IsNotNull(claimId);
        }

        [TestMethod]
        public void Test_method_for_update_contract_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var contractId = _claimService.UpdateContractInfoAsync(DtoBuilder.Get<ContractDto>(), claimId).Result;

            Assert.IsNotNull(contractId);
        }

        [TestMethod]
        public void Test_method_for_update_incident_info()
        {

            var incidentId = _claimService.UpdateIncidentInfoAsync(DtoBuilder.Get<IncidentDto>()).Result;

            Assert.IsNotNull(incidentId);
        }

        [TestMethod]
        public void Test_method_for_get_follow_up_count_list()
        {

            long userId = _mockClaimRepository.AsQueryable.Select(x => x.AssignedTo).FirstOrDefault();

            int claimsCount = _claimService.GetFollowUpCountList(userId).Result;

            Assert.IsTrue(claimsCount > 0 ? true : false);
        }

        [TestMethod]
        public void Test_method_for_get_pending_approved_count_list()
        {

            long userId = _mockClaimRepository.AsQueryable.Select(x => x.AssignedTo).FirstOrDefault();

            var claimsCount = _claimService.GetPendingApprovedCountList(userId, ClaimType.AllClaim).Result;

            Assert.IsTrue(claimsCount > 0 ? true : false);
        }

        [TestMethod]
        public void Test_method_for_get_damages_info_by_claim_id()
        {

            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var dmgList = _claimService.GetDamagesInfoByClaimIdAsync(claimId).Result;

            Assert.IsTrue(dmgList.Count() > 0 ? true : false);
        }

        [TestMethod]
        public void Test_method_for_add_damage()
        {

            var success = _claimService.AddDamage(DtoBuilder.Get<DamageDto>()).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_delete_damage()
        {

            var success = _claimService.DeleteDamage(DtoBuilder.Get<DamageDto>()).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_get_salvage_info_by_specific_claim_id()
        {
            int claimId = (int)_mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var salvageDto = _claimService.GetSalvageInfoByClaimIdAsync(claimId).Result;

            Assert.IsNotNull(salvageDto);
        }

        [TestMethod]
        public void Test_method_for_set_follow_update_by_specific_claim_id()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            DateTime followupDate = DateTime.Today;

            var success = _claimService.SetFollowUpdateByClaimIdAsync(claimId, followupDate, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_set_assigned_to()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var success = _claimService.SetAssignedTo(claimId, 2, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_set_follow_up_completed()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var success = _claimService.SetFollowUpCompletedAsync(claimId, true).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_delete_claims()
        {
            List<long> claimsToDelete = _mockClaimRepository.AsQueryable.Select(x => x.Id).ToListAsync().Result;

            var success = _claimService.DeleteClaims(claimsToDelete).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_save_claim_header_info()
        {
            var claim = _claimService.SaveClaimHeaderInfo(DtoBuilder.Get<ClaimDto>(), 1).Result;

            Assert.IsTrue(claim);
        }

        [TestMethod]
        public void Test_method_for_approve_or_reject_claims()
        {
            RiskClaimApproval claim = _mockRiskClaimApprovalRepository.GetByIdAsync(1).Result;

            claim.ApprovalStatus = null;

            var success = _claimService.ApproveOrRejectClaims(claim.Id, true).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_download_contract_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            string contractNumber = _mockClaimRepository.AsQueryable.Select(x => x.RiskContract.ContractNumber).FirstOrDefault();

            _claimService.DownloadContractInfo(claimId, contractNumber);

            Assert.IsNotNull(claimId);
        }

        [TestMethod]
        public void Test_method_for_download_vehicle_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            string unitNumber = _mockClaimRepository.AsQueryable.Select(x => x.RiskVehicle.UnitNumber).FirstOrDefault();

            Nullable<long> vehicleId = _claimService.DownloadVehicleInfo(claimId, unitNumber).Result;

            Assert.IsTrue(vehicleId.HasValue);
        }

        [TestMethod]
        public void Test_method_for_search_claims_by_contract_number()
        {
            string contractNumber = _mockClaimRepository.AsQueryable.Select(x => x.RiskContract.ContractNumber).FirstOrDefault();

            var claimDtoList = _claimService.SearchClaimsByContractNumberAsync(contractNumber).Result;

            Assert.IsNotNull(claimDtoList);
        }

        [TestMethod]
        public void Test_method_for_search_claim_by_claim_id()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var success = _claimService.SearchClaimByClaimIdAsync(claimId).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_save_salvage_info()
        {
            long claimNumber = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            Nullable<decimal> amount = _mockClaimRepository.AsQueryable.Select(x => x.SalvageAmount).FirstOrDefault();

            Nullable<DateTime> dateOfReceipt = _mockClaimRepository.AsQueryable.Select(x => x.SalvageReceiptDate).FirstOrDefault();

            var success = _claimService.SaveSalvageInfo(claimNumber, amount, dateOfReceipt).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_get_advanced_search_records_with_search_criteria_risk_driver_first_name()
        {
            SearchClaimsCriteria claimsCriteria = DomainBuilder.Get<SearchClaimsCriteria>();

            claimsCriteria.SortType = "RiskDriver.FirstName";

            var claimDtoList = _claimService.GetAdvancedSearchRecords(DtoBuilder.Get<ClaimSearchDto>(), claimsCriteria).Result;

            Assert.IsTrue(claimDtoList.Count > 0);
        }

        [TestMethod]
        public void Test_method_for_get_advanced_search_records()
        {
            SearchClaimsCriteria claimsCriteria = DomainBuilder.Get<SearchClaimsCriteria>();

            claimsCriteria.SortType = "RiskContract.ContractNumber";

            var claimDtoList = _claimService.GetAdvancedSearchRecords(DtoBuilder.Get<ClaimSearchDto>(), claimsCriteria).Result;

            Assert.IsTrue(claimDtoList.Count > 0);
        }

        [TestMethod]
        public void Test_method_for_get_advanced_search_record_count()
        {

            var claimListCount = _claimService.GetAdvancedSearchRecordCount(DtoBuilder.Get<ClaimSearchDto>()).Result;

            Assert.IsTrue(claimListCount > 0);
        }

        [TestMethod]
        public void Test_method_for_update_labour_hour()
        {
            Nullable<double> labourHour = _mockClaimRepository.AsQueryable.Select(x => x.LabourHour).FirstOrDefault();

            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var isSuccess = _claimService.UpdateLabourHour(labourHour, claimId).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_get_labour_hours_by_specific_claim_id()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            Nullable<double> LabourHours = _claimService.GetLabourHoursByClaimIdAsync(claimId).Result;

            Assert.IsTrue(LabourHours.HasValue);
        }

        [TestMethod]
        public void Test_method_for_delete_claim_from_view_claim()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var success = _claimService.DeleteClaimFromViewClaim(claimId, 1).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_get_company_by_specific_claim_id()
        {
            Claim claim = _mockClaimRepository.AsQueryable.FirstOrDefault();

            claim.OpenLocation.Company = DomainBuilder.Get<Company>();

            var compantDto = _claimService.GetCompanyByClaimIdAsync(claim.Id).Result;

            Assert.IsNotNull(compantDto);
        }

        [TestMethod]
        public void Test_method_for_get_contract_by_specific_claim_id()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var contractDto = _claimService.GetContractByClaimIdAsync(claimId).Result;

            Assert.IsNotNull(contractDto);
        }

        [TestMethod]
        public void Test_method_for_get_non_contract_info_by_specific_id()
        {
            long nonContractId = _mockRiskNonContractRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var nonContractDto = _claimService.GetNonContractInfoByIdAsync(nonContractId).Result;

            Assert.IsNotNull(nonContractDto);
        }

        [TestMethod]
        public void Test_method_for_update_non_contract_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var nonContractId = _claimService.UpdateNonContractInfoAsync(DtoBuilder.Get<NonContractDto>(), claimId).Result;

            Assert.IsNotNull(nonContractId);
        }

        [TestMethod]
        public void Test_method_for_generate_non_contract_number()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var nonContractNumber = _claimService.GenerateNonContractNumber(claimId).Result;

            Assert.IsNotNull(nonContractNumber);
        }

    }
}
