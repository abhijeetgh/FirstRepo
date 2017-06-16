using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface ITSDService
    {
        Task<FetchedContractDetailsDto> GetContractInfoFromTSDAsync(string contractNumber);

        ContractInfoFromTsd GetFullContractInfoFromTSD(string contractNumber, string unitNumber);

        bool IsContractNumberValid(string contractNumber);

        bool IsUnitNumberValid(string unitNumber);


        VehicleDto GetVehicleInfoFromTSD(string unitNumber);

        IEnumerable<TSDOpeningAgentName> GetAgentNameForChageLossReport(string ContractNos);

        Task<IEnumerable<SearchTagPlateDto>> GetSearchTagPlate(string tagNumber);
    }
}
