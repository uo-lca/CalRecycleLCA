using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class UnitGroupMap : EntityTypeConfiguration<UnitGroup>
    {
        public UnitGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.UnitGroupID);

            // Properties
            this.Property(t => t.UnitGroupUUID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(36);

            this.Property(t => t.Version)
                .HasMaxLength(15);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.ReferenceUnit)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("UnitGroup");
            this.Property(t => t.UnitGroupID).HasColumnName("UnitGroupID");
            this.Property(t => t.UnitGroupUUID).HasColumnName("UnitGroupUUID");
            this.Property(t => t.Version).HasColumnName("Version");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ReferenceUnit).HasColumnName("ReferenceUnit");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.UpdatedOn).HasColumnName("UpdatedOn");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.Voided).HasColumnName("Voided");
        }
    }
}
