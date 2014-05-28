using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class LCIAMethodMap : EntityTypeConfiguration<LCIAMethod>
    {
        public LCIAMethodMap()
        {
            // Primary Key
            this.HasKey(t => t.LCIAMethodID);

            // Properties
            this.Property(t => t.LCIAMethodUUID)
                .HasMaxLength(36);

            this.Property(t => t.LCIAMethodVersion)
                .HasMaxLength(15);

            this.Property(t => t.Name)
                .HasMaxLength(255);

            this.Property(t => t.Methodology)
                .HasMaxLength(60);

            this.Property(t => t.ReferenceYear)
                .HasMaxLength(60);

            this.Property(t => t.Duration)
                .HasMaxLength(255);

            this.Property(t => t.ImpactLocation)
                .HasMaxLength(60);

            this.Property(t => t.Source)
                .HasMaxLength(255);

            this.Property(t => t.ReferenceQuantity)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("LCIAMethod");
            this.Property(t => t.LCIAMethodID).HasColumnName("LCIAMethodID");
            this.Property(t => t.LCIAMethodUUID).HasColumnName("LCIAMethodUUID");
            this.Property(t => t.LCIAMethodVersion).HasColumnName("LCIAMethodVersion");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Methodology).HasColumnName("Methodology");
            this.Property(t => t.ImpactCategoryID).HasColumnName("ImpactCategoryID");
            this.Property(t => t.ImpactIndicator).HasColumnName("ImpactIndicator");
            this.Property(t => t.ReferenceYear).HasColumnName("ReferenceYear");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.ImpactLocation).HasColumnName("ImpactLocation");
            this.Property(t => t.IndicatorTypeID).HasColumnName("IndicatorTypeID");
            this.Property(t => t.Normalization).HasColumnName("Normalization");
            this.Property(t => t.Weighting).HasColumnName("Weighting");
            this.Property(t => t.UseAdvice).HasColumnName("UseAdvice");
            this.Property(t => t.SourceID).HasColumnName("SourceID");
            this.Property(t => t.FlowPropertyID).HasColumnName("FlowPropertyID");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.ReferenceQuantity).HasColumnName("ReferenceQuantity");

            // Relationships
            this.HasOptional(t => t.FlowProperty)
                .WithMany(t => t.LCIAMethods)
                .HasForeignKey(d => d.FlowPropertyID);
            this.HasOptional(t => t.ImpactCategory)
                .WithMany(t => t.LCIAMethods)
                .HasForeignKey(d => d.ImpactCategoryID);
            this.HasOptional(t => t.IndicatorType)
                .WithMany(t => t.LCIAMethods)
                .HasForeignKey(d => d.IndicatorTypeID);

        }
    }
}
