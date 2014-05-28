using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class FlowFlowPropertyMap : EntityTypeConfiguration<FlowFlowProperty>
    {
        public FlowFlowPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.FlowPropertyVersionID);

            // Properties
            this.Property(t => t.FlowPropertyVersionUUID)
                .HasMaxLength(36);

            this.Property(t => t.FlowProperty_SQL)
                .HasMaxLength(36);

            this.Property(t => t.FlowReference_SQL)
                .HasMaxLength(36);

            // Table & Column Mappings
            this.ToTable("FlowFlowProperty");
            this.Property(t => t.FlowPropertyVersionID).HasColumnName("FlowPropertyVersionID");
            this.Property(t => t.FlowPropertyVersionUUID).HasColumnName("FlowPropertyVersionUUID");
            this.Property(t => t.FlowID).HasColumnName("FlowID");
            this.Property(t => t.FlowPropertyID).HasColumnName("FlowPropertyID");
            this.Property(t => t.MeanValue).HasColumnName("MeanValue");
            this.Property(t => t.StDev).HasColumnName("StDev");
            this.Property(t => t.FlowProperty_SQL).HasColumnName("FlowProperty-SQL");
            this.Property(t => t.FlowReference_SQL).HasColumnName("FlowReference-SQL");
            this.Property(t => t.Ind_SQL).HasColumnName("Ind-SQL");

            // Relationships
            this.HasOptional(t => t.Flow)
                .WithMany(t => t.FlowFlowProperties)
                .HasForeignKey(d => d.FlowID);

        }
    }
}
