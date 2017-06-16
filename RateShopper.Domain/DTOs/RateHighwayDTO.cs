using Newtonsoft.Json;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RateHighwayDTO : BaseModel
    {
        public RateHighwayDTO(SearchModelDTO objSearchModelDTO)
            : base(objSearchModelDTO)
        {
            
        }

        [JsonProperty(PropertyName = "SIPP")]
        public override string CarClass
        {
            get
            {
                return base.CarClass;
            }
            set
            {
                base.CarClass = value;
            }
        }

        [JsonProperty(PropertyName = "Datasource")]
        public override string DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        [JsonProperty(PropertyName = "BeginDateTime")]
        public override string PickupDateTime
        {
            get
            {
                return base.PickupDateTime;
            }
            set
            {
                base.PickupDateTime = value;
            }
        }

        [JsonProperty(PropertyName = "EndDateTime")]
        public override string ReturnDateTime
        {
            get
            {
                return base.ReturnDateTime;
            }
            set
            {
                base.ReturnDateTime = value;
            }
        }

        [JsonProperty(PropertyName = "PickupLocation")]
        public override string PickupLocation
        {
            get
            {
                return base.PickupLocation;
            }
            set
            {
                base.PickupLocation = value;
            }
        }

        [JsonProperty(PropertyName = "LOR")]
        public override string RentalLength
        {
            get
            {
                return base.RentalLength;
            }
            set
            {
                base.RentalLength = value;
            }
        }

        [JsonProperty(PropertyName = "Vend")]
        public override string VendorCode
        {
            get
            {
                return base.VendorCode;
            }
            set
            {
                base.VendorCode = value;
            }
        }

        [JsonProperty(PropertyName = "DOW")]
        public string DayOfWeek
        {
            get { return "1,2,3,4,5,6,7"; }
        }

        [JsonProperty(PropertyName = "RateType")]
        public string DisplayRateType
        {
            get { return "4"; }
        }

        [JsonProperty(PropertyName = "ReqDesc")]
        public string ShopDescription
        {
            get { return "CybageSource"; }
        }

        [JsonIgnore]
        public override string DropOffLocation
        {
            get
            {
                return base.DropOffLocation;
            }
            set
            {
                base.DropOffLocation = value;
            }
        }

        [JsonIgnore]
        public string AdhocRequest
        {
            get { return RateHighwaySetting.AdhocRequestURL; }
        }

        [JsonIgnore]
        public string AccessID
        {
            get { return RateHighwaySetting.AccessID; }
        }

    }

    public class RateHighwayCallBackDTO
    {
        [JsonProperty(PropertyName = "ShopRequestId")]
        public long ShopRequestId { get; set; }
        
        [JsonProperty(PropertyName = "Endpoint")]
        public string RateShopperEndPoint { get; set; }

        [JsonProperty(PropertyName = "SendResult")]
        public bool SendResult
        {
            get { return true; }
        }

        [JsonProperty(PropertyName = "EndpointType")]
        public string EndPointType
        {
            get { return "REST"; }
        }

        [JsonIgnore]
        public string AccessID
        {
            get { return RateHighwaySetting.AccessID; }
        }

        [JsonIgnore]
        public string CallBackURL
        {
            get { return RateHighwaySetting.CallBackURL; }
        }
    }
}
