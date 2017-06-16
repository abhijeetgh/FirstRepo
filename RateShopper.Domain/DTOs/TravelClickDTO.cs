using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class TravelClickDTO : BaseModel
    {
        public TravelClickDTO(SearchModelDTO objSearchModelDTO)
            : base(objSearchModelDTO)
        {

        }
    }
}
