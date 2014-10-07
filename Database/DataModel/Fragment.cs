namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fragment")]
    public partial class Fragment : Entity
    {
        public Fragment()
        {
            FragmentFlows = new HashSet<FragmentFlow>();
            FragmentNodeFragments = new HashSet<FragmentNodeFragment>();
            FragmentStages = new HashSet<FragmentStage>();
            FragmentSubstitutions = new HashSet<FragmentSubstitution>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        public virtual ICollection<FragmentStage> FragmentStages { get; set; }

        public virtual ICollection<FragmentSubstitution> FragmentSubstitutions { get; set; }
    }
}
