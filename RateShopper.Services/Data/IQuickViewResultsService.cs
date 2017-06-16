using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface IQuickViewResultsService : IBaseService<QuickViewResults>
    {
        QuickViewReportDTO GetQuickViewResult(long QuickViewId,long SearchSummaryId);
       // QuickViewDTO GetQuickViewResult(long QuickViewId,long SearchSummaryId);
		QuickViewLengthDateCombinationDTO GetlengthDateCombination(long SearchSummaryID);

    }
}
