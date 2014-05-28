using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class ImpactCategoryMap : EntityTypeConfiguration<ImpactCategory>
    {
        public ImpactCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ImpactCategoryID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("ImpactCategory");
            this.Property(t => t.ImpactCategoryID).HasColumnName("ImpactCategoryID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
