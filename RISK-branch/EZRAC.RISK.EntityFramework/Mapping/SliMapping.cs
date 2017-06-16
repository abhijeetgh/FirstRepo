using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class SliMapping : EntityTypeConfiguration<Sli>
    {
        public SliMapping()
        {
            ToTable("Sli");

            this.HasMany(x => x.SliLocations).WithMany(x => x.Sli).Map(x => {

                x.MapLeftKey("SliId");
                x.MapRightKey("LocationId");
                x.ToTable("SliLocations");
            
            });
        }

    }
}
