using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;

namespace RateShopper.Domain.DTOs
{
    public class SearchViewAppliedRuleSetDTO
    {
        public string CompaniesID { get; set; }
        public string CompanyCode { get; set; }
        public string CarClassID { get; set; }
        public string CarClassCode { get; set; }
        public string RentalLengthID { get; set; }
        public string RentalLengthName { get; set; }
        public string DaysOfWeekID { get; set; }
        public string DaysOfWeek { get; set; }
        public List<Company> lstCompany { get; set; }
        public List<RuleSetGroupCustomDTO> lstRuleSetGroupCustom { get; set; }
        
    }
    public class RuleSetGroupCustomDTO
    {
        public long ID { get; set; }
        public string GroupName { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public List<GroupCompany> lstGroupCompany { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingDay { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingWeek { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingMonth { get; set; }
    }
    public class GroupCompany
    {
        public long CompanyID { get; set; }
        public bool IsBrand { get; set; }
        public string CompanyName { get; set; }
    }
    public class RuleSetGapSettingCustomDTO
    {
        public long ID { get; set; }
        public long RuleSetGroupID { get; set; }
        public long RangeIntervalID { get; set; }
        public string RangeName { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set;}
        public string GapAmount { get; set; }
        public string BaseMinAmount { get; set; }
        public string BaseMaxAmount { get; set; }
        public string BaseGapAmount { get; set; }
    }
    public class RuleSetTemplate
    {
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingDay { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingWeek { get; set; }
        public List<RuleSetGapSettingCustomDTO> lstRuleSetGapSettingMonth { get; set; }
    }
    public class UpdateRuleSet
    {
        public string RuleSetCompanyIDs { get; set; }
        public List<RuleSetGroupCustomDTO> lstRuleSetGroupCustomDTO { get; set; }
    }
}
