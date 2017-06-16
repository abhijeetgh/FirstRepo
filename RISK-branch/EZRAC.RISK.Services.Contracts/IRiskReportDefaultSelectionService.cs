using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.ReportDtos;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IRiskReportDefaultSelectionService
    {
        IEnumerable<RiskReportDefaultSelectionDto> GetReportDefaultSelectionAsync(string ReportKey, string property);
    }
}
