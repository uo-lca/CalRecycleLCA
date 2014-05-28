using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class DataTypeMap : EntityTypeConfiguration<DataType>
    {
        public DataTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DataTypeID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("DataType");
            this.Property(t => t.DataTypeID).HasColumnName("DataTypeID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
