using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface ILookUpService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();

        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int Id);

        Task<IEnumerable<UserDto>> GetUsersByRolesAsync(List<Int64> roleIds);

        Task<IEnumerable<UserDto>> GetAssignToUsersAsync();

        Task<UserDto> GetUserbyIdAsync(Int64 Id);

        Task<IEnumerable<LossTypesDto>> GetAllLossTypesAsync();

        Task<IEnumerable<ClaimStatusDto>> GetAllClaimStatusesAsync();

        Task<IEnumerable<ClaimDto>> GetClaimsByContractNumberOrUnitNumber(string contractNumber,string unitNumber);

        Task<IEnumerable<PoliceAgencyDto>> GetAllPoliceAgenciesAsync();

        Task<IEnumerable<InsuranceDto>> GetAllInsuranceCompaniesAsync();

        Task<IEnumerable<BillingTypeDto>> GetBillingTypesAsync();

        Task<IEnumerable<PaymentTypesDto>> GetAllPaymentTypesAsync();

        Task<IEnumerable<NotesTypeDto>> GetAllNoteTypesAsync();

        Task<IEnumerable<DamageTypesDto>> GetAllDamageTypesAsync();

        Task<List<FileCategoriesDto>> GetAllFileCategoriesAsync();

        Task<IEnumerable<string>> GetAllUserEmailsAsync();

        Task<LocationDto> GetLocationByCodeAsync(string locationCode);


        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();

        Task<IEnumerable<WriteOffTypeDTO>> GetAllWriteOffTypesAsync();

        double GetTotalDue(double? TotalBilling, double? TotalPayment);
    }
}
