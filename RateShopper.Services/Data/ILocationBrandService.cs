using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ILocationBrandService: IBaseService<LocationBrand>
    {
        long SaveLocationBrand(LocationDTO objLocationDTO);
        bool DeleteLocationBrand(long locationBrandID, long updatedBy);
        long GetLocationBrandId(long locationId, long BrandId, string rentalLength);
        decimal FetchExtraDailyRate(long LocationBrandId,string rentalLength);
        string GetQuickViewCompetitorsIds(long locationBrandId);
        string GetCompetitors(long locationBrandId);
    }
}
