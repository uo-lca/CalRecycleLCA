namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentFlow")]
    public partial class FragmentFlow : Entity
    {
        public FragmentFlow()
        {
            DependencyParams = new HashSet<DependencyParam>();
            Fragments = new HashSet<Fragment>();
            FragmentFlow1 = new HashSet<FragmentFlow>();
            FragmentNodeFragments = new HashSet<FragmentNodeFragment>();
            FragmentNodeProcesses = new HashSet<FragmentNodeProcess>();
            NodeCaches = new HashSet<NodeCache>();
            NodeDissipationParams = new HashSet<NodeDissipationParam>();
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentFlowID { get; set; }

        public int? FragmentID { get; set; }

        public string Name { get; set; }

        // Short name for visualization
        [StringLength(30)]
        public string ShortName { get; set; }

        public int? FragmentStageID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? ParentFragmentFlowID { get; set; }

        public virtual ICollection<DependencyParam> DependencyParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<Fragment> Fragments { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlow1 { get; set; }

        public virtual FragmentFlow FragmentFlow2 { get; set; }

        public virtual FragmentStage FragmentStage { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        public virtual ICollection<FragmentNodeProcess> FragmentNodeProcesses { get; set; }

        public virtual ICollection<NodeCache> NodeCaches { get; set; }

        public virtual ICollection<NodeDissipationParam> NodeDissipationParams { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }
    }
}
