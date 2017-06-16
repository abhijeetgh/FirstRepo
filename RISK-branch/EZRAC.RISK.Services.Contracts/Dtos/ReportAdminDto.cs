using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportAdminDto
    {
        public IEnumerable<ReportPaymentDto> ReportPaymentDtos { get; set; }
        public IEnumerable<ReportClaimDto> ReportClaimDtos { get; set; } //report view model Dto
        public IEnumerable<ClaimDto> ClaimDtos { get; set; } //report master claim view model Dto
        public IEnumerable<UserActionLogDto> UserActionLogDtos { get; set; }
    }
}
