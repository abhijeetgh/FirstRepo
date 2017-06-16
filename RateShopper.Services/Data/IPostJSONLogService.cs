using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface IPostJSONLogService :IBaseService<PostJSONRequestLog>
    {
        void SavePushJSONSummary(List<PushJSONRequestDTO> pushJSONRequestDTO, long searchSummaryID);
        Task<string> PostJSON();
    }
}
