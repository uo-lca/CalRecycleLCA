using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class ProcessMap : EntityTypeConfiguration<Process>
    {
        public ProcessMap()
        {
            // Primary Key
            this.HasKey(t => t.ProcessID);

            // Properties
            this.Property(t => t.ProcessUUID)
                .HasMaxLength(36);

            this.Property(t => t.ProcessVersion)
                .HasMaxLength(255);

            this.Property(t => t.Name)
                .HasMaxLength(255);

            this.Property(t => t.Year)
                .HasMaxLength(60);

            this.Property(t => t.Geography)
                .HasMaxLength(15);

            this.Property(t => t.ReferenceFlow_SQL)
                .HasMaxLength(36);

            this.Property(t => t.RefererenceType)
                .HasMaxLength(60);

            this.Property(t => t.ProcessType)
                .HasMaxLength(60);

            this.Property(t => t.Diagram)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("Process");
            this.Property(t => t.ProcessID).HasColumnName("ProcessID");
            this.Property(t => t.ProcessUUID).HasColumnName("ProcessUUID");
            this.Property(t => t.ProcessVersion).HasColumnName("ProcessVersion");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Geography).HasColumnName("Geography");
            this.Property(t => t.ReferenceFlow_SQL).HasColumnName("ReferenceFlow-SQL");
            this.Property(t => t.RefererenceType).HasColumnName("RefererenceType");
            this.Property(t => t.ProcessType).HasColumnName("ProcessType");
            this.Property(t => t.Diagram).HasColumnName("Diagram");
            this.Property(t => t.FlowID).HasColumnName("FlowID");

            // Relationships
            this.HasOptional(t => t.Flow)
                .WithMany(t => t.Processes)
                .HasForeignKey(d => d.FlowID);

        }
    }
}
