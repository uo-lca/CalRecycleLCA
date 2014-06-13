namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeEmission")]
    public partial class NodeEmission
    {
        public NodeEmission()
        {
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
        }

        public int NodeEmissionID { get; set; }

        public int? FragmentNodeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public double? Quantity { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual FragmentNode FragmentNode { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }
    }
}
