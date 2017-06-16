using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class AutoConsoleJobEditDTO
    {
        //JobId
        public long? JobId { get; set; }

        //Frequency	
        public long ScheduledJobFrequencyID { get; set; }
        public string JobScheduleWeekDays { get; set; }
        public string CustomHoursToRun { get; set; }
        public string CustomMinutesToRun { get; set; }
        public string RunTime { get; set; }
        public int? RunDay { get; set; }

        //Days to Run
        public DateTime DaysToRunStartDate { get; set; }
        public DateTime DaysToRunEndDate { get; set; }

        //Price Gap
        public bool IsWideGapTemplate { get; set; }
        public bool IsActiveTethering { get; set; }
        public bool IsGovTemplate { get; set; }

        //send for Update
        public string TSDUpdateWeekDay { get; set; }

        //Rate featch parameters
        public bool IsStandardShop { get; set; }
        public int? StartDateOffset { get; set; }
        public int? EndDateOffset { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PickUpTime { get; set; }
        public string DropOffTime { get; set; }

        public string ScrapperSourceIDs { get; set; }
        public string LocationBrandIDs { get; set; }
        public string RentalLengthIDs { get; set; }
        public string CarClassesIDs { get; set; }

        public string ScrapperSource { get; set; }
        public string CarClasses { get; set; }
        public string location { get; set; }
        public long LoggedInUserId { get; set; }

        public List<ScheduledJobMinRates> ScheduledJobMinRates { get; set; }
        public ScheduledJobTetheringsDTO ScheduledJobTetherings { get; set; }
        public string IntermediateID { get; set; }
        public string ProviderCode { get; set; }
        public long ProviderId { get; set; }
        public string UserName { get; set; }
        public bool IsGov { get; set; }
        public bool CompeteOnBase { get; set; }

        public bool IsOpaqueActive { get; set; }
        public string OpaqueRateCodes { get; set; }
        public List<ScheduledJobOpaqueValuesDTO> OpaqueRates { get; set; }
    }
}
