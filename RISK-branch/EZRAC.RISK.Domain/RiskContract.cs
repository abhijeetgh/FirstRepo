namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    public class RiskContract:BaseEntity
    {
        public RiskContract()
        {
            this.Claims = new List<Claim>();
        }    
        
        public string ContractNumber { get; set; }
        public Nullable<System.DateTime> PickUpDate { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public Nullable<int> DaysOut { get; set; }
        public Nullable<int> Miles { get; set; }
        public Nullable<int> MilesIn { get; set; }
        public Nullable<int> MilesOut { get; set; }
        public Nullable<double> DailyRate { get; set; }
        public Nullable<double> WeeklyRate { get; set; }
        public bool CDW { get; set; }
        public bool LDW { get; set; }
        public bool CDWVoid { get; set; }
        public bool LDWVoid { get; set; }
        public bool SLI { get; set; }
        public bool LPC { get; set; }
        public bool LPC2 { get; set; }
        public bool GARS { get; set; }

        public Nullable<int> CardNumber { get; set; }

        public string CardType { get; set; }

        public string CardExpDate { get; set; }

        public string SwapedVehicles { get; set; }
        
        public Nullable<int> TotalRate { get; set; }
       
        public  IList<Claim> Claims { get; set; }
    }
}
