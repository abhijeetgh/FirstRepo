using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class WriteOffInfoViewModel
    {
        public long WriteOffId { get; set; }
        public Nullable<double> Amount { get; set; }
        public DateTime WriteOffDate { get; set; }
        public long WriteOffTypeId { get; set; }
        public string Description { get; set; }
        public string WriteOffType { get; set; }
        public long ClaimId { get; set; }
    }
}