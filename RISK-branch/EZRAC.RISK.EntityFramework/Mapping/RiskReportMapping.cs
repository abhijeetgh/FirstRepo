using EZRAC.RISK.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskReportMapping : EntityTypeConfiguration<RiskReport>
    {
        public RiskReportMapping()
        {
            ToTable("RiskReport");

            this.HasRequired(p => p.RiskReportCategory).
               WithMany(x => x.RiskReport)
                 //.Map(x => x.MapKey("UserRoleID"));
            .HasForeignKey(x => x.ReportCategoryId);
        }
    }
}
