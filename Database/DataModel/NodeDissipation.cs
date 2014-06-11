namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeDissipation")]
    public partial class NodeDissipation
    {
        public NodeDissipation()
        {
            NodeDissipationParams = new HashSet<NodeDissipationParam>();
        }

        public int NodeDissipationID { get; set; }

        public int? FragmentNodeID { get; set; }

        public int? FlowPropertyID { get; set; }

        public double? EmisionFactor { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual FragmentNode FragmentNode { get; set; }

        public virtual ICollection<NodeDissipationParam> NodeDissipationParams { get; set; }
    }
}
