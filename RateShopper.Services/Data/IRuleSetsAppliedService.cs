using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IRuleSetsAppliedService : IBaseService<RuleSetsApplied>
    {
        List<LocationBrandModel> GetLocationBrands();
        List<AppliedRuleSetsDTO> GetAppliedRuleSets(long locationBrandId);
        List<AppliedRuleSetDetailsDTO> GetAppliedRuleSetDetails(long locationBrandId);
        void ApplyRuleSets(long rulesetId, DateTime startDate, DateTime endDate, long userId);
    }
}
