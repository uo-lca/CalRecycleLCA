using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class CategorySystemMap : EntityTypeConfiguration<CategorySystem>
    {
        public CategorySystemMap()
        {
            // Primary Key
            this.HasKey(t => t.CategorySystemID);

            // Properties
            this.Property(t => t.CategorySystem1)
                .HasMaxLength(100);

            this.Property(t => t.URI)
                .HasMaxLength(255);

            this.Property(t => t.Delimeter)
                .HasMaxLength(4);

            this.Property(t => t.DataType_SQL)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CategorySystem");
            this.Property(t => t.CategorySystemID).HasColumnName("CategorySystemID");
            this.Property(t => t.CategorySystem1).HasColumnName("CategorySystem");
            this.Property(t => t.URI).HasColumnName("URI");
            this.Property(t => t.DataTypeID).HasColumnName("DataTypeID");
            this.Property(t => t.Delimeter).HasColumnName("Delimeter");
            this.Property(t => t.DataType_SQL).HasColumnName("DataType-SQL");

            // Relationships
            this.HasOptional(t => t.DataType)
                .WithMany(t => t.CategorySystems)
                .HasForeignKey(d => d.DataTypeID);

        }
    }
}
