using System;
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

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
    public class DocumentGeneratorServiceUnitTest
    {
        #region Private variables
       
        IGenericRepository<RiskDocumentCategory> _mockDocumentCategoryRepository = null;
        IGenericRepository<RiskDriver> _mockRiskDriverRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskBilling> _mockBillingRepository = null;
   
        IDocumentGeneratorService _documentGeneratorService = null;

      
        #endregion

        [TestInitialize]
        public void Setup()
        {
           
            _mockDocumentCategoryRepository = new MockGenericRepository<RiskDocumentCategory>(DomainBuilder.GetRiskDocumentCategories()).SetUpRepository();

            _mockRiskDriverRepository = new MockGenericRepository<RiskDriver>(DomainBuilder.GetRiskDrivers()).SetUpRepository();

            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();

           
            _mockBillingRepository = new MockGenericRepository<RiskBilling>(DomainBuilder.GetRiskBillings()).SetUpRepository();

            //Call This Service
            _documentGeneratorService = new DocumentGeneratorService(
                                            _mockDocumentCategoryRepository,
                                            _mockRiskDriverRepository, _mockClaimRepository,
                                             _mockBillingRepository);

        }

        [TestMethod]
        public void Test_method_for_get_all_document_types()
        {
            var documentCategoriesDto =  _documentGeneratorService.GetAllDocumentTypesAsync().Result;


            Assert.IsNotNull(documentCategoriesDto);
        }

        [TestMethod]
        public void Test_method_for_get_drivers_by_claim()
        {
            long claimId = Convert.ToInt64(_mockRiskDriverRepository.AsQueryable.Select(x => x.ClaimId).Max());
            var driversDto =  _documentGeneratorService.GetDriversByClaimAsync(claimId).Result;


            Assert.IsNotNull(driversDto);
        }

        [TestMethod]
        public void Test_method_for_get_policy_number_by_driver_id()
        {
            long driverId = _mockRiskDriverRepository.AsQueryable.Select(x => x.Id).Max();

            var policyNumber =  _documentGeneratorService.GetPolicyNumberByDriverId(driverId).Result;


            Assert.IsNotNull(policyNumber);
        }

        [TestMethod]
        public void Test_method_for_get_total_due_with_all_risk_billings()
        {
            long[] sellectedbillings = _mockClaimRepository.AsQueryable.Select(x => x.Id).ToArray();
           
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var totalDue =  _documentGeneratorService.GetTotalDueAsync(sellectedbillings, claimId).Result;


            Assert.IsTrue(totalDue>0);
        }

        [TestMethod]
        public void Test_method_for_get_total_due_with_no_risk_billings()
        {
            long[] sellectedbillings = null;

            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var totalDue =  _documentGeneratorService.GetTotalDueAsync(sellectedbillings, claimId).Result;


            Assert.IsTrue(totalDue==0);
        }

        [TestMethod]
        public void Test_method_for_get_original_balance_with_all_risk_billings()
        {
            long[] sellectedbillings = _mockBillingRepository.AsQueryable.Select(x => x.Id).ToArray();

            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var totalDue =  _documentGeneratorService.GetOriginalBalanceAsync(sellectedbillings, claimId).Result;


            Assert.IsNotNull(totalDue);
        }

        [TestMethod]
        public void Test_method_for_get_original_balance_with_no_risk_billings()
        {
            long[] sellectedbillings = null;

            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var totalDue =  _documentGeneratorService.GetOriginalBalanceAsync(sellectedbillings, claimId).Result;


            Assert.IsNotNull(totalDue);
        }

        [TestMethod]
        public void Test_method_for_get_document_header_info()
        {
            long claimId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            long driverId = _mockClaimRepository.AsQueryable.Where(z => z.Id == claimId).Select(x => x.RiskDrivers.Select(y => y.Id).Max()).FirstOrDefault();            

            var documentHeaderDto =  _documentGeneratorService.GetDocumentHeaderInfoAsync(claimId, driverId).Result;

            Assert.IsNotNull(documentHeaderDto);
        }

        [TestMethod]
        public void Test_method_for_get_date_of_last_payment()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();


            var lastPaymentDate =  _documentGeneratorService.GetDateOfLastPaymentAsync(claimId).Result;


            Assert.IsNotNull(lastPaymentDate);
        }

        [TestMethod]
        public void Test_method_for_get_actual_cash_value_for_claim()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var amount =  _documentGeneratorService.GetActualCashValueForClaim(claimId).Result;

            Assert.IsNotNull(amount);
        }

        [TestMethod]
        public void Test_method_for_get_estimated_billing()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var estimatedBill =  _documentGeneratorService.GetEstimatedBilling(claimId).Result;

            Assert.IsTrue(estimatedBill>0);
        }

        [TestMethod]
        public void Test_method_get_incident_by_specific_claim_id()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var incidentDto =  _documentGeneratorService.GetIncidentByClaimIdAsync(claimId).Result;

            Assert.IsNotNull(incidentDto);
        }

        [TestMethod]
        public void Test_method_for_get_selected_billings_by_ids()
        {
            long[] sellectedbillings = _mockBillingRepository.AsQueryable.Select(x => x.Id).ToArray();

            var billingDtoList =  _documentGeneratorService.GetSelectedBillingsByIdsAsync(sellectedbillings).Result;

            Assert.IsNotNull(billingDtoList);

        }

        [TestMethod]
        public void Test_method_for_get_insurance_info_by_driver_id()
        {
            long driverId = _mockRiskDriverRepository.AsQueryable. Select(x => x.Id).Max();

            var insuranceDto =  _documentGeneratorService.GetInsuranceInfoByDriverId(driverId).Result;

            Assert.IsNotNull(insuranceDto);

        }

        [TestMethod]
        public void Test_method_for_get_sli_policy_number_by_claim_id()
        {
            long claimId =  _mockClaimRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var sliPolicyNumber =  _documentGeneratorService.GetSliPolicyNumberByClaimId(claimId).Result;

            Assert.IsNotNull(sliPolicyNumber);

        }

        [TestMethod]
        public void Test_method_for_get_salvage_bill_by_claim_id()
        {
            long? claimId =  _mockBillingRepository.AsQueryable.Select(x => x.ClaimId).FirstOrDefault();

            double total = 0;

            if(claimId.HasValue)
            {
                total =  _documentGeneratorService.GetSalvageBillByClaimIdAsync(claimId.Value).Result;
            }            

            Assert.IsTrue(total==0);
        }
    }
}
