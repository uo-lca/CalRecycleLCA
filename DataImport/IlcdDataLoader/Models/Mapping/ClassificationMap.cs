using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class ClassificationMap : EntityTypeConfiguration<Classification>
    {
        public ClassificationMap()
        {
            // Primary Key
            this.HasKey(t => t.ClassificationID);

            // Properties
            this.Property(t => t.ClassificationUUID)
                .HasMaxLength(36);

            this.Property(t => t.ClassID_SQL)
                .HasMaxLength(100);

            this.Property(t => t.CategorySystem_SQL)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Classification");
            this.Property(t => t.ClassificationID).HasColumnName("ClassificationID");
            this.Property(t => t.ClassificationUUID).HasColumnName("ClassificationUUID");
            this.Property(t => t.CategorySystemID).HasColumnName("CategorySystemID");
            this.Property(t => t.ClassID).HasColumnName("ClassID");
            this.Property(t => t.ClassID_SQL).HasColumnName("ClassID-SQL");
            this.Property(t => t.CategorySystem_SQL).HasColumnName("CategorySystem-SQL");

            // Relationships
            this.HasOptional(t => t.CategorySystem)
                .WithMany(t => t.Classifications)
                .HasForeignKey(d => d.CategorySystemID);
            this.HasOptional(t => t.Class)
                .WithMany(t => t.Classifications)
                .HasForeignKey(d => d.ClassID);

        }
    }
}
