namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    public class Claim : AuditableEntity
    {           
        public System.DateTime OpenDate { get; set; }

        public Nullable<System.DateTime> CloseDate { get; set; }

        public Int64 AssignedTo { get; set; }

        public string AssignedToName { get; set; }

        public Nullable<System.DateTime> FollowUpDate { get; set; }

        public Nullable<DateTime> EstReturnDate { get; set; }
       
        public Nullable<double> LabourHour { get; set; }
       
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<double> TotalBilling { get; set; }

        public Nullable<double> TotalPayment { get; set; }

        public int LossTypeId { get; set; }

        public int ClaimStatusId { get; set; }

        public Nullable<Int64> OpenLocationId { get; set; }

        public Nullable<Int64> CloseLocationId { get; set; }

        public Nullable<Int64> VehicleId { get; set; }

        public Nullable<Int64> ContractId { get; set; }

        public Nullable<Int64> NonContractId { get; set; }

        public Nullable<Int64> IncidentId { get; set; }

        public Nullable<decimal> SalvageAmount { get; set; }

        public Nullable<DateTime> SalvageReceiptDate { get; set; }

        public bool IsCollectable { get; set; }

        public bool HasContract { get; set; }

        public RiskVehicle RiskVehicle { get; set; }

        public RiskClaimStatus RiskClaimStatus { get; set; }

        public RiskLossType RiskLossType { get; set; }

        public Location OpenLocation { get; set; }

        public Location CloseLocation { get; set; }

        public RiskContract RiskContract { get; set; }

        public RiskNonContract RiskNonContract { get; set; }

        public RiskIncident RiskIncident { get; set; }
       
        public IList<RiskDriver> RiskDrivers { get; set; }

        public IList<RiskClaimApproval> RiskClaimApprovals { get; set; }

        public IList<RiskNote> RiskNotes { get; set; }

        public IList<RiskDamage> RiskDamages { get; set; }

        public IList<RiskBilling> RiskBillings { get; set; }

        public IList<RiskPayment> RiskPayments { get; set; }

        public User AssignedUser { get; set; }

        public RiskDocumentsReceived RiskDocumentsReceived { get; set; }

        public IList<ClaimTrackings> ClaimTrackings { get; set; }

        public IList<RiskWriteOff> RiskWriteOffs { get; set; }

        
    }
}
