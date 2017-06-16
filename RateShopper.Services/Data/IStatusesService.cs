using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;

namespace RateShopper.Services.Data
{
    public interface IStatusesService : IBaseService<Statuses>
    {
        long GetStatusIDByName(string status);
    }
}
