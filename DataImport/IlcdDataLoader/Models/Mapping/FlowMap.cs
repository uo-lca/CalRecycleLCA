using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class FlowMap : EntityTypeConfiguration<Flow>
    {
        public FlowMap()
        {
            // Primary Key
            this.HasKey(t => t.FlowID);

            // Properties
            this.Property(t => t.FlowUUID)
                .IsFixedLength()
                .HasMaxLength(36);

            this.Property(t => t.FlowVersion)
                .HasMaxLength(15);

            this.Property(t => t.Name)
                .HasMaxLength(255);

            this.Property(t => t.CASNumber)
                .IsFixedLength()
                .HasMaxLength(15);

            this.Property(t => t.FlowType_SQL)
                .HasMaxLength(200);

            this.Property(t => t.ReferenceFlowProperty_SQL)
                .HasMaxLength(36);

            // Table & Column Mappings
            this.ToTable("Flow");
            this.Property(t => t.FlowID).HasColumnName("FlowID");
            this.Property(t => t.FlowUUID).HasColumnName("FlowUUID");
            this.Property(t => t.FlowVersion).HasColumnName("FlowVersion");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CASNumber).HasColumnName("CASNumber");
            this.Property(t => t.FlowPropertyID).HasColumnName("FlowPropertyID");
            this.Property(t => t.FlowTypeID).HasColumnName("FlowTypeID");
            this.Property(t => t.FlowType_SQL).HasColumnName("FlowType_SQL");
            this.Property(t => t.ReferenceFlowProperty_SQL).HasColumnName("ReferenceFlowProperty_SQL");

            // Relationships
            this.HasOptional(t => t.FlowProperty)
                .WithMany(t => t.Flows)
                .HasForeignKey(d => d.FlowPropertyID);
            this.HasOptional(t => t.FlowType)
                .WithMany(t => t.Flows)
                .HasForeignKey(d => d.FlowTypeID);

        }
    }
}
