using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IlcdDataLoader.Models.Mapping
{
    public class ProcessFlowMap : EntityTypeConfiguration<ProcessFlow>
    {
        public ProcessFlowMap()
        {
            // Primary Key
            this.HasKey(t => t.ProcessFlowID);

            // Properties
            this.Property(t => t.ProcessUUID)
                .HasMaxLength(36);

            this.Property(t => t.Type)
                .HasMaxLength(15);

            this.Property(t => t.VarName)
                .HasMaxLength(15);

            this.Property(t => t.Flow_SQL)
                .HasMaxLength(50);

            this.Property(t => t.Direction_SQL)
                .HasMaxLength(50);

            this.Property(t => t.Geography)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("ProcessFlow");
            this.Property(t => t.ProcessFlowID).HasColumnName("ProcessFlowID");
            this.Property(t => t.ProcessUUID).HasColumnName("ProcessUUID");
            this.Property(t => t.ProcessID).HasColumnName("ProcessID");
            this.Property(t => t.FlowID).HasColumnName("FlowID");
            this.Property(t => t.DirectionID).HasColumnName("DirectionID");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.VarName).HasColumnName("VarName");
            this.Property(t => t.Magnitude).HasColumnName("Magnitude");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.STDev).HasColumnName("STDev");
            this.Property(t => t.Flow_SQL).HasColumnName("Flow-SQL");
            this.Property(t => t.Direction_SQL).HasColumnName("Direction-SQL");
            this.Property(t => t.Ind_SQL).HasColumnName("Ind-SQL");
            this.Property(t => t.Geography).HasColumnName("Geography");

            // Relationships
            this.HasOptional(t => t.Direction)
                .WithMany(t => t.ProcessFlows)
                .HasForeignKey(d => d.DirectionID);
            this.HasOptional(t => t.Flow)
                .WithMany(t => t.ProcessFlows)
                .HasForeignKey(d => d.FlowID);
            this.HasOptional(t => t.Process)
                .WithMany(t => t.ProcessFlows)
                .HasForeignKey(d => d.ProcessID);

        }
    }
}
