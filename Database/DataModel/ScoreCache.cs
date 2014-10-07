namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScoreCache")]
    public partial class ScoreCache : Entity
    {
        public int ScoreCacheID { get; set; }

        public int? NodeCacheID { get; set; }

        public int? LCIAMethodID { get; set; }

        public double? ImpactScore { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }

        public virtual NodeCache NodeCache { get; set; }
    }
}
