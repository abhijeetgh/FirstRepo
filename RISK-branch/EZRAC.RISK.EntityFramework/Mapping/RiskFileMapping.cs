using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskFilesMapping : EntityTypeConfiguration<RiskFile>
    {
        public RiskFilesMapping()
        {
            ToTable("RiskFiles");            
            this.HasRequired(p => p.FileType)
            .WithMany(x => x.RiskFiles)
            .HasForeignKey(x => x.FileTypeId);
        }
    }
}
