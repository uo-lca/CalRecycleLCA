using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class IndicatorTypeMap : EntityTypeConfiguration<IndicatorType>
    {
        public IndicatorTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.IndicatorTypeID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("IndicatorType");
            this.Property(t => t.IndicatorTypeID).HasColumnName("IndicatorTypeID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
