using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DocumentHeaderDto
    {
        public CompanyDto Company { get; set; }
        public DriverForDocumentDto DriverInfo { get; set; }
        public VehicleDto VehicleDto { get; set; }
        public ContractDto Contarct { get; set; }
        public Nullable<DateTime> DateOfLoss { get; set; }
        public LocationDto Location { get; set; }
        public long ClaimId { get; set; }
        public string ContractNumber { get; set; }
        public string AssignedUserEmail { get; set; }
        public string AssignedUserFullName { get; set; }
        public string AssignedUserPhoneNumber { get; set; }
        public int LossTypeId { get; set; }
        public DateTime OpenDate { get; set; }

    }

    public class DriverForDocumentDto {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string LicenceNumber { get; set; }
        public string LicenceState { get; set; }
    }
}
