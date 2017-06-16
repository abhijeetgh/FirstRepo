using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class ContractViewModel
    {
        public string ContractNumber { get; set; }
        public Nullable<DateTime> PickupDate { get; set; }
        public Nullable<DateTime> ReturnDate { get; set; }
        public Nullable<int> DaysOut { get; set; }
        public Nullable<int> Miles { get; set; }
        public Nullable<double> DailyRate { get; set; }
        public Nullable<double> WeeklyRate { get; set; }
        public bool CDW { get; set; }
        public bool CDWVoided { get; set; }
        public bool LDWVoided { get; set; }
        public bool LDW { get; set; }
        public bool SLI { get; set; }
        public bool LPC { get; set; }
        public bool LPC2 { get; set; }
        public bool GARS { get; set; }
        public Nullable<int> TotalRate { get; set; }
        public string SwapedVehicles { get; set; }


        public Int64 Id { get; set; }
    }
}