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

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
    public class BillingServiceUnitTest
    {
        #region Private variables
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskBilling> _mockBillingRepository = null;
        IGenericRepository<RiskAdminCharge> _mockAdminChargesRepository = null;
        IGenericRepository<RiskPayment> _mockPaymentRepository = null;
        IGenericRepository<RiskPaymentType> _mockPaymentTypesRepository = null;
        IGenericRepository<RiskPaymentReason> _mockPaymentReasonRepository = null;
        IPaymentService _paymentService = null;
        IBillingService _billingService = null;
        ILookUpService _lookUpService = null;
        IGenericRepository<RiskWriteOff> _riskWriteOffRepository = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockBillingRepository = new MockGenericRepository<RiskBilling>(DomainBuilder.GetRiskBillings()).SetUpRepository();

            _mockAdminChargesRepository = new MockGenericRepository<RiskAdminCharge>(DomainBuilder.GetList<RiskAdminCharge>()).SetUpRepository();

            _mockPaymentRepository = new MockGenericRepository<RiskPayment>(DomainBuilder.GetRiskPayments()).SetUpRepository();

            _mockPaymentTypesRepository = new MockGenericRepository<RiskPaymentType>(DomainBuilder.GetList<RiskPaymentType>()).SetUpRepository();

            _mockPaymentReasonRepository = new MockGenericRepository<RiskPaymentReason>(DomainBuilder.GetList<RiskPaymentReason>()).SetUpRepository();

            _paymentService = new PaymentService(_mockClaimRepository, _mockPaymentRepository, _mockPaymentTypesRepository, _mockPaymentReasonRepository);

            _billingService = new BillingService(_mockClaimRepository, _mockBillingRepository, _mockAdminChargesRepository, _paymentService, _lookUpService, _riskWriteOffRepository);
        }

        [TestMethod]
        public void Test_method_for_get_billings_by_specific_claimId()
        {
            long id = (Int64)_mockClaimRepository.AsQueryable.Select(c => c.Id).Max();

            var billings = _billingService.GetBillingsByClaimIdAsync(id).Result;

            Assert.IsNotNull(billings);
        }

        [TestMethod]
        public void Test_method_for_update_billing_to_claim()
        {

            Claim claim = _mockClaimRepository.AsQueryable.Include(x => x.RiskBillings).FirstOrDefault();

            RiskBillingDto riskBillingDto = DtoBuilder.Get<RiskBillingDto>();

            riskBillingDto.BillingTypeId = claim.RiskBillings.First().BillingTypeId;

            var isSuccess = _billingService.AddOrUpdateBillingToClaimAsync(riskBillingDto).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_add_billing_to_claim()
        {
            Claim claim = _mockClaimRepository.AsQueryable.Include(x => x.RiskBillings).FirstOrDefault();

            RiskBillingDto riskBillingDto = DtoBuilder.Get<RiskBillingDto>();

            riskBillingDto.BillingTypeId = claim.RiskBillings.Select(x => x.BillingTypeId).Max() + 1;

            var isSuccess = _billingService.AddOrUpdateBillingToClaimAsync(riskBillingDto).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_delete_billing_by_specific_Id()
        {
            long id = (Int64)_mockBillingRepository.AsQueryable.Include(x => x.Claim).Select(x => x.Id).Max();

            var isSuccess = _billingService.DeleteBillingByIdAsync(id).Result;

            Assert.IsTrue(isSuccess);
        }

    }
}
