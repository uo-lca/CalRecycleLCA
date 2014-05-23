using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class FlowTypeMap : EntityTypeConfiguration<FlowType>
    {
        public FlowTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FlowTypeID);

            // Properties
            this.Property(t => t.Type)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FlowType");
            this.Property(t => t.FlowTypeID).HasColumnName("FlowTypeID");
            this.Property(t => t.Type).HasColumnName("Type");
        }
    }
}
