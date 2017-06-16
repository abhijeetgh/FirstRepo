using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class ClaimTrackingsMap : EntityTypeConfiguration<ClaimTrackings>
    {
        public ClaimTrackingsMap()
        {
            this.ToTable("ClaimTrackings");
            
            this.HasRequired(p => p.RiskTrackings)
                .WithMany(x=>x.ClaimTrackings)
                .HasForeignKey(x => x.TrackingId);

            this.HasRequired(p => p.Claim)
                .WithMany(x => x.ClaimTrackings)
                .HasForeignKey(x => x.ClaimId);

            this.HasRequired(p => p.User)
                .WithMany(x => x.ClaimTrackings)
                .HasForeignKey(x => x.CreatedBy);
        }
    }
}
