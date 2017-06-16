using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.RISK.Domain;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskDamageMapping : EntityTypeConfiguration<RiskDamage>
    {
        public RiskDamageMapping ()
	    {
            this.ToTable("RiskDamage");

            this.HasRequired(p => p.RiskDamageType)
               .WithMany(p => p.RiskDamage)
               .HasForeignKey(p => p.DamageTypeId);

            this.HasRequired(p => p.RiskVehicle)
              .WithMany(p => p.RiskDamages)
              .HasForeignKey(p => p.VehicleId);
	    }

    }
}
