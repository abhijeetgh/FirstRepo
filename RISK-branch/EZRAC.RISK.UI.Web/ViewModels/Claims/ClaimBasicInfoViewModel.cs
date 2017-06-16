using EZRAC.Risk.UI.Web.ViewModels.ClaimBasicInfo;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ClaimBasicInfoViewModel
    {

        public ClaimInfoForViewEditViewModel ClaimInfo { get; set; }
        public NonContractInfoViewModel NonContractInfo { get; set; }
        public ContractInfoViewModel ContractInfo { get; set; }
        public VehicleInfoViewModel VehicleInfo { get; set; }
        public IncidentInfoViewModel IncidentInfo { get; set; }
    }
}