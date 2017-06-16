namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    using EZRAC.RISK.Util.Common;
   
    public class RiskVehicle :BaseEntity
    {
       
        public string UnitNumber { get; set; }
        public string TagNumber { get; set; }
        public string VIN { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public Nullable<DateTime> TagExpires { get; set; }
        public Nullable<long> Mileage { get; set; }
        public string Description { get; set; }


        public string PurchaseTypeString
        {
            get { return PurchaseType.ToString(); }
            private set { PurchaseType = EnumExtensions.ParseEnum<PurchaseType>(value); }
        }

        
        public Nullable<PurchaseType> PurchaseType { get; set; }

        public  IList<Claim> Claims { get; set; }
        public IList<RiskDamage> RiskDamages { get; set; }
    }

}
