using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    class IncidentMapping : EntityTypeConfiguration<RiskIncident>
    {
        public IncidentMapping() {

            ToTable("RiskIncident");

            this.HasOptional(p => p.Location)
            .WithMany(x => x.Incidents)
          .HasForeignKey(x => x.LocationId);

            this.HasOptional(p => p.PoliceAgency)
           .WithMany(x => x.Incidents)
         .HasForeignKey(x => x.PoliceAgencyId);

        }

    }
}
