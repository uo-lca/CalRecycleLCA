namespace Data.Mappings
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
            BackgroundProcesses = new HashSet<BackgroundProcess>();
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public int ProcessID { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(60)]
        public string Year { get; set; }

        [StringLength(15)]
        public string Geography { get; set; }

        public int? ReferenceTypeID { get; set; }

        public int? ProcessTypeID { get; set; }

        public int? ReferenceFlowID { get; set; }

        public virtual ICollection<BackgroundProcess> BackgroundProcesses { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ProcessType ProcessType { get; set; }

        public virtual ReferenceType ReferenceType { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
