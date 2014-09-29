namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BackgroundCache")]
    public partial class BackgroundCache : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BackgroundCacheID { get; set; }

        public int? BackgroundID { get; set; }

        public int? ScenarioID { get; set; }

        public int? LCIAMethodID { get; set; }

        public double? Score { get; set; }

        public virtual Background Background { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }

        public virtual Scenario Scenario { get; set; }
    }
}
