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
        public Scenario()
        {
            BackgroundCaches = new HashSet<BackgroundCache>();
            NodeCaches = new HashSet<NodeCache>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
            Params = new HashSet<Param>();
            FragmentSubstitutions = new HashSet<FragmentSubstitution>();
            ProcessSubstitutions = new HashSet<ProcessSubstitution>();
            ScoreCaches = new HashSet<ScoreCache>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioID { get; set; }

        public int ScenarioGroupID { get; set; }

        public int TopLevelFragmentID { get; set; }

        public double ActivityLevel { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<BackgroundCache> BackgroundCaches { get; set; }

        public virtual ICollection<NodeCache> NodeCaches { get; set; }

        public virtual ScenarioGroup ScenarioGroup { get; set; }

        public virtual Fragment TopLevelFragment { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }

        public virtual ICollection<Param> Params { get; set; }

        public virtual ICollection<FragmentSubstitution> FragmentSubstitutions { get; set; }

        public virtual ICollection<ProcessSubstitution> ProcessSubstitutions { get; set; }

        public virtual ICollection<ScoreCache> ScoreCaches { get; set; }

    }
}
