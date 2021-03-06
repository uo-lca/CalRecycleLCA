namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
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
            //ConservationParams = new HashSet<ConservationParam>();
            ChildFragmentFlows = new HashSet<FragmentFlow>();
            FragmentNodeFragments = new HashSet<FragmentNodeFragment>();
            FragmentNodeProcesses = new HashSet<FragmentNodeProcess>();
            NodeCaches = new HashSet<NodeCache>();
            ScoreCaches = new HashSet<ScoreCache>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentFlowID { get; set; }

        public int FragmentID { get; set; }

        public string Name { get; set; }

        // Short name for visualization
        [StringLength(30)]
        public string ShortName { get; set; }

        public int? FragmentStageID { get; set; }

        public int NodeTypeID { get; set; }

        public int FlowID { get; set; }

        public int DirectionID { get; set; }

        public int? ParentFragmentFlowID { get; set; }

        public virtual ICollection<DependencyParam> DependencyParams { get; set; }
        //public virtual ICollection<ConservationParam> ConservationParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual ICollection<FragmentFlow> ChildFragmentFlows { get; set; }

        public virtual FragmentFlow ParentFragmentFlow { get; set; }

        public virtual FragmentStage FragmentStage { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        public virtual ICollection<FragmentNodeProcess> FragmentNodeProcesses { get; set; }

        public virtual ICollection<NodeCache> NodeCaches { get; set; }

        public virtual ICollection<ScoreCache> ScoreCaches { get; set; }
    }
}
