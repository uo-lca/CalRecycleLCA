using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class ClassMap : EntityTypeConfiguration<Class>
    {
        public ClassMap()
        {
            // Primary Key
            this.HasKey(t => t.ClassID);

            // Properties
            this.Property(t => t.ExternalClassID)
                .HasMaxLength(60);

            this.Property(t => t.Name)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Class");
            this.Property(t => t.ClassID).HasColumnName("ClassID");
            this.Property(t => t.ExternalClassID).HasColumnName("ExternalClassID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CategorySystemID_SQL).HasColumnName("CategorySystemID-SQL");
        }
    }
}
