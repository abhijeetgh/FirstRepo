using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentsReceived
{
    public class DocumentsReceivedViewModel
    {
        public long ClaimId { get; set; }
        public bool PoliceReport { get; set; }
        public bool ClaimFolder { get; set; }
        public bool EstimateReceived { get; set; }
        public bool EstimateApproved { get; set; }
    }
}