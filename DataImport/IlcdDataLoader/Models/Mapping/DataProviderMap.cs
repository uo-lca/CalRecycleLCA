using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class DataProviderMap : EntityTypeConfiguration<DataProvider>
    {
        public DataProviderMap()
        {
            // Primary Key
            this.HasKey(t => t.DataProviderID);

            // Properties
            this.Property(t => t.DataProviderUUID)
                .HasMaxLength(36);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("DataProvider");
            this.Property(t => t.DataProviderID).HasColumnName("DataProviderID");
            this.Property(t => t.DataProviderUUID).HasColumnName("DataProviderUUID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
