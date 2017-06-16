using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface ILocationService : IBaseService<Location>
    {
        List<CarClassDTO> GetCarClass();
        List<RentalLengthDTO> GetRentalLength();
        List<CompanyDTO> GetCompany();
        long SaveLocation(LocationDTO objLocationDTO);
        List<LocationDTO> GetLocations();
        bool DeleteLocation(long locationID, long updatedBy);
        LocationDTO GetLocation(long locationID, long locationBrandID);
        Dictionary<string, long> GetLocationDictionary();
    }
}
