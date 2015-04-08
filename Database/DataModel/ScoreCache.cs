namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScoreCache")]
    public partial class ScoreCache : Entity
    {
        //public int ScoreCacheID { get; set; }

        [Key, Column(Order = 1)]
        public int FragmentFlowID { get; set; }

        [Key, Column(Order = 2)]
        public int ScenarioID { get; set; }

        [Key, Column(Order = 3)]
        public int LCIAMethodID { get; set; }

        public double ImpactScore { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }

        public virtual Scenario Scenario { get; set; }        
    }
}
