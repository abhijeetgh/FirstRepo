using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IClaimService
    {

        Task<IEnumerable<ClaimDto>> GetClaimsByCriteria(SearchClaimsCriteria claimsCriteria);

        Task<IList<ClaimDto>> SearchClaimsByContractNumberAsync(string contractNumber);

        Task<bool> SearchClaimByClaimIdAsync(long claimId);

        Task<bool> SetFollowUpdateByClaimIdAsync(long claimId, DateTime followupDate, long updatedBy);

        Task<bool> SetAssignedTo(long claimId, long userId, long updatedBy);

        Task<int> GetFollowUpCountList(long userId);

        Task<bool> SetFollowUpCompletedAsync(long claimId, bool isCompleted);

        Task<Nullable<Int64>> CreateClaimAsync(ClaimDto createClaimRequest, long createdBy);

        Task<bool> SaveClaimHeaderInfo(ClaimDto claimResponse, long updatedBy);

        Task<ClaimBasicInfoDto> GetClaimBasicInfoByClaimIdAsync(long claimId);

        Task<ClaimDto> GetClaimInfoByClaimIdAsync(long claimId);

        Task<List<DriverInfoDto>> GetDriverAndIncInfoByClaimIdAsync(int claimNumber);

        Task<IEnumerable<DamageDto>> GetDamagesInfoByClaimIdAsync(long claimNumber);

        Task<List<PicturesAndFilesDto>> GetFilesInfoByClaimIdAsync(int claimNumber);

        Task<List<BillingsDto>> GetBillingsInfoByClaimIdAsync(int claimNumber);

        Task<List<PaymentDto>> GetPaymentsInfoByClaimIdAsync(int claimNumber);

        Task<SalvageDto> GetSalvageInfoByClaimIdAsync(int claimNumber);

        Task<bool> DeleteClaims(List<Int64> claimsToDelete);

        Task<int> GetPendingApprovedCountList(long userId, ClaimType type);

        Task<bool> ApproveOrRejectClaims(long claimId, bool status);

        Task<IEnumerable<ClaimDto>> GetPendingApprovedClaimsByCriteria(SearchClaimsCriteria claimsCriteria);

        Task<Nullable<Int64>> UpdateClaimInfoAsync(ClaimDto claimDto, long updatedBy);

        Task<ContractDto> GetContractInfoByIdAsync(Int64 id);

        Task<Nullable<Int64>> UpdateContractInfoAsync(ContractDto contractDto, Int64 claimId);

        Task<ClaimDto> GetClaimInfoForHeaderAsync(Int64 claimId);

        Task<VehicleDto> GetVehicleInfoByIdAsync(Int64 id, string contractNumber);

        Task<Nullable<Int64>> AddOrUpdateVehicleInfoAsync(VehicleDto vehicleDto, Int64 claimId);

        Task<IncidentDto> GetIncidentInfoByIdAsync(Int64 incidentId);

        Task<Nullable<Int64>> UpdateIncidentInfoAsync(IncidentDto incidentDto);

        Task<bool> AddDamage(DamageDto damageDto);

        Task<bool> DeleteDamage(DamageDto damageDto);

        Task DownloadContractInfo(Int64 claimId, string contractNumber);

        Task<Nullable<Int64>> DownloadVehicleInfo(Int64 claimId, string unitNumber);

        Task<bool> SaveSalvageInfo(long claimNumber, Nullable<decimal> amount, Nullable<DateTime> dateOfReceipt);

        Task<IList<ClaimDto>> GetAdvancedSearchRecords(ClaimSearchDto claimSearchDto, SearchClaimsCriteria claimsCriteria);

        Task<int> GetAdvancedSearchRecordCount(ClaimSearchDto claimSearchDto);

        Task<bool> UpdateLabourHour(Nullable<double> labourHour, long claimId);

        Task<Nullable<double>> GetLabourHoursByClaimIdAsync(long id);

        Task<bool> DeleteClaimFromViewClaim(long id, long user);

        Task<CompanyDto> GetCompanyByClaimIdAsync(long id);

        Task<ContractDto> GetContractByClaimIdAsync(long id);

        Task<ContractDto> GetContractInfoByIdWithoutCreditCardDetailsAsync(Int64 id);

        bool IsClaimCreator(long userId,long claimId);

        Task<NonContractDto> GetNonContractInfoByIdAsync(long nonContractId);

        Task<Nullable<long>> UpdateNonContractInfoAsync(Dtos.NonContractDto nonContractDto, long p);

        Task<string> GenerateNonContractNumber(long claimId);
    }
}
