using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.RISK.Util.Common;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class FetchedContractDetailsDto
    {
        public string ContractNo { get; set; }
        public Nullable<DateTime> DateIn { get; set; }
        public Nullable<DateTime> DateOut { get; set; }
        public Nullable<DateTime> OriginalDate { get; set; }

        //Driver Info
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DOB { get; set; }
        public string LicenseNumber { get; set; }
        public long LocationId { get; set; }
        public string LocationCode { get; set; }


        //Vehicle Info
        public string UnitNo { get; set; }
        public IEnumerable<string> SwappedVehicles { get; set; }
        public string PurchaseType { get; set; }

    }
}
