namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentFlow")]
    public partial class FragmentFlow
    {
        public FragmentFlow()
        {
            DependencyParams = new HashSet<DependencyParam>();
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
        }

        public int FragmentFlowID { get; set; }

        public int? FragmentID { get; set; }

        public string Name { get; set; }

        public int? FragmentStageID { get; set; }

        public int? ReferenceFlowPropertyID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public double? Quantity { get; set; }

        public int? ParentFragmentFlowID { get; set; }

        [StringLength(50)]
        public string ReferenceFlowPropertyUUID { get; set; }

        [StringLength(50)]
        public string FlowUUID { get; set; }

        public virtual ICollection<DependencyParam> DependencyParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual FragmentStage FragmentStage { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }
    }
}
