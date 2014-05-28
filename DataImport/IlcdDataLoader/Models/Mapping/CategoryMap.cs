using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryID);

            // Properties
            this.Property(t => t.Hier)
                .HasMaxLength(250);

            this.Property(t => t.ClassID_SQL)
                .HasMaxLength(60);

            this.Property(t => t.Parent_SQL)
                .HasMaxLength(60);

            this.Property(t => t.ClassName_SQL)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Category");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.CategorySystemID).HasColumnName("CategorySystemID");
            this.Property(t => t.ClassID).HasColumnName("ClassID");
            this.Property(t => t.ParentClassID).HasColumnName("ParentClassID");
            this.Property(t => t.DataTypeID_notneededremovelater).HasColumnName("DataTypeID-notneededremovelater");
            this.Property(t => t.HierarchyLevel).HasColumnName("HierarchyLevel");
            this.Property(t => t.Hier).HasColumnName("Hier");
            this.Property(t => t.ClassID_SQL).HasColumnName("ClassID-SQL");
            this.Property(t => t.Parent_SQL).HasColumnName("Parent-SQL");
            this.Property(t => t.ClassName_SQL).HasColumnName("ClassName-SQL");

            // Relationships
            this.HasOptional(t => t.CategorySystem)
                .WithMany(t => t.Categories)
                .HasForeignKey(d => d.CategorySystemID);
            this.HasOptional(t => t.Class)
                .WithMany(t => t.Categories)
                .HasForeignKey(d => d.ClassID);
            this.HasOptional(t => t.Class1)
                .WithMany(t => t.Categories1)
                .HasForeignKey(d => d.ParentClassID);

        }
    }
}
