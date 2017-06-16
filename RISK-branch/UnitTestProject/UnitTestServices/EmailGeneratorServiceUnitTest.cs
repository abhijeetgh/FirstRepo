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
using UnitTestProject.Helpers;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class EmailGeneratorServiceUnitTest
    {
        #region Private variables
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskDriver> _mockDriverRepository = null;
        IBillingService _mockBillingService = null;
        IPaymentService _mockPaymentService = null;
        IClaimService _claimService = null;
        IDriverAndIncService _driverAndIncService = null;
        IEmailGeneratorService _emailGeneratorService = null;

        //For PaymentService
        //_mockclaimRepository
        IGenericRepository<RiskPayment> _mockPaymentRepository = null;
        IGenericRepository<RiskPaymentType> _mockPaymentTypesRepository = null;
        IGenericRepository<RiskPaymentReason> _mockPaymentReasonRepository = null;

        //ForBillingService
        //_mockclaimRepository
        IGenericRepository<RiskBilling> _mockBillingRepository = null;
        IGenericRepository<RiskAdminCharge> _mockAdminChargesRepository = null;
        //_mockpaymentRepository

        //ForClaimService
        //_mockclaimRepository
        IGenericRepository<Location> _mockLocationRepository = null;
        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<RiskContract> _mockRiskContractRepository = null;
        IGenericRepository<RiskNonContract> _mockRiskNonContractRepository = null;
        IGenericRepository<RiskVehicle> _mockVehicleRepository = null;
        IGenericRepository<RiskClaimApproval> _mockRiskClaimApprovalRepository = null;
        IGenericRepository<RiskIncident> _mockRiskIncidentRepository = null;
        IGenericRepository<RiskPoliceAgency> _mockRiskPoliceAgencyRepository = null;
        IGenericRepository<RiskDamage> _mockRiskDamageRepository = null;
        //_mockdriverRepository
        IGenericRepository<RiskDriverInsurance> _mockRiskDriverInsuranceRepository = null;
        ITSDService _tsdService = null;

        //ForDriverAndIncService
        //_mockclaimRepository
        //_mockdriverRepository
        IGenericRepository<RiskInsurance> _mockInsuranceRepository = null;
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

            _mockPaymentService = new PaymentService(_mockClaimRepository, _mockPaymentRepository, _mockPaymentTypesRepository, _mockPaymentReasonRepository);

            //Call Billing Service
            _mockBillingRepository = new MockGenericRepository<RiskBilling>(DomainBuilder.GetRiskBillings()).SetUpRepository();

            _mockAdminChargesRepository = new MockGenericRepository<RiskAdminCharge>(DomainBuilder.GetList<RiskAdminCharge>()).SetUpRepository();

            _mockBillingService = new BillingService(_mockClaimRepository, _mockBillingRepository, _mockAdminChargesRepository, _mockPaymentService, _lookUpService, _riskWriteOffRepository);

            //CallDriverAndInc Service
            _mockInsuranceRepository = new MockGenericRepository<RiskInsurance>(DomainBuilder.GetRiskInsurances()).SetUpRepository();

            _driverAndIncService = new DriverService(_mockClaimRepository, _mockDriverRepository, _mockInsuranceRepository);

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
                                        _mockBillingService, _tsdService, _mockPaymentService
                                        );
            //Call This Service
            _emailGeneratorService = new EmailGeneratorService(_mockClaimRepository, _mockDriverRepository,
                                                                    _mockBillingService, _mockPaymentService,
                                                                    _claimService, _driverAndIncService);

        }

        [TestMethod]
        public void Test_method_for_get_information_to_show_list()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var informationToSendList = _emailGeneratorService.GetInformationToShowList(claimId).Result;

            Assert.IsNotNull(informationToSendList);
        }

        [TestMethod]
        public void Test_method_for_get_recipients()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(c => c.Id).FirstOrDefault();

            var recipients = _emailGeneratorService.GetRecipients(claimId).Result;

            Assert.IsNotNull(recipients);
        }

        [TestMethod]
        public void Test_method_for_get_information_to_send()
        {
            ClaimsConstant.EmailInfoToSend[] SelectedInfoToSend = Enum.GetValues(typeof(ClaimsConstant.EmailInfoToSend)).Cast<ClaimsConstant.EmailInfoToSend>().ToArray();

            // User id can be any value.
            // Remind that the user id must be present in user repository while it mocked.

            var informationToSendDto = _emailGeneratorService.GetInformationToSend(SelectedInfoToSend, 1).Result;

            Assert.IsNotNull(informationToSendDto);
        }

        [TestMethod]
        public void Test_method_for_get_email_recipients()
        {

            var emailGeneratorDto = DomainBuilder.Get<EmailGeneratorDto>();

            emailGeneratorDto.SelectedReceipient = (int)ClaimsConstant.EmailRecipients.PrimaryDriver;


            emailGeneratorDto.ClaimId = _mockDriverRepository.AsQueryable.Where(x => x.DriverTypeId == (int)ClaimsConstant.EmailRecipients.PrimaryDriver).Select(x => x.ClaimId).FirstOrDefault().Value;

            var toEmails = _emailGeneratorService.GetEmailRecipients(emailGeneratorDto).Result;

            Assert.IsTrue(toEmails != null && toEmails.Any());

            emailGeneratorDto.SelectedReceipient = (int)ClaimsConstant.EmailRecipients.AdditionalDriver;


            emailGeneratorDto.ClaimId = _mockDriverRepository.AsQueryable.Where(x => x.DriverTypeId == (int)ClaimsConstant.EmailRecipients.AdditionalDriver).Select(x => x.ClaimId).FirstOrDefault().Value;

            toEmails = _emailGeneratorService.GetEmailRecipients(emailGeneratorDto).Result;

            Assert.IsTrue(toEmails != null && toEmails.Any());

            emailGeneratorDto.SelectedReceipient = (int)ClaimsConstant.EmailRecipients.ThirdPartyDriver;


            emailGeneratorDto.ClaimId = _mockDriverRepository.AsQueryable.Where(x => x.DriverTypeId == (int)ClaimsConstant.EmailRecipients.ThirdPartyDriver).Select(x => x.ClaimId).FirstOrDefault().Value;

            toEmails = _emailGeneratorService.GetEmailRecipients(emailGeneratorDto).Result;

            Assert.IsTrue(toEmails != null && toEmails.Any());


        }

        [TestMethod]
        public void Test_method_for_get_subject_line()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            //The lgoged user name can any string . Here we use "Cybage"

            var subject = _emailGeneratorService.GetSubjectLine(claimId, "Cybage").Result;

            Assert.IsNotNull(subject);
        }
    }
}
