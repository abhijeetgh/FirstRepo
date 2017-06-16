using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public abstract class BaseModel
    {
        public virtual string PickupLocation { get; set; }
        public virtual string DropOffLocation { get; set; }
        public virtual string PickupDateTime { get; set; }
        public virtual string ReturnDateTime { get; set; }
        public virtual string DataSource { get; set; }
        public virtual string VendorCode { get; set; }
        public virtual string CarClass { get; set; }
        public virtual string RentalLength { get; set; }

        public BaseModel(SearchModelDTO objSearchModelDTO)
        {
            if (objSearchModelDTO != null)
            {
                CarClass = objSearchModelDTO.CarClasses;
                DataSource = objSearchModelDTO.ScrapperSource;
                DropOffLocation = objSearchModelDTO.DropOffLocation;
                if (objSearchModelDTO.StartDate != null && objSearchModelDTO.StartDate.ToShortDateString() != "1/1/0001")
                {
                    PickupDateTime = Convert.ToDateTime(objSearchModelDTO.StartDate.ToString("yyyy-MM-dd") + " " + objSearchModelDTO.PickUpTime).ToString("yyyy-MM-ddTHH:mm");
                }
                if (objSearchModelDTO.EndDate != null && objSearchModelDTO.EndDate.ToShortDateString() != "1/1/0001")
                {
                    ReturnDateTime = Convert.ToDateTime(objSearchModelDTO.EndDate.ToString("yyyy-MM-dd") + " " + objSearchModelDTO.DropOffTime).ToString("yyyy-MM-ddTHH:mm");
                }
                PickupLocation = objSearchModelDTO.location;
                RentalLength = objSearchModelDTO.RentalLengthIDs;
                VendorCode = objSearchModelDTO.VendorCodes;
            }
        }
    }    
}
