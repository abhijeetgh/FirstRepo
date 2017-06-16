using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface IJobTypeMapperService : IBaseService<JobTypeFrequencyMapper>
    {

        List<JobFrequencyTypesDTO> GetJobFrequencyTypes(string jobType);
    }
}
