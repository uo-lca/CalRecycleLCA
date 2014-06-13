namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentStage")]
    public partial class FragmentStage
    {
        public FragmentStage()
        {
            FragmentNodes = new HashSet<FragmentNode>();
            FragmentNodes1 = new HashSet<FragmentNode>();
        }

        public int FragmentStageID { get; set; }

        public int? FragmentID { get; set; }

        [StringLength(255)]
        public string StageName { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual ICollection<FragmentNode> FragmentNodes { get; set; }

        public virtual ICollection<FragmentNode> FragmentNodes1 { get; set; }
    }
}
