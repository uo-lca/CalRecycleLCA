namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Scenario")]
    public partial class Scenario : Entity
    {
        public const int MODEL_BASE_CASE_ID = 1;

        public Scenario()
        {
            BackgroundCaches = new HashSet<BackgroundCache>();
            NodeCaches = new HashSet<NodeCache>();
            BackgroundSubstitutions = new HashSet<BackgroundSubstitution>();
            CompositionSubstitutions = new HashSet<CompositionSubstitution>();
            Params = new HashSet<Param>();
            FragmentSubstitutions = new HashSet<FragmentSubstitution>();
            ProcessSubstitutions = new HashSet<ProcessSubstitution>();
            ScoreCaches = new HashSet<ScoreCache>();
        }

        public int ScenarioID { get; set; }

        public int ScenarioGroupID { get; set; }

        public int TopLevelFragmentID { get; set; }

        public double ActivityLevel { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int FlowID { get; set; }

        public int DirectionID { get; set; }

        public virtual ICollection<BackgroundCache> BackgroundCaches { get; set; }

        public virtual ICollection<NodeCache> NodeCaches { get; set; }

        public virtual ScenarioGroup ScenarioGroup { get; set; }

        public virtual Fragment TopLevelFragment { get; set; }

        public virtual ICollection<BackgroundSubstitution> BackgroundSubstitutions { get; set; }

        public virtual ICollection<CompositionSubstitution> CompositionSubstitutions { get; set; }

        public virtual ICollection<Param> Params { get; set; }

        public virtual ICollection<FragmentSubstitution> FragmentSubstitutions { get; set; }

        public virtual ICollection<ProcessSubstitution> ProcessSubstitutions { get; set; }

        public virtual ICollection<ScoreCache> ScoreCaches { get; set; }

    }
}
