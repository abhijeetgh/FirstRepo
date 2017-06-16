using RateShopper.Domain.DTOs;
using RateShopper.Providers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Interface
{
    public interface IRequestProcessor
    {
        Task<ResponseModel> SendAsync(ProviderRequest objProviderRequest);
    }
}
