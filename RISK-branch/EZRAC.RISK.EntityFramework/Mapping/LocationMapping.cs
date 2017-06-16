using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class LocationMapping : EntityTypeConfiguration<EZRAC.RISK.Domain.Location>
    {
        public LocationMapping() {

            ToTable("Location");

            this.HasRequired(p => p.Company)
            .WithMany(x => x.Locations)
          .HasForeignKey(x => x.CompanyId);

          
        }
    }
}
