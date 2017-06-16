using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Helpers
{
    public class MockTsdService : ITSDService
    {
        public async Task<FetchedContractDetailsDto> GetContractInfoFromTSDAsync(string contractNumber)
        {
            FetchedContractDetailsDto fetchedContractInfo = DtoBuilder.Get<FetchedContractDetailsDto>();

            fetchedContractInfo.ContractNo = contractNumber;

            return fetchedContractInfo;
        }

        public ContractInfoFromTsd GetFullContractInfoFromTSD(string contractNumber, string unitNumber)
        {
            ContractInfoFromTsd fullContractInfo = DtoBuilder.Get<ContractInfoFromTsd>();

            fullContractInfo.ContractInfo = DtoBuilder.Get<ContractDto>();

            fullContractInfo.ContractInfo.ContractNumber = contractNumber;

            fullContractInfo.VehicleInfo = DtoBuilder.Get<VehicleDto>();

            fullContractInfo.VehicleInfo.UnitNumber = unitNumber;

            fullContractInfo.DriverAndInsuranceInfo = DtoBuilder.GetList<DriverInfoDto>();

            return fullContractInfo;
        }

        public bool IsContractNumberValid(string contractNumber)
        {
            return true;
        }

        public bool IsUnitNumberValid(string unitNumber)
        {
            return true;
        }

        public VehicleDto GetVehicleInfoFromTSD(string unitNumber)
        {
            VehicleDto vehicleDto = DtoBuilder.Get<VehicleDto>();

            vehicleDto.UnitNumber = unitNumber;

            return vehicleDto;
        }

        public IEnumerable<TSDOpeningAgentName> GetAgentNameForChageLossReport(string ContractNos)
        {
            ChargeTypeReportDto chargeTypeReport = DtoBuilder.Get<ChargeTypeReportDto>();

            IEnumerable<TSDOpeningAgentName> openingAgents = DtoBuilder.GetList<TSDOpeningAgentName>();

            foreach (var item in openingAgents)
            {
                item.ContractNo = ContractNos;
            }

            return openingAgents;
        }

        public async Task<IEnumerable<SearchTagPlateDto>> GetSearchTagPlate(string tagNumber)
        {
            IEnumerable<SearchTagPlateDto> tagPlates = DtoBuilder.GetList<SearchTagPlateDto>();

            return tagPlates;
        }
    }
}
