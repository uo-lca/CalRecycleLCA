namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Process")]
    public partial class Process : Entity
    {
        public Process()
        {
            FragmentNodeProcesses = new HashSet<FragmentNodeProcess>();
            ProcessCompositions = new HashSet<ProcessComposition>();
            ProcessFlows = new HashSet<ProcessFlow>();
            ProcessSubstitutions = new HashSet<ProcessSubstitution>();
        }

        public int ProcessID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(60)]
        public string ReferenceYear { get; set; }

        [StringLength(15)]
        public string Geography { get; set; }

        public int? ReferenceTypeID { get; set; }

        public int? ProcessTypeID { get; set; }

        public int? ReferenceFlowID { get; set; }

        public int ILCDEntityID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<FragmentNodeProcess> FragmentNodeProcesses { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ProcessType ProcessType { get; set; }

        public virtual ReferenceType ReferenceType { get; set; }

        public virtual ICollection<ProcessComposition> ProcessCompositions { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
        
        public virtual ICollection<ProcessSubstitution> ProcessSubstitutions { get; set; }
    }
}
