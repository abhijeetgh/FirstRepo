using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IGlobalLimitService : IBaseService<GlobalLimit>
    {
        List<LocationBrandModel> GetLocationBrands();
        List<GlobalLimitDTO> GetGlobalLimitDetails(long brandLocationID);
        bool DeleteGlobalLimit(long globalLimitID);
        long SaveGlobalLimit(GlobalLimitDTO objGlobalLimitDTO);
    }
}
