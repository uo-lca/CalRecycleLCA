namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNode")]
    public partial class FragmentNode
    {
        public FragmentNode()
        {
            FragmentEdges = new HashSet<FragmentEdge>();
            FragmentEdges1 = new HashSet<FragmentEdge>();
            FragmentNodeFragments = new HashSet<FragmentNodeFragment>();
            NodeDissipations = new HashSet<NodeDissipation>();
            NodeEmissions = new HashSet<NodeEmission>();
        }

        public int FragmentNodeID { get; set; }

        public int? FragmentID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        public int? FragmentStageID { get; set; }

        public int? ReferenceProperty { get; set; }

        public int? NodeTypeID { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual ICollection<FragmentEdge> FragmentEdges { get; set; }

        public virtual ICollection<FragmentEdge> FragmentEdges1 { get; set; }

        public virtual FragmentStage FragmentStage { get; set; }

        public virtual FragmentStage FragmentStage1 { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        public virtual ICollection<NodeDissipation> NodeDissipations { get; set; }

        public virtual ICollection<NodeEmission> NodeEmissions { get; set; }
    }
}
