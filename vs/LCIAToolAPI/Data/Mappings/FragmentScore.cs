namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentScore")]
    public partial class FragmentScore
    {
        public int FragmentScoreID { get; set; }

        public int? FragmentID { get; set; }

        public int? LCIAMethodID { get; set; }

        public int? FragmentNodeStageID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public double? ImpactScore { get; set; }
    }
}
