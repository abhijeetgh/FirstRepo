namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskDamage :BaseEntity
    {
        
        public Int64 ClaimId { get; set; }
        public Int64 VehicleId { get; set; }
        public int DamageTypeId { get; set; }
        public string Details { get; set; }
        public Claim Claim { get; set; }

        public RiskDamageType RiskDamageType { get; set; }

        public RiskVehicle RiskVehicle { get; set; }
    }
}
