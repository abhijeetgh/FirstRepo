using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IPostXMLLogsService
    {        
        Task<string> PostXML();
    }
}
