using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ILocationCompanyService : IBaseService<LocationCompany>
    {
        List<LocationCompanyDTO> GetCompanyLocations(long companyID);
        void SaveCompanyLocations(List<LocationCompanyDTO> lstCompanyLocations, long companyID);
        List<LocationCompanyDTO> GetLocations();
    }
}
