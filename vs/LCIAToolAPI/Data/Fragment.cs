namespace Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fragment")]
    public partial class Fragment
    {
        public Fragment()
        {
        //    FragmentFlows = new HashSet<FragmentFlow>();
        //    FragmentNodeFragments = new HashSet<FragmentNodeFragment>();
        //    FragmentStages = new HashSet<FragmentStage>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        //public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        //public virtual ICollection<FragmentStage> FragmentStages { get; set; }
    }
}
