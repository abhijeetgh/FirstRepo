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
    public class PaymentService :IPaymentService
    {
        IGenericRepository<RiskPayment> _paymentRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskPaymentType> _paymentTypesRepository = null;
        IGenericRepository<RiskPaymentReason> _paymentReasonRespository = null;

        public PaymentService(IGenericRepository<Claim> claimRepository, IGenericRepository<RiskPayment> paymentRepository, IGenericRepository<RiskPaymentType> paymentTypesRepository, IGenericRepository<RiskPaymentReason> paymentReasonRespository)
        {
            _claimRepository = claimRepository;
            _paymentRepository = paymentRepository;
            _paymentTypesRepository = paymentTypesRepository;
            _paymentReasonRespository = paymentReasonRespository;
        }

        /// <summary>
        /// This methods returns all the Payment Types
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentTypesDto>> GetAllPaymentTypesAsync()
        {
            List<PaymentTypesDto> paymentTypeList = new List<PaymentTypesDto>();
            IEnumerable<RiskPaymentType> paymentTypes = await _paymentTypesRepository.GetAllAsync();

            PaymentTypesDto paymentType = null;

            foreach (var item in paymentTypes)
            {
                paymentType = new PaymentTypesDto();
                paymentType.Id = item.Id;
                paymentType.PaymentType = item.PaymentFromType;

                paymentTypeList.Add(paymentType);
            }
            
            return paymentTypeList;
        }
        /// <summary>
        /// This method returns Payments for particular claim.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns></returns>
        public async Task<PaymentDto> GetAllPaymentsByClaimId(long claimNumber)
        {
            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskPayments.Select(y => y.RiskPaymentType), y => y.RiskPayments.Select(z => z.RiskPaymentReason)).Where(x => x.Id == claimNumber).FirstOrDefaultAsync();
            var paymentList = MapPaymentsDto(claim);

            return paymentList;
        }

        /// <summary>
        /// This method is used to add payment for a particular claim
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<bool> AddPaymentInfo(PaymentInfoDto payment)
        {
            var success = false;
            if (payment != null)
            {
                var riskPayment = new RiskPayment()
                    {
                        Amount = payment.Amount,
                        ClaimId = payment.ClaimId,
                        PaymentDate = payment.ReceivedDate,
                        PaymentTypeId = payment.PaymentTypeId,
                        ReasonId=payment.ReasonId                        
                    };
                  await _paymentRepository.InsertAsync(riskPayment);
                  success = true;
            }

            var claim = await _claimRepository.AsQueryable.Where(x => x.Id == payment.ClaimId).FirstOrDefaultAsync();
            if (claim != null)
            {
                claim.TotalPayment = (claim.TotalPayment.HasValue ? claim.TotalPayment.Value : default(double)) + payment.Amount;
                await _claimRepository.UpdateAsync(claim);
            }
            return success;
        }

        /// <summary>
        /// This method is used to delete payment of particular Claim.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<bool> DeletePaymentInfo(PaymentInfoDto payment)
        {
            var success = false;
            if (payment != null)
            {
               var deletePayment = await _paymentRepository.AsQueryable.Include(x=>x.Claim).Where(x => x.Id == payment.PaymentId).FirstOrDefaultAsync();

               if (deletePayment != null)
               {
                   deletePayment.Claim.TotalPayment = (deletePayment.Claim.TotalPayment.HasValue ? deletePayment.Claim.TotalPayment.Value : default(double)) - deletePayment.Amount;
                   await _paymentRepository.DeleteAsync(deletePayment);
               }
               success = true; 
            }

            return success;
        }

        public async Task<bool> UpdateCdwLdwAndLpc2(long claimId)
        {
            bool isSuccess = false;

            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskContract,
                                                                             x => x.RiskPayments)
                                                             .Where(x => x.Id == claimId)
                                                             .FirstOrDefaultAsync();

            bool isCdwValid = (claim != null && claim.RiskContract != null && claim.RiskContract.CDW && !claim.RiskContract.CDWVoid) ? true : false;
            bool isLdwValid = (claim != null && claim.RiskContract != null && claim.RiskContract.LDW && !claim.RiskContract.LDWVoid) ? true : false;
            bool Lpc2 = (claim != null && claim.RiskContract != null && claim.RiskContract.LPC2) ? true : false;

            await DeletePayment(claim, (int)Constants.PaymentTypes.Cdw);
            await DeletePayment(claim, (int)Constants.PaymentTypes.Ldw);
            await DeletePayment(claim, (int)Constants.PaymentTypes.Lpc2);

            if ((isCdwValid || isLdwValid) && claim.TotalBilling.HasValue)
            {
                int paymentTypeId = isCdwValid ? (int)Constants.PaymentTypes.Cdw : (int)Constants.PaymentTypes.Ldw;

                claim.RiskPayments = InsertCdwAndLdw(claim.RiskPayments, claim.TotalBilling.Value, paymentTypeId);

                claim.TotalPayment = GetTotalPayment(claim.RiskPayments);

                await _claimRepository.UpdateAsync(claim);

            }
            else if (Lpc2 && claim.TotalBilling.HasValue)
            {
                int paymentTypeId = isCdwValid ? (int)Constants.PaymentTypes.Lpc2 : (int)Constants.PaymentTypes.Lpc2;

                double lpc2Amount = Convert.ToDouble(ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.LPC2Deductible));

                double paymentAmount = claim.TotalBilling.Value < lpc2Amount ? claim.TotalBilling.Value : lpc2Amount;

                claim.RiskPayments = InsertCdwAndLdw(claim.RiskPayments, paymentAmount, paymentTypeId);

                claim.TotalPayment = GetTotalPayment(claim.RiskPayments);

                await _claimRepository.UpdateAsync(claim);

            }
            else {

                await _claimRepository.UpdateAsync(claim);
            }


            isSuccess = true;

            return isSuccess;
        }

        public async Task<Nullable<double>> GetTotalPayments(long claimId)
        {
            Nullable<double> amount = null;
            amount = _paymentRepository.AsQueryable.Where(x => x.ClaimId == claimId).Sum(x => x.Amount);
            return amount;
        }

        public async Task<bool> IsPaymentValid(long claimId,double paymentValue)
        {
            var isValid = false;
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim.TotalBilling != null)
            {
                isValid = claim.TotalBilling - ((claim.TotalPayment.HasValue ? claim.TotalPayment.Value : default(double)) + paymentValue) < 0 ? false : true;
            }
            return isValid;
        }

        public async Task<IEnumerable<RiskPaymentReasonDto>> GetAllPaymentReasonsAsync()
        {
            List<RiskPaymentReasonDto> paymentReasonList = new List<RiskPaymentReasonDto>();
            IEnumerable<RiskPaymentReason> paymentReasons = await _paymentReasonRespository.GetAllAsync();

            RiskPaymentReasonDto paymentReason = null;

            foreach (var item in paymentReasons)
            {
                paymentReason = new RiskPaymentReasonDto();
                paymentReason.Id = item.Id;
                paymentReason.Reason = item.Reason;

                paymentReasonList.Add(paymentReason);
            }

            return paymentReasonList;
        }

        #region Private Methods
        private PaymentDto MapPaymentsDto(Claim claim)
        {
            PaymentDto paymentDto=null;

            if (claim!=null)
	        {
		        paymentDto=new PaymentDto();
                paymentDto.ClaimId=claim.Id;
                paymentDto.TotalBilling = claim.TotalBilling;
                paymentDto.TotalPayment = claim.TotalPayment;
                paymentDto.Payments = MapPaymentsInfoDto(claim.RiskPayments);
                
            }
            return paymentDto;
        }
         private IList<PaymentInfoDto> MapPaymentsInfoDto(IEnumerable<RiskPayment> payments)
        {

            var paymentsInfoDtoList = payments.Select(
               x => new PaymentInfoDto
               {
                   Amount = x.Amount,
                   ClaimId = x.ClaimId,
                   PaymentId = x.Id,
                   PaymentFrom = x.RiskPaymentType.PaymentFromType,
                   SelectedReason = x.RiskPaymentReason != null ? x.RiskPaymentReason.Reason : "-",
                   PaymentTypeId = x.PaymentTypeId,
                   ReasonId = x.ReasonId,
                   ReceivedDate = (DateTime)x.PaymentDate
               });
          
            return paymentsInfoDtoList.ToList();
        }

         private async Task DeletePayment(Claim claim, int paymentTypeId)
         {
             RiskPayment riskPayment = claim.RiskPayments.Where(x => x.PaymentTypeId == paymentTypeId).FirstOrDefault();

             if (riskPayment != null)
             {
                 await _paymentRepository.DeleteAsync(riskPayment);

                 claim.RiskPayments.Remove(riskPayment);

                 claim.TotalPayment = GetTotalPayment(claim.RiskPayments);
             }

         }

         private double GetTotalPayment(IList<RiskPayment> riskPayments)
         {
             double totalAmount = default(double);

             foreach (var payment in riskPayments)
             {
                 totalAmount += payment.Amount.HasValue ? payment.Amount.Value : default(double);
             }

             return totalAmount;
         }

         private IList<RiskPayment> InsertCdwAndLdw(IList<RiskPayment> riskPayments, double totalBilling, int paymentTypeId)
         {
             RiskPayment riskPayment = riskPayments.Where(x => x.PaymentTypeId == paymentTypeId).FirstOrDefault();

             if (riskPayment == null)
                 riskPayment = new RiskPayment();
             else
                 riskPayments.Remove(riskPayment);

             riskPayment.PaymentTypeId = paymentTypeId;
             riskPayment.PaymentDate = DateTime.Now;
             riskPayment.Amount = totalBilling;

             riskPayments.Add(riskPayment);

             return riskPayments;
         }
        #endregion


    }
}
