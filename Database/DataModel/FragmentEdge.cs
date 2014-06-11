namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentEdge")]
    public partial class FragmentEdge
    {
        public FragmentEdge()
        {
            DependencyParams = new HashSet<DependencyParam>();
        }

        public int FragmentEdgeID { get; set; }

        public int? FragmentID { get; set; }

        public int? Origin { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? Terminus { get; set; }

        public double? Quantity { get; set; }

        public virtual ICollection<DependencyParam> DependencyParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual FragmentEdge FragmentEdge1 { get; set; }

        public virtual FragmentEdge FragmentEdge2 { get; set; }

        public virtual FragmentNode FragmentNode { get; set; }

        public virtual FragmentNode FragmentNode1 { get; set; }
    }
}
