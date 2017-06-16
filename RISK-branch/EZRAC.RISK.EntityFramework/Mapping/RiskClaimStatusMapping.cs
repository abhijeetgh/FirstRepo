using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskClaimStatusMapping : EntityTypeConfiguration<RiskClaimStatus>
    {
        public RiskClaimStatusMapping()
        {
            this.ToTable("RiskClaimStatus");
            this.Property(t => t.Id).HasColumnName("Id");            
            this.Property(t => t.Description);
        }
    }
}
