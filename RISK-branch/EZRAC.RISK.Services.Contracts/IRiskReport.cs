using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using System.Data;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IRiskReport
    {
		Task<IEnumerable<RiskReportCategoryDto>> GetReportListAsync(long LoggedUsedId);
		Task<ReportAdminDto> GetAdminReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<VehicleToBeReleasedReportDto>> GetVahicleToBeReleasedReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<WriteOffCustomReportDto>> GetWriteOffReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<AccountReceivableCustomReportDto>> GetAccountReceivableReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<ARAgingCustomReportDto>> GetARAgingReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<ChargeBackLossCustomReportDto>> GetChargeBackLossCustomReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<VehicleDamageSectionCustomReportDto>> GetVehicleDamageSectionCustomReportDtoAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<BasicReportDtos>> GetBasicReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<DepositDateReportDto>> GetDepositDateReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<ChargeTypeReportDto>> GetChargeTypeReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<StolenRecoveredReportDto>> GetStolenRecoveredReportsAsync(ReportCriteriaDto reportCriteriaDto);
		Task<AdminFeeCommissionReportDto> GetAdminFeeCommissionReportAsync(ReportCriteriaDto reportCriteriaDto);
		Task<IEnumerable<CollectionCustomReportDto>> GetCollectionReportsAsync(ReportCriteriaDto reportCriteriaDto);
        Task<DataTable> GetAllClaimExport(string fromDate, string toDate, string exportType);
        bool IsAuthorizedForReports(long LoggedUsedId);
    }
}
