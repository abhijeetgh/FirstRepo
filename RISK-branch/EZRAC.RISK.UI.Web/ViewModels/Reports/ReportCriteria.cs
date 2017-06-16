using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class ReportCriteriaViewModel
    {
        public ReportCriteriaViewModel()
        {
            
        }
        public string ReportKey { get; set; }
        public string ReportText { get; set; }

        public string TagNumber { get; set; }

        public string DateTypeKey { get; set; }
        public IEnumerable<SelectListItem> DateTypeList { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AsOfDate { get; set; }
        public string AgingDays { get; set; }
        public IEnumerable<SelectListItem> AgingDaysList { get; set; }

        public string ReportTypeKey { get; set; }
        public IEnumerable<SelectListItem> ReportTypeList { get; set; }

        //this is not the claim status this is for OPEN CLOSE and statusID is 42
        public int Status { get; set; }
        public string TicketOrViolations { get; set; } // For singal as Violations or tickets

        public int GreaterThanLessThan { get; set; }        
        public long GreaterThanLessThanValue { get; set; }

        public bool IncludeTicket { get; set; }
        public bool IncludeCollection { get; set; }

        public long AgentId { get; set; }

        public long ExportType { get; set; }

        public long ChargeTypeId { get; set; }
        public IEnumerable<SelectListItem> ChargeTypeList { get; set; }


        public IEnumerable<string> PaymentTypeId { get; set; }
        public IEnumerable<SelectListItem> PaymentTypeList { get; set; }



        //Multiple dropdown selection
        public IEnumerable<string> LossTypeIds { get; set; }
        public IEnumerable<SelectListItem> LossTypeList { get; set; }

        public IEnumerable<string> ClaimStatusIds { get; set; }
        public IEnumerable<SelectListItem> ClaimStatusList { get; set; }

        public IEnumerable<string> LocationIds { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }

        public IEnumerable<string> UserIds { get; set; }
        public IEnumerable<SelectListItem> UserList { get; set; }

        public IEnumerable<string> VehicalDamageIds { get; set; }
        public IEnumerable<SelectListItem> VehicalDamageList { get; set; }

    }
}