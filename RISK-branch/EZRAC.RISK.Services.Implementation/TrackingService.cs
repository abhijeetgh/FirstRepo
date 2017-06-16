using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class TrackingService : ITrackingService
    {
        
        IGenericRepository<RiskTrackings> _trackingRepository = null;
        IGenericRepository<ClaimTrackings> _claimTrackingRepository = null;
        IGenericRepository<Permission> _permissionRepository = null;
        IGenericRepository<User> _userRepository = null;
        IGenericRepository<UserRole> _userRoleRepository = null;
        IGenericRepository<RiskTrackingTypes> _trackingTypesRepository = null;
        public TrackingService(IGenericRepository<RiskTrackings> trackingRepository,
                               IGenericRepository<ClaimTrackings> claimTrackingRepository,
                               IGenericRepository<Permission> permissionRepository,
                               IGenericRepository<User> userRepository,
                               IGenericRepository<UserRole> userRoleRepository,
                               IGenericRepository<RiskTrackingTypes> trackingTypesRepository)
        {
            _trackingRepository = trackingRepository;
            _claimTrackingRepository = claimTrackingRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _trackingTypesRepository = trackingTypesRepository;
        }

        public async Task<IEnumerable<TrackingDto>> GetAllTrackingsByTypeAsync(long trackingType,long claimId)
        {
            List<RiskTrackings> nextTracking = null;
            var allTrackings = await _trackingRepository.AsQueryable.IncludeMultiple(x => x.ClaimTrackings,
                                                                                            x=>x.Next,x=>x.RiskTrackingTypes.Permissions)
                                                                                            .Where(x=>x.TrackingTypeId == trackingType)
                                                                                            .ToListAsync();

            var claimTrackings = await _claimTrackingRepository.AsQueryable.IncludeMultiple(x => x.RiskTrackings,
                                                                                            x => x.User)
                                                                                            .Where(x => x.ClaimId == claimId && x.TrackingTypeId == trackingType)
                                                                                            .ToListAsync();

            var lastTracking = claimTrackings.OrderByDescending(x => x.TrackingId).FirstOrDefault();

            nextTracking = lastTracking != null ?
                                allTrackings.Where(x => x.TrackingTypeId == trackingType && x.Id == lastTracking.TrackingId).SingleOrDefault().Next :
                                allTrackings.Where(x => x.SequenceId == 1 && x.TrackingTypeId == trackingType).ToList();

            
            var trackingsToShow = MapTrackingDto(allTrackings,nextTracking.Select(x=>x.Id).ToList(),claimTrackings);
            return trackingsToShow.OrderBy(x => x.SequenceId);
        }

        public IEnumerable<TrackingTypeDto> GetTrackingCategoriesAsync()
        {
           var riskCategories =  _trackingTypesRepository.AsQueryable.Include(x => x.Permissions).ToList();
           var trackingTypes = MapTrackerCategoryDto(riskCategories);
           return trackingTypes;
        }

        public async Task<bool> UpdateEventAsync(long claimId,long userId,long trackingId,long type)
        {
            var success = false;
            ClaimTrackings lastClaimTrack = await _claimTrackingRepository.AsQueryable
                                                            .Where(x=>x.TrackingTypeId == type && x.ClaimId == claimId)
                                                            .OrderByDescending(x=>x.CreatedDateTime).FirstOrDefaultAsync();
            TimeSpan timeTaken = new TimeSpan(0,0,0);

            timeTaken = lastClaimTrack != null ? DateTime.Now - lastClaimTrack.CreatedDateTime : timeTaken;

            await _claimTrackingRepository.InsertAsync(
                new ClaimTrackings
                {
                    ClaimId = claimId,
                    CreatedBy = userId,
                    TrackingId = trackingId,
                    CreatedDateTime = DateTime.Now,
                    TrackingTypeId = type,
                    timeTaken = timeTaken.TotalMinutes
                });
                success = true;
            
            return success;
        }

        public async Task<bool> UndoEventTrackingAsync(long claimTrackingId)
        {
            var success = false;
            if (claimTrackingId != null)
            { 
                    var eventToUndo = await _claimTrackingRepository.GetByIdAsync(claimTrackingId);
                    await _claimTrackingRepository.DeleteAsync(eventToUndo);
                    success = true;
            }
            return success;
        }

        public async Task<TimeSpan> GetTotalTimeTakenAsync(long claimId, long type)
        {
            var events = await _claimTrackingRepository.AsQueryable
                                                            .Where(x => x.ClaimId == claimId && x.TrackingTypeId == type)
                                                            .ToListAsync();

            var firstEvent = events.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
            var lastEvent = events.OrderBy(x => x.CreatedDateTime).FirstOrDefault();
            TimeSpan span = new TimeSpan(0, 0, 0);
            span = firstEvent != null && lastEvent != null ? (firstEvent.CreatedDateTime - lastEvent.CreatedDateTime) : span;
            return span;
        }

      

        #region private Methods
        private IEnumerable<TrackingDto> MapTrackingDto(IEnumerable<RiskTrackings> tracking, List<long> nextTrackingList, IEnumerable<ClaimTrackings> claimTracking)
        {
            var trackingList = new List<TrackingDto>();
            TrackingDto trackingDto = null;
            foreach (var item in tracking)
            {
                trackingDto = new TrackingDto();
                var currentClaimTracking = claimTracking.Where(x => x.TrackingId == item.Id).FirstOrDefault();
                var lastTracking = claimTracking.OrderByDescending(x => x.TrackingId).FirstOrDefault();

                trackingDto.Id = item.Id;
                trackingDto.TrackingTypeId = item.TrackingTypeId;
                trackingDto.TrackingDescription = item.TrackingDescription;
                if (currentClaimTracking != null)
                {
                    trackingDto.ClaimTrackingId = currentClaimTracking.Id;
                    trackingDto.TimeTaken = currentClaimTracking != null ? TimeSpan.FromMinutes(Convert.ToDouble(currentClaimTracking.timeTaken)) : TimeSpan.Zero;
                    trackingDto.ClaimId = currentClaimTracking.ClaimId;
                    trackingDto.CreatedBy = currentClaimTracking.User.FirstName + currentClaimTracking.User.LastName;
                    trackingDto.CreatedDateTime = currentClaimTracking.CreatedDateTime;
                    trackingDto.IsCompleted = true;
                }
                trackingDto.IsCurrent = nextTrackingList.Contains(item.Id) ? true : false;
                trackingDto.CanUndo = lastTracking != null && item.Id == lastTracking.TrackingId ? true : false;
                trackingList.Add(trackingDto);
                trackingDto.SequenceId = item.SequenceId;
            }
            return trackingList;
        }


        private IEnumerable<TrackingTypeDto> MapTrackerCategoryDto(IEnumerable<RiskTrackingTypes> types)
                {
            var listTrackingDto = new List<TrackingTypeDto>();
            TrackingTypeDto trackingDto = null;

            foreach (var item in types)
                {
                trackingDto = new TrackingTypeDto();
                trackingDto.Id = item.Id;
                trackingDto.Key = item.Permissions != null ? item.Permissions.Key : string.Empty;
                trackingDto.Category = item.TrackingType;
                listTrackingDto.Add(trackingDto);
            }
            return listTrackingDto;
        } 
        #endregion

    }

}
