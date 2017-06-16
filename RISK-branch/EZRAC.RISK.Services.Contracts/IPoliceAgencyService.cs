using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IPoliceAgencyService
    {
        Task<PoliceAgencyDto> GetPoliceAgencyDetailById(long id);

        Task<bool> AddOrUpdatePoliceAgency(PoliceAgencyDto agencyDto, long userId);

        Task<bool> DeletePoliceAgency(PoliceAgencyDto agencyDto, long userId);

        bool IsPoliceAgencyAlreadyUsed(long id);
    }
}
