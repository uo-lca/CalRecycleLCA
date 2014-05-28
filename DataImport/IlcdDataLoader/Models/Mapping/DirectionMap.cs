using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class DirectionMap : EntityTypeConfiguration<Direction>
    {
        public DirectionMap()
        {
            // Primary Key
            this.HasKey(t => t.DirectionID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Direction");
            this.Property(t => t.DirectionID).HasColumnName("DirectionID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
