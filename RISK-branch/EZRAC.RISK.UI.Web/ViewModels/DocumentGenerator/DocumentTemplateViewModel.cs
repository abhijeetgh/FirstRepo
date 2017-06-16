using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.Risk.UI.Web.ViewModels.Claims;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class DocumentTemplateViewModel
    {
        public long ClaimId { get; set; }

        public double TotalDue { get; set; }

        public string RiskUserEmail { get; set; }

        public string RiskUserFullName { get; set; }

        public string RiskUserPhone { get; set; }

        public bool TreatAsCaliforniaTicket { get; set; }

        public double SalvageBill { get; set; }

        public CompanyViewModel Company { get; set; }

        public DriverViewModel DriverViewModel { get; set; }

        public VehicleViewModel VehicleViewModel { get; set; }

        public Nullable<DateTime> DateOfLoss { get; set; }

        public Nullable<DateTime> DateOfLastPayment { get; set; }

        public IEnumerable<RiskBillingsViewModel> Billings { get; set; }

        public IEnumerable<PaymentInfoViewModel> Payments { get; set; }

        public ContractViewModel Contract { get; set; }

        public IEnumerable<DamageViewModel> Damages { get; set; }

        public LocationViewModel Location { get; set; }

    }

}