namespace Data.Mappings
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
            //BackgroundFragments = new HashSet<BackgroundFragment>();
            //FragmentStages = new HashSet<FragmentStage>();
        }

        public int FragmentID { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }

        //public ICollection<BackgroundFragment> BackgroundFragments { get; set; }

        //public virtual ICollection<FragmentStage> FragmentStages { get; set; }
    }
}
