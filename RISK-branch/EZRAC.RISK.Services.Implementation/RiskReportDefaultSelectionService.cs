using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.SqlClient;


namespace EZRAC.RISK.Services.Implementation
{
    public class RiskReportDefaultSelectionService : IRiskReportDefaultSelectionService
    {
        IGenericRepository<RiskReportDefaultselection> _riskReportDefaultselectionRepository = null;

        public RiskReportDefaultSelectionService(IGenericRepository<RiskReportDefaultselection> riskReportDefaultselectionRepository)
        {
            _riskReportDefaultselectionRepository = riskReportDefaultselectionRepository;
        }

        public IEnumerable<RiskReportDefaultSelectionDto> GetReportDefaultSelectionAsync(string reportKey, string property)
        {
            IEnumerable<RiskReportDefaultSelectionDto> riskReportDefaultSelectionDto = null;

            riskReportDefaultSelectionDto = _riskReportDefaultselectionRepository.AsQueryable.Where(x => x.ReportKey == reportKey && x.Property == property).Select(x => new RiskReportDefaultSelectionDto()
            {
                Property = x.Property,
                PropId = x.PropId,
                ReportKey = x.ReportKey,
                IsSelected = x.IsSelected
            }).ToList();

            return riskReportDefaultSelectionDto;
        }
    }
}
