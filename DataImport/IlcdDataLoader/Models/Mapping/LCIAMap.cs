using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class LCIAMap : EntityTypeConfiguration<LCIA>
    {
        public LCIAMap()
        {
            // Primary Key
            this.HasKey(t => t.LCIAID);

            // Properties
            this.Property(t => t.LCIAUUID)
                .IsFixedLength()
                .HasMaxLength(36);

            this.Property(t => t.Location)
                .HasMaxLength(100);

            this.Property(t => t.Flow_SQL)
                .HasMaxLength(36);

            this.Property(t => t.Direction_SQL)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("LCIA");
            this.Property(t => t.LCIAID).HasColumnName("LCIAID");
            this.Property(t => t.LCIAMethodID).HasColumnName("LCIAMethodID");
            this.Property(t => t.LCIAUUID).HasColumnName("LCIAUUID");
            this.Property(t => t.FlowID).HasColumnName("FlowID");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.DirectionID).HasColumnName("DirectionID");
            this.Property(t => t.Factor).HasColumnName("Factor");
            this.Property(t => t.Flow_SQL).HasColumnName("Flow-SQL");
            this.Property(t => t.Direction_SQL).HasColumnName("Direction-SQL");

            // Relationships
            this.HasOptional(t => t.Direction)
                .WithMany(t => t.LCIAs)
                .HasForeignKey(d => d.DirectionID);
            this.HasOptional(t => t.LCIAMethod)
                .WithMany(t => t.LCIAs)
                .HasForeignKey(d => d.LCIAMethodID);

        }
    }
}
