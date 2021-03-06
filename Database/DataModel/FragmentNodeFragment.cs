namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNodeFragment")]
    public partial class FragmentNodeFragment : Entity
    {
        public FragmentNodeFragment() {
            FragmentSubstitutions = new HashSet<FragmentSubstitution>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentNodeFragmentID { get; set; }

        public int FragmentFlowID { get; set; }

        public int SubFragmentID { get; set; }

        public int FlowID { get; set; }

        public bool Descend { get; set; } // might 

        public virtual Flow Flow { get; set; }

        public virtual Fragment SubFragment { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }
        
        public virtual ICollection<FragmentSubstitution> FragmentSubstitutions { get; set; }
    }
}
