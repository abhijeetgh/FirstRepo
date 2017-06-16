using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    /// <summary>
    /// Claim Class used for claim listing and creating new claims
    /// </summary>
    public class ClaimDto
    {
        public Int64 Id { get; set; }

        public string ContractNo { get; set; }

        public DateTime OpenDate { get; set; }

        public Nullable<DateTime> CloseDate { get; set; }

        public Nullable<DateTime> DateOfLoss { get; set; }

        public string DriverName { get; set; }

        public string UnitNumber { get; set; }

        public Nullable<long> VehicleId { get; set; }

        public string VehicleName { get; set; }

        public Nullable<double> LabourHour { get; set; }

        public Nullable<DateTime> FollowUpdate { get; set; }

        public Nullable<DateTime> EstReturnDate { get; set; }

        public bool IsComplete { get; set; }

        public IEnumerable<LocationDto> OpenLocaton { get; set; }
        public Nullable<Int64> SelectedOpenLocationId { get; set; }
        public string SelectedOpenLocationName { get; set; }

        public string CompanyAbbr { get; set; }

        public IEnumerable<LocationDto> CloseLocation { get; set; }
        public Nullable<Int64> SelectedCloseLocationId { get; set; }
        public string SelectedCloseLocationName { get; set; }


        public IEnumerable<LossTypesDto> LossTypes { get; set; }
        public int SelectedLossTypeId { get; set; }
        public string SelectedLossTypeDescription { get; set; }


        public IEnumerable<ClaimStatusDto> Status { get; set; }
        public int SelectedStatusId { get; set; }
        public string SelectedStatusName { get; set; }


        public IEnumerable<UserDto> Users { get; set; }
        public Int64 SelectedAssignedUserId { get; set; }
        public string SelectedAssignedUserName { get; set; }

        public Nullable<Int64> ApproverId { get; set; }
        public string ApprovalStatus { get; set; }

        public int RecordCount { get; set; }

        public Nullable<double> TotalDue { get; set; }

        public bool IsCollectable { get; set; }

        //need to add notes

        public bool HasContract { get; set; }

        public bool DisableHasContract { get; set; }
    }
    public class ClaimSearchDto
    {
        public long SearchTypeId { get; set; }
        public string SearchType { get; set; }
        public string SearchItem { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        //public string DateOfLoss { get; set; }
    }
}
