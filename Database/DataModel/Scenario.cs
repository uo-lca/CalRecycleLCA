namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Scenario")]
    public partial class Scenario
    {
        public Scenario()
        {
            BackgroundCaches = new HashSet<BackgroundCache>();
            NodeCaches = new HashSet<NodeCache>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
            ScenarioParams = new HashSet<ScenarioParam>();
            FragmentSubstitutions = new HashSet<FragmentSubstitution>();
            ProcessSubstitutions = new HashSet<ProcessSubstitution>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioID { get; set; }

        public int? ScenarioGroupID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<BackgroundCache> BackgroundCaches { get; set; }

        public virtual ICollection<NodeCache> NodeCaches { get; set; }

        public virtual ScenarioGroup ScenarioGroup { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }

        public virtual ICollection<ScenarioParam> ScenarioParams { get; set; }

        public virtual ICollection<FragmentSubstitution> FragmentSubstitutions { get; set; }

        public virtual ICollection<ProcessSubstitution> ProcessSubstitutions { get; set; }
    }
}
