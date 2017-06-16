using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Helper
{
    public class TRNXML
    {
        // public TRNXML TRNXML { get; set; }
        public string Dategmtime { get; set; }
        public Sender Sender { get; set; }
        public Recipient Recipient { get; set; }
        public TrainingPartner TrainingPartner { get; set; }
        public Customer Customer { get; set; }
        public Message Message { get; set; }
        public List<RateDetail> Payload { get; set; }
    }
    public class Sender
    {
        public string SenderID { get; set; }
        public string SenderName { get; set; }
    }
    public class Recipient
    {
        public string RecipientID { get; set; }
        public string RecipientName { get; set; }
    }
    public class TrainingPartner
    {
        public string TradingPartnerCode { get; set; }
        public string TradingPartnerName { get; set; }
    }
    public class Customer
    {
        public string CustomerNumber { get; set; }
        public string Passcode { get; set; }
    }
    public class Message
    {
        public string MessageID { get; set; }
        public string MessageDesc { get; set; }
    }
    public class RateDetail
    {
        public string RemoteID { get; set; }
        public string Branch { get; set; }
        public string ClassCode { get; set; }
        public string RateType { get; set; }
        public string RateCode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int RatePlan { get; set; }
        public int PerMile { get; set; }
        public decimal? DailyRate { get; set; }
        public string RentalLength { get; set; }
        public decimal? DailyFree { get; set; }
        public decimal? WeeklyRate { get; set; }
        public decimal? WeeklyFree { get; set; }
        public decimal ExtraDayRate { get; set; }
        public decimal ExtraDayFree { get; set; }
        public string RateSystem { get; set; }
        public bool ShouldSerializeDailyRate()
        {
            return DailyRate.HasValue;
        }
        public bool ShouldSerializeDailyFree()
        {
            return DailyFree.HasValue;
        }
        public bool ShouldSerializeWeeklyRate()
        {
            return WeeklyRate.HasValue;
        }
        public bool ShouldSerializeWeeklyFree()
        {
            return WeeklyFree.HasValue;
        }
    }
}
