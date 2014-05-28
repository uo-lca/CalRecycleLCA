namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Process")]
    public partial class Process
    {
        public Process()
        {
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public int ProcessID { get; set; }

        [StringLength(36)]
        public string ProcessUUID { get; set; }

        [StringLength(255)]
        public string ProcessVersion { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(60)]
        public string Year { get; set; }

        [StringLength(15)]
        public string Geography { get; set; }

        [Column("ReferenceFlow-SQL")]
        [StringLength(36)]
        public string ReferenceFlow_SQL { get; set; }

        [StringLength(60)]
        public string RefererenceType { get; set; }

        [StringLength(60)]
        public string ProcessType { get; set; }

        [StringLength(60)]
        public string Diagram { get; set; }

        public int? FlowID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
