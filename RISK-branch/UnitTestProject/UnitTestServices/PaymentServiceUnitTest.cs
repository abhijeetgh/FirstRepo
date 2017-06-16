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

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class PaymentServiceUnitTest
    {
        #region Private variables
        IGenericRepository<RiskPayment> _mockPaymentRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskPaymentType> _mockPaymentTypesRepository = null;
        IGenericRepository<RiskPaymentReason> _mockPaymentReasonRepository = null;
        IPaymentService _paymentService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockPaymentRepository = new MockGenericRepository<RiskPayment>(DomainBuilder.GetRiskPayments()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

            _mockPaymentTypesRepository = new MockGenericRepository<RiskPaymentType>(DomainBuilder.GetList<RiskPaymentType>()).SetUpRepository();
            _mockPaymentReasonRepository = new MockGenericRepository<RiskPaymentReason>(DomainBuilder.GetList<RiskPaymentReason>()).SetUpRepository();

            _paymentService = new PaymentService(_mockClaimRepository, _mockPaymentRepository, _mockPaymentTypesRepository, _mockPaymentReasonRepository);
        }

        [TestMethod]
        public void Test_method_for_get_all_payment_types()
        {
            var paymentTypeList =  _paymentService.GetAllPaymentTypesAsync().Result;

            Assert.IsNotNull(paymentTypeList);

            Assert.IsTrue(paymentTypeList.GetType() == typeof(List<PaymentTypesDto>));
        }

        [TestMethod]
        public void Test_method_for_get_all_payments_by_claim_id()
        {
            long claimnumber =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var paymentList =  _paymentService.GetAllPaymentsByClaimId(claimnumber).Result;

            Assert.IsNotNull(paymentList);

            Assert.IsTrue(paymentList.GetType() == typeof(PaymentDto));
        }

        [TestMethod]
        public void Test_method_for_add_payment_info()
        {
            var success =  _paymentService.AddPaymentInfo(DtoBuilder.Get<PaymentInfoDto>()).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_delete_payment_info()
        {
            var success =  _paymentService.DeletePaymentInfo(DtoBuilder.Get<PaymentInfoDto>()).Result;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Test_method_for_update_CdwLdw_and_Lpc2_with_CDW()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            claim.RiskContract.CDW = true;

            var isSuccess =  _paymentService.UpdateCdwLdwAndLpc2(claim.Id).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_update_CdwLdw_and_Lpc2_with_LDW()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            claim.RiskContract.LDW = true;

            var isSuccess =  _paymentService.UpdateCdwLdwAndLpc2(claim.Id).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_update_CdwLdw_and_Lpc2_with_LPC2()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            claim.RiskContract.LPC2 = true;

            var isSuccess =  _paymentService.UpdateCdwLdwAndLpc2(claim.Id).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_update_CdwLdw_and_Lpc2()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            var isSuccess =  _paymentService.UpdateCdwLdwAndLpc2(claim.Id).Result;

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void Test_method_for_get_total_payments()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x=>x.Id).FirstOrDefault();

            var amount =  _paymentService.GetTotalPayments(claimId).Result;

            if(!amount.HasValue)
            {
                amount = 0;
            }

            Assert.IsNotNull(amount);
        }

        [TestMethod]
        public void Test_method_for_check_is_payment_valid_total_payment()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            claim.TotalBilling = 5.00;

            var isValid =  _paymentService.IsPaymentValid(claim.Id, 0.5).Result;

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_method_for_check_is_payment_valid_total_payment_null()
        {
            Claim claim =  _mockClaimRepository.AsQueryable.FirstOrDefault();

            Random randobj = new Random();

            claim.TotalBilling = 5.00;

            claim.TotalPayment = null;

            double paymentValue = (double)(randobj.NextDouble()*claim.TotalBilling);

            var isValid =  _paymentService.IsPaymentValid(claim.Id, paymentValue).Result;

            Assert.IsTrue(isValid);
        }
    }

}
