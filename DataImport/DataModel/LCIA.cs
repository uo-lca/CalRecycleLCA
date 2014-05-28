namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCIA")]
    public partial class LCIA
    {
        public int LCIAID { get; set; }

        public int? LCIAMethodID { get; set; }

        [StringLength(36)]
        public string LCIAUUID { get; set; }

        public int? FlowID { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public int? DirectionID { get; set; }

        public double? Factor { get; set; }

        [Column("Flow-SQL")]
        [StringLength(36)]
        public string Flow_SQL { get; set; }

        [Column("Direction-SQL")]
        [StringLength(100)]
        public string Direction_SQL { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }
    }
}
