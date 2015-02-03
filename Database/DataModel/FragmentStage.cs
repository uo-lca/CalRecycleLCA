namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentStage")]
    public partial class FragmentStage : Entity
    {
        public FragmentStage()
        {
            FragmentFlows = new HashSet<FragmentFlow>();
        }

        public int FragmentStageID { get; set; }

        public int FragmentID { get; set; }

        [StringLength(255)]
        public string Name { get; set; } 

        public virtual Fragment Fragment { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }
    }
}
