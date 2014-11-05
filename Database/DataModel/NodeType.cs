namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeType")]
    public partial class NodeType : Entity
    {
        public NodeType()
        {
            Backgrounds = new HashSet<Background>();
            FragmentFlows = new HashSet<FragmentFlow>();
            BackgroundSubstitutions = new HashSet<BackgroundSubstitution>();
        }

        public int NodeTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<BackgroundSubstitution> BackgroundSubstitutions { get; set; }
    }
}
