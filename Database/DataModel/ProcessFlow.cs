namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessFlow")]
    public partial class ProcessFlow
    {
        public int ProcessFlowID { get; set; }

        [StringLength(36)]
        public string ProcessUUID { get; set; }

        public int? ProcessID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        [StringLength(15)]
        public string Type { get; set; }

        [StringLength(15)]
        public string VarName { get; set; }

        public double? Magnitude { get; set; }

        public double? Result { get; set; }

        public double? STDev { get; set; }

        [Column("Flow-SQL")]
        [StringLength(50)]
        public string Flow_SQL { get; set; }

        [Column("Direction-SQL")]
        [StringLength(50)]
        public string Direction_SQL { get; set; }

        [Column("Ind-SQL")]
        public int? Ind_SQL { get; set; }

        [StringLength(15)]
        public string Geography { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual Process Process { get; set; }
    }
}
