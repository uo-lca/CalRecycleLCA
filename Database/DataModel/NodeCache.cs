namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeCache")]
    public partial class NodeCache
    {
        public NodeCache()
        {
            ScoreCaches = new HashSet<ScoreCache>();
        }

        public int NodeCacheID { get; set; }

        public int? FragmentFlowID { get; set; }

        public int? ScenarioID { get; set; }

        public double? NodeWeight { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Scenario Scenario { get; set; }

        public virtual ICollection<ScoreCache> ScoreCaches { get; set; }
    }
}
