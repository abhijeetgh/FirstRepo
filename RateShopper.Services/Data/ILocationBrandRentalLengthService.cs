using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ILocationBrandRentalLengthService : IBaseService<LocationBrandRentalLength>
    {
        void SaveLocationBrandRentalLengths(LocationDTO objLocationDTO);
    }
}
