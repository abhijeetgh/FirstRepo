using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class SplitMonthDetailsMap:BaseEntityConfiguration<SplitMonthDetails>
    {
        public SplitMonthDetailsMap()
        {
            this.ToTable("SplitMonthDetails");
            this.Property(t => t.SplitIndex).HasColumnName("SplitIndex");
            this.Property(t => t.StartDay).HasColumnName("StartDay");
            this.Property(t => t.EndDay).HasColumnName("EndDay");
            this.Property(t => t.Label).HasColumnName("Label");
        }
    }
}
