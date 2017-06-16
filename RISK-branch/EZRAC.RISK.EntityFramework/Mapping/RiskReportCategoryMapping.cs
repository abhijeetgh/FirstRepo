using EZRAC.RISK.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskReportCategoryMapping : EntityTypeConfiguration<RiskReportCategory>
    {
        public RiskReportCategoryMapping()
        {
            ToTable("RiskReportCategory");
        }
    }
}
