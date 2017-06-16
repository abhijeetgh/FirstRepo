using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;


namespace RateShopper.Data.Mapping
{
    public class SearchJobUpdateAllMap : BaseEntityConfiguration<SearchJobUpdateAll>
    {
        public SearchJobUpdateAllMap()
        {
            this.ToTable("SearchJobUpdateAll");
            this.Property(t => t.SearchSummaryId).HasColumnName("SearchSummaryId");
            this.Property(t => t.CarClassId).HasColumnName("CarClassId");
            this.Property(t => t.BaseEdit).HasColumnName("BaseEdit");
            this.Property(t => t.BaseEditTwo).HasColumnName("BaseEditTwo");
            this.Property(t => t.IsDaily).HasColumnName("IsDaily");
        }
    }
}
