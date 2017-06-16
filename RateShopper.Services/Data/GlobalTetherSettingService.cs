using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RateShopper.Domain.DTOs;
using System.Web.Script.Serialization;

namespace RateShopper.Services.Data
{
    public class GlobalTetherSettingService : BaseService<GlobalTetherSetting>, IGlobalTetherSettingService
    {
        public GlobalTetherSettingService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<GlobalTetherSetting>();
            _cacheManager = cacheManager;
        }

        public List<GlobalTetherValuesDTO> GetLocationTetheringDetails(long locationId, long dominantBrandId, long dependantBrandId, List<long> LocationCarclassIds)
        {

            var result = _context.GlobalTetherSettings.Where(a => a.DependantBrandID == dependantBrandId && a.DominentBrandID == dominantBrandId && a.LocationID == locationId && LocationCarclassIds.Contains(a.CarClassID)).Select(a => new GlobalTetherValuesDTO { CarClassId = a.CarClassID, TetherValue = a.TetherValue, IsPercentage = a.IsTeatherValueinPercentage }).ToList();

            return result;
        }

        public List<long> GetLocationCarClasses(long locationBrandId)
        {
            return _context.LocationBrandCarClass.Where(a => a.LocationBrandID == locationBrandId).Select(a => a.CarClassID).ToList();
        }

        public string SaveLocationTetheringDetails(string locationId, string dominantBrandId, string dependantBrandId, string TetherValues, string loggedInUserId)
        {
            long LocationId = 0, DominantBrandId = 0, DependantBrandId = 0, LoggedInUserId = 0; string message = "success";
            long.TryParse(locationId, out LocationId);
            long.TryParse(dominantBrandId, out DominantBrandId);
            long.TryParse(dependantBrandId, out DependantBrandId);
            long.TryParse(loggedInUserId, out LoggedInUserId);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (!string.IsNullOrEmpty(TetherValues))
            {
                dynamic tetherValues = serializer.Deserialize<dynamic>(TetherValues.Trim());

                foreach (var tethervalue in tetherValues)
                {
                    long carclassID = Convert.ToInt64(tethervalue.Key);

                    GlobalTetherSetting existingGlobalTethering = base.GetAll(false).Where(a => a.LocationID == LocationId && a.DominentBrandID == DominantBrandId && a.DependantBrandID == DependantBrandId && a.CarClassID == carclassID).FirstOrDefault();

                    if (existingGlobalTethering != null)
                    {
                        existingGlobalTethering.IsTeatherValueinPercentage = Convert.ToBoolean(tethervalue.Value["IsValueInPercentage"]);
                        if (!string.IsNullOrEmpty(tethervalue.Value["TetherValue"]))
                        {
                            existingGlobalTethering.TetherValue = Convert.ToDecimal(tethervalue.Value["TetherValue"]);
                            existingGlobalTethering.UpdatedBy = LoggedInUserId;
                            existingGlobalTethering.UpdatedDateTime = DateTime.Now;
                            base.Update(existingGlobalTethering);
                        }
                        else
                        {
                            base.Delete(existingGlobalTethering);
                        }

                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(tethervalue.Value["TetherValue"]))
                        {

                            GlobalTetherSetting globalTetherSetting = new GlobalTetherSetting();
                            globalTetherSetting.LocationID = LocationId;
                            globalTetherSetting.DominentBrandID = DominantBrandId;
                            globalTetherSetting.DependantBrandID = DependantBrandId;
                            globalTetherSetting.CarClassID = carclassID;
                            globalTetherSetting.IsTeatherValueinPercentage = Convert.ToBoolean(tethervalue.Value["IsValueInPercentage"]);
                            globalTetherSetting.TetherValue = Convert.ToDecimal(tethervalue.Value["TetherValue"]);
                            globalTetherSetting.CreatedBy = globalTetherSetting.UpdatedBy = LoggedInUserId;
                            globalTetherSetting.CreatedDateTime = globalTetherSetting.UpdatedDateTime = DateTime.Now;

                            _context.GlobalTetherSettings.Add(globalTetherSetting);
                        }
                    }
                }

                _context.SaveChanges();
            }
            return message;
        }


        public List<ExistingTetheredLocationsDTO> ExistingTetheredLocations()
        {
            //Get all brands
            Dictionary<long, string> Companies = _context.Companies.Where(a => a.IsBrand && !a.IsDeleted).Select(a => new { ID = a.ID, Code = a.Code }).Distinct().ToDictionary(a => a.ID, a => a.Code);

            //Get all locations
            Dictionary<long, string> locations = _context.Locations.Where(a => !a.IsDeleted).Select(a => new { ID = a.ID, Code = a.Code }).Distinct().ToDictionary(a => a.ID, a => a.Code);

            //Get list of all tethered locations
            var ExistingTetherLocations = _context.GlobalTetherSettings.Join(_context.CarClasses, GTS => GTS.CarClassID, CC => CC.ID,
                (GTS, CC) => new { DominentBrandID = GTS.DominentBrandID, DependantBrandID = GTS.DependantBrandID, IsCarClassDeleted = CC.IsDeleted, LocationID = GTS.LocationID })
                .Where(a => !a.IsCarClassDeleted).GroupBy(x => new { x.DominentBrandID, x.DependantBrandID }).Select(a => new { Key = a.Key, Value = a.GroupBy(b => b.LocationID) }).Select(c => new { DominantBrandId = c.Key.DominentBrandID, DependantBrandId = c.Key.DependantBrandID, Value = c.Value }).ToList();

            List<ExistingTetheredLocationsDTO> TetheredLocations = new List<ExistingTetheredLocationsDTO>();
            foreach (var tetherBrandLocation in ExistingTetherLocations)
            {

                ExistingTetheredLocationsDTO existingTetherLocation = new ExistingTetheredLocationsDTO();
                existingTetherLocation.DominantBrandId = tetherBrandLocation.DominantBrandId;
                existingTetherLocation.DependantBrandId = tetherBrandLocation.DependantBrandId;
                existingTetherLocation.LocationIds = string.Join(",", tetherBrandLocation.Value.Where(b => locations.ContainsKey(b.Key)).Select(x => x.Key.ToString()));
                existingTetherLocation.DominantBrand = Companies[tetherBrandLocation.DominantBrandId];
                existingTetherLocation.DependantBrand = Companies[tetherBrandLocation.DependantBrandId];
                existingTetherLocation.Locations = string.Join(",", tetherBrandLocation.Value.Where(b => locations.ContainsKey(b.Key)).Select(x => locations[x.Key]));
                TetheredLocations.Add(existingTetherLocation);
            }

            return TetheredLocations;

        }


        public string DeleteTetherSetting(long locationId, long dominentBrandId, long dependentBrandId)
        {
            string message = string.Empty;
            if (_context.GlobalTetherSettings.Where(tether => tether.LocationID == locationId && tether.DominentBrandID == dominentBrandId && tether.DependantBrandID == dependentBrandId).Count() > 0)
            {
                _context.GlobalTetherSettings.Where(tether => tether.LocationID == locationId && tether.DominentBrandID == dominentBrandId && tether.DependantBrandID == dependentBrandId)
                    .ToList().ForEach(tether => _context.GlobalTetherSettings.Remove(tether));
                _context.SaveChanges();
                message = "success";
            }
            else
                message = "failed";
            return message;
        }

        public List<GlobalTetherSetting> GetGlobalTetherSettingsLocationSpecific(long locationId)
        {
            List<GlobalTetherSetting> globalTetherSetting=_context.GlobalTetherSettings.Where(obj => obj.LocationID == locationId).ToList();
            return globalTetherSetting;
        }
    }
}
