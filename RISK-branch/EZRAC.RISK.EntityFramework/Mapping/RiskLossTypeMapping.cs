using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskLossTypeMapping : EntityTypeConfiguration<RiskLossType>
    {
        public RiskLossTypeMapping()
        {
            this.ToTable("RiskLossTypes");
            this.HasKey(t => t.Id);
            this.Property(t => t.Type).HasColumnName("Type");          
        }
    }
}
