namespace LcaDataModel
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
            FragmentFlows = new HashSet<FragmentFlow>();
            FragmentStages = new HashSet<FragmentStage>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentID { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }
        
        public virtual ILCDEntity ILCDEntity { get; set; }
        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<FragmentStage> FragmentStages { get; set; }
    }
}