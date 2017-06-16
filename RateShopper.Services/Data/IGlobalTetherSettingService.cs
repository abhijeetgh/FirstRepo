using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface IGlobalTetherSettingService:IBaseService<GlobalTetherSetting>
    {
        List<GlobalTetherValuesDTO> GetLocationTetheringDetails(long LocationId, long DominantBrandId, long DependantBrandId, List<long> LocationCarclassIds);
        List<long> GetLocationCarClasses(long locationBrandId);
        string SaveLocationTetheringDetails(string locationId, string dominantBrandId, string dependantBrandId, string TetherValues, string loggedInUserId);
        List<ExistingTetheredLocationsDTO> ExistingTetheredLocations();
        string DeleteTetherSetting(long locationId, long dominentBrandId, long dependentBrandId);
        List<GlobalTetherSetting> GetGlobalTetherSettingsLocationSpecific(long locationId);
    }
}
