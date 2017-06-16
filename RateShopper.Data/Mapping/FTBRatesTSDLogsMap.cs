using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class FTBRatesTSDLogsMap : BaseEntityConfiguration<FTBRatesTSDLogs>
    {
        public FTBRatesTSDLogsMap()
        {
            this.ToTable("FTBRatesTSDLogs");
            this.Property(t => t.ScheduleJobId).HasColumnName("ScheduleJobId");
            this.Property(t => t.ShopDate).HasColumnName("ShopDate");
            this.Property(t => t.ReservationCount).HasColumnName("ReservationCount");
            this.Property(t => t.Target).HasColumnName("Target");
            this.Property(t => t.TargetDetailsID).HasColumnName("TargetDetailsID");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
        }
    }
}
