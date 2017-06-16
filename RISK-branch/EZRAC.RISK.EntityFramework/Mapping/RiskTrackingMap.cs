using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping    
{
    public class RiskTrackingMap : EntityTypeConfiguration<RiskTrackings>
    {
        public RiskTrackingMap()
        {
            this.HasRequired(p => p.RiskTrackingTypes)
                   .WithMany(p => p.RiskTrackings)
                   .HasForeignKey(x => x.TrackingTypeId);
            this.ToTable("RiskTrackings");

            this.HasMany(x => x.Next).WithMany(x=>x.Previous).Map(x =>
            {
                x.MapLeftKey("TrackingId");
                x.MapRightKey("Next");
                x.ToTable("TrackerMapping");

            });

        }
    }
}
