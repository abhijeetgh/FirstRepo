using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    class MangeRuleSetDTO
    {

    }
    public class RuleSetGroupDTO
    {
        public long RuleSetGroupID { get; set; }
        public long DeleteRuleSetGroupID { get; set; }
        public string CompanyIDs { get; set; }
        public string AddCompanyIDs { get; set; }
        public string DeleteCompanyIDs { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingDay { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingWeek { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingMonth { get; set; }
        //This entity use in update case when user add another rule
        public List<RuleSetGapSettingCustomDTO> AddlstRuleSetGapSettingDay { get; set; }
        public List<RuleSetGapSettingCustomDTO> AddlstRuleSetGapSettingWeek { get; set; }
        public List<RuleSetGapSettingCustomDTO> AddlstRuleSetGapSettingMonth { get; set; }
    }
    public class RuleSetTemplateDTO
    {
        public long ID { get; set; }
        public bool IsAutomationRuleSet { get; set; }
        public long ScheduledJobID { get; set; }
        public long ScheduledJobRulesetID { get; set; }
        public int OriginalRuleSetID { get; set; }
        public long LoggedInUserID { get; set; }
        public string RuleSetName { get; set; }
        public long LocationBrandID { get; set; }
        public long AddLocationBrandID { get; set; }

        public string RuleSetLengths { get; set; }
        public string AddRuleSetLengths { get; set; }
        public string DeleteRuleSetLengths { get; set; }

        public string RuleSetCompanies { get; set; }
        public string AddRuleSetCompanies { get; set; }
        public string DeleteRuleSetCompanies { get; set; }

        public string RuleSetCarClasses { get; set; }
        public string AddRuleSetCarClasses { get; set; }
        public string DeleteRuleSetCarClasses { get; set; }

        public string RuleSetDayOfWeeks { get; set; }
        public string AddRuleSetDayOfWeeks { get; set; }
        public string DeleteRuleSetDayOfWeeks { get; set; }

        public bool IsWideGapTemplate { get; set; }
        public bool IsPositionOffset { get; set; }
        public int CompanyPositionAbvAvg { get; set; }
        public string IntermediateID { get; set; }
        public bool IsGOV { get; set; }
    }
}
