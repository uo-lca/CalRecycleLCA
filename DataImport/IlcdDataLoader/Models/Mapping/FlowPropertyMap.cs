using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class FlowPropertyMap : EntityTypeConfiguration<FlowProperty>
    {
        public FlowPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.FlowPropertyID);

            // Properties
            this.Property(t => t.FlowPropertyUUID)
                .IsFixedLength()
                .HasMaxLength(36);

            this.Property(t => t.FlowPropertyVersion)
                .HasMaxLength(15);

            this.Property(t => t.Name)
                .HasMaxLength(255);

            this.Property(t => t.UnitGroup_SQL)
                .HasMaxLength(36);

            // Table & Column Mappings
            this.ToTable("FlowProperty");
            this.Property(t => t.FlowPropertyID).HasColumnName("FlowPropertyID");
            this.Property(t => t.FlowPropertyUUID).HasColumnName("FlowPropertyUUID");
            this.Property(t => t.FlowPropertyVersion).HasColumnName("FlowPropertyVersion");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UnitGroupID).HasColumnName("UnitGroupID");
            this.Property(t => t.UnitGroup_SQL).HasColumnName("UnitGroup_SQL");

            // Relationships
            this.HasOptional(t => t.UnitGroup)
                .WithMany(t => t.FlowProperties)
                .HasForeignKey(d => d.UnitGroupID);

        }
    }
}
