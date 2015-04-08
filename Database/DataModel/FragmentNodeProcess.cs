namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNodeProcess")]
    public partial class FragmentNodeProcess : Entity
    {
        public FragmentNodeProcess() {
            ProcessSubstitutions = new HashSet<ProcessSubstitution>();
        } 
        
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentNodeProcessID { get; set; }

        public int FragmentFlowID { get; set; }

        public int ProcessID { get; set; }

        public int FlowID { get; set; }

        public int? ConservationFragmentFlowID { get; set; } // not a foreign key, just an annotation

        public virtual Flow Flow { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Process Process { get; set; }
        
        public virtual ICollection<ProcessSubstitution> ProcessSubstitutions { get; set; }

    }
}
