using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;


namespace RateShopper.Data.Mapping
{
    public class ScheduledJobRuleSetsMap : BaseEntityConfiguration<ScheduledJobRuleSets>
    {
        public ScheduledJobRuleSetsMap()
        {
            this.ToTable("ScheduledJobRuleSets");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ScheduleJobID).HasColumnName("ScheduleJobID");
            this.Property(t => t. RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.OriginalRuleSetID).HasColumnName("OriginalRuleSetID");
            this.Property(t => t.IntermediateID).HasColumnName("IntermediateID");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
        }
    }
}
