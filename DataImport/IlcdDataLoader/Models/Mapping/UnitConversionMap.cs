using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class UnitConversionMap : EntityTypeConfiguration<UnitConversion>
    {
        public UnitConversionMap()
        {
            // Primary Key
            this.HasKey(t => t.UnitConversionID);

            // Properties
            this.Property(t => t.UnitConversionUUID)
                .HasMaxLength(36);

            this.Property(t => t.Unit)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("UnitConversion");
            this.Property(t => t.UnitConversionID).HasColumnName("UnitConversionID");
            this.Property(t => t.UnitConversionUUID).HasColumnName("UnitConversionUUID");
            this.Property(t => t.Unit).HasColumnName("Unit");
            this.Property(t => t.UnitGroupID).HasColumnName("UnitGroupID");
            this.Property(t => t.Conversion).HasColumnName("Conversion");
            this.Property(t => t.Ind_sql).HasColumnName("Ind-sql");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.UpdatedOn).HasColumnName("UpdatedOn");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.Voided).HasColumnName("Voided");

            // Relationships
            this.HasOptional(t => t.UnitGroup)
                .WithMany(t => t.UnitConversions)
                .HasForeignKey(d => d.UnitGroupID);

        }
    }
}
