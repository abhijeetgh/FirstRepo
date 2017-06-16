using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    class RiskDocumentTypeMap : EntityTypeConfiguration<RiskDocumentType>
    {
        public RiskDocumentTypeMap()
        {
            this.ToTable("RiskDocumentTypes");

            this.HasRequired(p => p.RiskDocumentCategory)
               .WithMany(p => p.RiskDocumentTypes)
               .HasForeignKey(p => p.CategoryId);

        }
    }
}
