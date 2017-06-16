using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IFTBTargetService : IBaseService<FTBTarget>
    {
        FTBTargetDTO GetFTBTargetBy();
        bool AddTargetDetails(List<FTBTargetDTO> ftbTargetDTO);
        bool UpdateTargetDetails(List<FTBTargetDTO> ftbTargetDTO);
        Task<List<FTBTargetDTO>> GetTargetDetails(long locationBrandId, long year, long month,bool isCopyFrom);
        List<FTBTargetDTO> FetchTargetDetails(long locationBrandId, long year, long month);
        Task<int> CopyFTBTarget(FTBCopyMonthsDTO objFTBCopyMonthsDTO);
    }
}
