using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class Enumerations
    {
        public enum HttpMethod
        {
            Get = 0,
            Post = 1
        }

        public enum RateHighwayRequestType
        {
            AdHocRequest = 0,
            ShopStatus = 1,
            ShopResult = 2
        }

        public enum ShopStatus
        {
            None,
            Pending,
            RequestSentToScrapper,
            DataReceivedFromScrapper,
            ProcessComplete,
            Failed,
            Deleted

        }

    }

    public static class ProviderConstants
    {
        public const string RateHighway = "RH";
        public const string ScrappingService = "SS";
    }

    public static class ShopTypes
    {
        public const string SummaryShop = "SummaryShop";
        
    }

    public static class DateFormats
    {
        public const string MMDDYYYY = "MM/dd/yyyy";

    }

    public static class ShopStatusToDisplay
    {
        public const string InProgress = "IN PROGRESS";
        public const string Scheduled = "SCHEDULED";
        public const string Stopped = "STOPPED";
        public const string Finished = "FINISHED";
        public const string Complete = "COMPLETE";
        public const string Failed = "FAILED";

    }

}
