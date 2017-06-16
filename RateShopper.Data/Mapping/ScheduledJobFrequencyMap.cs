using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
	public class ScheduledJobFrequencyMap : BaseEntityConfiguration<ScheduledJobFrequency>
	{
		public ScheduledJobFrequencyMap()
		{
			// Primary Key
			//this.HasKey(t => t.ID);

			// Properties
			//this.Property(t => t.Frequency)
			//    .IsRequired()
			//    .HasMaxLength(200);

			// Table & Column Mappings
			this.ToTable("ScheduledJobFrequency");
			//this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.UIControlID).HasColumnName("UIControlID");
			this.Property(t => t.MinuteInterval).HasColumnName("MinuteInterval");
			this.Property(t => t.DayInterval).HasColumnName("DayInterval");
		}
	}
}
