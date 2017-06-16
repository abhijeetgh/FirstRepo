using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class BillingService : IBillingService
    {
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskBilling> _billingRepository = null;
        IGenericRepository<RiskAdminCharge> _adminChargesRepository = null;
        IPaymentService _paymentService = null;
        IGenericRepository<RiskWriteOff> _riskWriteOffRepository = null;
        ILookUpService _lookUpService = null;

        public BillingService(IGenericRepository<Claim> claimRepository,
                              IGenericRepository<RiskBilling> billingRepository,
                                IGenericRepository<RiskAdminCharge> adminChargesRepository,
                                  IPaymentService paymentService,
                            ILookUpService lookUpService,
            IGenericRepository<RiskWriteOff> riskWriteOffRepository)
        {
            _claimRepository = claimRepository;
            _billingRepository = billingRepository;
            _adminChargesRepository = adminChargesRepository;
            _paymentService = paymentService;
            _lookUpService = lookUpService;
            _riskWriteOffRepository = riskWriteOffRepository;
        }

        public async Task<BillingsDto> GetBillingsByClaimIdAsync(long id)
        {
            BillingsDto billings = null;
            Claim claim = await _claimRepository.AsQueryable.Include(c => c.RiskBillings.Select(x => x.RiskBillingType)).Where(c => c.Id == id).FirstOrDefaultAsync();
            IEnumerable<RiskBilling> riskBilling = await _billingRepository.AsQueryable.Where(c => c.ClaimId == id).ToListAsync();

            if (claim != null)
            {

                billings = GetBillingsDto(claim, riskBilling);
            }

            return billings;

        }

        private BillingsDto GetBillingsDto(Claim claim, IEnumerable<RiskBilling> riskBilling)
        {
            BillingsDto billings = null;

            if (claim != null && claim.RiskBillings != null)
            {
                billings = new BillingsDto();
                billings.Billings = MapBillingDto(claim.RiskBillings);
                billings.ClaimId = claim.Id;

                //billings.TotalBilling = claim.TotalBilling.HasValue ? claim.TotalBilling.Value : default(double);
                billings.TotalBilling = claim.RiskBillings.Any() ? claim.RiskBillings.Select(x => x.Amount).Sum() : default(double);

                billings.TotalDue = _lookUpService.GetTotalDue(claim.TotalBilling, claim.TotalPayment);// claim.TotalBilling.Value - (claim.TotalPayment.HasValue ? claim.TotalPayment.Value : default(double));
            }

            return billings;
        }

        private IEnumerable<RiskBillingDto> MapBillingDto(IList<RiskBilling> riskBilling)
        {
            List<RiskBillingDto> billings = null;

            if (riskBilling != null)
            {
                billings = new List<RiskBillingDto>();

                billings = riskBilling.Select(x => new RiskBillingDto
                {
                    Id = x.Id,
                    BillingTypeId = x.BillingTypeId,
                    Amount = x.Amount,
                    BillingTypeDesc = x.RiskBillingType.Description,
                    Discount = x.Discount,
                    ClaimId = x.ClaimId,
                    SubTotal = x.Discount.HasValue ? x.Amount * (1 - x.Discount.Value / 100) : x.Amount
                }).ToList();
            }
            return billings;
        }

        public async Task<bool> AddOrUpdateBillingToClaimAsync(RiskBillingDto billingDto)
        {
            bool isSuccess = false;
            if (billingDto != null)
            {
                List<RiskBilling> riskBillings = null;

                Claim claim = await _claimRepository.AsQueryable.Include(x => x.RiskBillings).Where(c => c.Id == billingDto.ClaimId).FirstOrDefaultAsync();

                if (claim != null)
                {
                    riskBillings = GetUpdatedRiskBillingList(claim.RiskBillings.ToList(), billingDto);

                    var WriteOffData = _riskWriteOffRepository.AsQueryable.Where(x => x.ClaimId == billingDto.ClaimId).Select(x => x.Amount);

                    double? totalWriteOff = WriteOffData.Any() ? WriteOffData.Sum().Value : default(double);

                    claim.TotalBilling = GetTotalBilling(riskBillings) - totalWriteOff;

                    claim.RiskBillings = riskBillings;

                    await _claimRepository.UpdateAsync(claim);

                    await _paymentService.UpdateCdwLdwAndLpc2(claim.Id);

                    isSuccess = true;
                }
            }

            return isSuccess;

        }

        private List<RiskBilling> GetUpdatedRiskBillingList(List<RiskBilling> riskBillings, RiskBillingDto billingDto)
        {


            riskBillings = AddOrUpdateBillingList(riskBillings, billingDto);


            if (billingDto.AutoAdminFeeCalculate && billingDto.BillingTypeId == (int)Constants.BillingTypes.Estimate)
            {
                RiskBillingDto adminBillingDto = GetCalculatedAdminFeeBilling(billingDto.Amount);

                riskBillings = AddOrUpdateBillingList(riskBillings, adminBillingDto);



            }
            return riskBillings;
        }

        private List<RiskBilling> AddOrUpdateBillingList(List<RiskBilling> riskBillings, RiskBillingDto billingDto)
        {
            RiskBilling riskBilling = riskBillings.Where(x => x.BillingTypeId == billingDto.BillingTypeId).FirstOrDefault();


            if (riskBilling == null)
            {
                riskBilling = new RiskBilling();
            }
            else
            {
                riskBillings.Remove(riskBilling);
            }
            riskBilling = MapBillingDomain(billingDto, riskBilling);

            riskBillings.Add(riskBilling);

            return riskBillings;
        }

        private double GetTotalBilling(List<RiskBilling> riskBilling)
        {

            double totalAmount = default(double);

            foreach (var billing in riskBilling)
            {
                double subTotal = billing.Discount.HasValue ? billing.Amount * (1 - billing.Discount.Value / 100) : billing.Amount;

                totalAmount += subTotal;

            }


            return totalAmount;
        }

        private RiskBilling GetNewOrUpdatedBilling(RiskBillingDto billingDto, Claim claim)
        {
            RiskBilling riskBilling = claim.RiskBillings.Where(x => x.BillingTypeId == billingDto.BillingTypeId || x.Id == billingDto.Id).FirstOrDefault();

            if (riskBilling == null)
            {
                riskBilling = new RiskBilling();
            }

            riskBilling = MapBillingDomain(billingDto, riskBilling);

            return riskBilling;
        }

        private RiskBillingDto GetCalculatedAdminFeeBilling(double estimateAmount)
        {
            RiskBillingDto riskBilling = null;

            Nullable<double> adminCharge = _adminChargesRepository.AsQueryable.Where(x => x.EstimatedAmount < estimateAmount).Sum(x => x.AdminAmount);

            if (adminCharge.HasValue)
            {
                riskBilling = new RiskBillingDto();
                riskBilling.Amount = adminCharge.Value;
                riskBilling.Discount = default(double);
                riskBilling.BillingTypeId = (int)Constants.BillingTypes.AdminChange;
            }

            return riskBilling;
        }

        private RiskBilling MapBillingDomain(RiskBillingDto billingDto, RiskBilling riskBilling)
        {

            if (billingDto != null && riskBilling != null)
            {
                riskBilling.Amount = billingDto.Amount;
                riskBilling.Discount = billingDto.Discount;
                riskBilling.BillingTypeId = billingDto.BillingTypeId;
            }
            return riskBilling;
        }

        public async Task<bool> DeleteBillingByIdAsync(Int64 id)
        {
            bool isSuccess = false;

            long claimId;

            RiskBilling riskBilling = await _billingRepository.AsQueryable.Include(x => x.Claim).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (riskBilling != null && riskBilling.Claim != null)
            {
                claimId = riskBilling.Claim.Id;
                double SubTotal = riskBilling.Discount.HasValue ? riskBilling.Amount * (1 - riskBilling.Discount.Value / 100) : riskBilling.Amount;

                riskBilling.Claim.TotalBilling = (riskBilling.Claim.TotalBilling.HasValue ? riskBilling.Claim.TotalBilling.Value : default(double)) - SubTotal;

                await _billingRepository.DeleteAsync(riskBilling);

                await _paymentService.UpdateCdwLdwAndLpc2(claimId);

                isSuccess = true;
            }

            return isSuccess;
        }
    }
}
