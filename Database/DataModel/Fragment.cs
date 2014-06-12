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
            BackgroundFragments = new HashSet<BackgroundFragment>();
            FragmentEdges = new HashSet<FragmentEdge>();
            FragmentStages = new HashSet<FragmentStage>();
        }

        public int FragmentID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? RootNode { get; set; }

        public int? ReferenceFlow { get; set; }

        public virtual ICollection<BackgroundFragment> BackgroundFragments { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<FragmentEdge> FragmentEdges { get; set; }

        public virtual ICollection<FragmentStage> FragmentStages { get; set; }
    }
}
