using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class SourceMap : EntityTypeConfiguration<Source>
    {
        public SourceMap()
        {
            // Primary Key
            this.HasKey(t => t.SourceID);

            // Properties
            this.Property(t => t.SourceUUID)
                .HasMaxLength(36);

            this.Property(t => t.SourceVersion)
                .HasMaxLength(15);

            this.Property(t => t.Source1)
                .HasMaxLength(255);

            this.Property(t => t.Citation)
                .HasMaxLength(60);

            this.Property(t => t.PubType)
                .HasMaxLength(60);

            this.Property(t => t.URI)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Source");
            this.Property(t => t.SourceID).HasColumnName("SourceID");
            this.Property(t => t.SourceUUID).HasColumnName("SourceUUID");
            this.Property(t => t.SourceVersion).HasColumnName("SourceVersion");
            this.Property(t => t.Source1).HasColumnName("Source");
            this.Property(t => t.Citation).HasColumnName("Citation");
            this.Property(t => t.PubType).HasColumnName("PubType");
            this.Property(t => t.URI).HasColumnName("URI");
        }
    }
}
