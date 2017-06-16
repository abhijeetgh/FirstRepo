using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IRuleSetService : IBaseService<RuleSet>
    {
        List<LocationBrandModel> GetBrandLocation();
        List<RuleSetDTO> GetRuleSet();
        List<RuleSetDTO> GetAutomationRuleSet(long LocationBrandID, bool IsWideGap, long ScheduledJobID, bool IsGov);
        RuleSetTemplate GetRuleSetDefaultSetting();
        UpdateRuleSet GetSelectedRuleSetData(long ruleSetID);
        RuleSetTemplateDTO CreateRuleSet(List<RuleSetGroupDTO> lstRuleSetGroupDTO, RuleSetTemplateDTO RuleSetTemplate, string RuleSetGroupNameDB, decimal RuleSetGroupMaxValue);
        RuleSetTemplateDTO UpdateRuleSet(List<RuleSetGroupDTO> lstRuleSetGroupDTO, RuleSetTemplateDTO RuleSetTemplate, string RuleSetGroupNameDB, decimal RuleSetGroupMaxValue);
        bool DeletRuleSet(long RuleSetID, long LoggedInUserId);
        void DeleteAutomationRuleSet(long ScheduledJobId);
        void updateAutomationRuleSet(long ScheduledJobId, string IntermediateId);
        void DeleteDirectCompetitorRuleSet(long LocationBrandID, string DeletedCompanyID);
    }
}
