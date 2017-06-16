using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EZRAC.RISK.Services.Implementation
{
    public class LocationService : ILocationService
    {
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskIncident> _incidentRepository = null;
        IGenericRepository<Location> _locationRepository = null;
        IGenericRepository<Company> _companyRepository = null;

        public LocationService(IGenericRepository<RiskClaimStatus> claimStatusRepository,
                            IGenericRepository<RiskLossType> lossTypeRepository,
                            IGenericRepository<Claim> claimRepository,
                            IGenericRepository<Location> locationRepository,
                            IGenericRepository<Company> companyRepository,
                            IGenericRepository<RiskIncident> incidentRepository)
        {

            _claimRepository = claimRepository;
            _locationRepository = locationRepository;
            _companyRepository = companyRepository;
            _incidentRepository = incidentRepository;
        }

        public async Task<LocationDto> GetLocationByIdAsync(long id)
        {
            LocationDto locationDto = null;

            Location location = await _locationRepository.GetByIdAsync(id);

            if (location != null)
            {
                locationDto = new LocationDto();
                locationDto.Id = location.Id;
                locationDto.Code = location.Code;
                locationDto.CompanyAbbr = locationDto.CompanyAbbr;
                locationDto.CompanyId = location.CompanyId;
                locationDto.Name = location.Name;
                locationDto.State = location.State;
            }

            return locationDto;
        }


        public async Task<bool> AddOrUpdateLocationAsync(LocationDto locationDto)
        {
            bool isSuccess = false;
            Location location = null;
            Company company = null;
            if (locationDto != null)
            {
                company = await _companyRepository.GetByIdAsync(locationDto.CompanyId);

                if (locationDto.Id != 0)
                {
                    location = await _locationRepository.AsQueryable.Where(x => x.Id == locationDto.Id).FirstOrDefaultAsync();

                    location = GetLocationDomain(locationDto, location, company, false);

                    await _locationRepository.UpdateAsync(location);
                    isSuccess = true;
                }
                else
                {
                    location = GetLocationDomain(locationDto, new Location(), company, true);

                    await _locationRepository.InsertAsync(location);
                    isSuccess = true;
                }



            }
            return isSuccess;
        }

        private Location GetLocationDomain(LocationDto locationDto, Location location, Company company, bool isCreate)
        {

            if (locationDto != null && location != null)
            {

                location.Id = locationDto.Id;
                location.Code = locationDto.Code;
                location.Name = locationDto.Name;
                location.CompanyId = locationDto.CompanyId;
                location.State = locationDto.State.ToUpper();
                location.CompanyAbbr = company != null ? company.Abbr : String.Empty;
                if (isCreate)
                {
                    location.CreatedBy = locationDto.AddedOrUpdatedUserId;
                    location.CreatedDateTime = DateTime.Now;
                    location.UpdatedDateTime = DateTime.Now;
                    location.UpdatedBy = locationDto.AddedOrUpdatedUserId;
                }
                else
                {
                    location.UpdatedDateTime = DateTime.Now;
                    location.UpdatedBy = locationDto.AddedOrUpdatedUserId;
                }

            }
            return location;
        }


        public async Task<bool> IsLocationValidAsync(LocationDto locationDto)
        {
            bool isValid = false;
            if (locationDto != null)
            {
                isValid = !await _locationRepository.AsQueryable.Where(x => x.Id != locationDto.Id && !x.IsDeleted &&
                                                                       x.Code.Equals(locationDto.Code) &&
                                                                       x.CompanyId == locationDto.CompanyId).AnyAsync();
            }

            return isValid;
        }


        public async Task<bool> DeleteByIdAsync(long id)
        {
            bool isSuccess = false;

            Location location = await _locationRepository.GetByIdAsync(id);

            location.IsDeleted = true;

            await _locationRepository.UpdateAsync(location);

            isSuccess = true;

            return isSuccess;

        }


        public async Task<bool> IsLocationUsed(long id)
        {
            bool isLocationUsed = false;
            bool isUsedInClaims = false;
            bool isUSedInIncident = false;

            isUsedInClaims = await _claimRepository.AsQueryable.Where(x => x.OpenLocationId == id || x.CloseLocationId == id).AnyAsync();
            isUSedInIncident = await _incidentRepository.AsQueryable.Where(x => x.LocationId == id).AnyAsync();

            isLocationUsed = isUsedInClaims || isUSedInIncident ? true : false;

            return isLocationUsed;
        }
    }
}
