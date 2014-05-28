namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowFlowProperty")]
    public partial class FlowFlowProperty
    {
        [Key]
        public int FlowPropertyVersionID { get; set; }

        [StringLength(36)]
        public string FlowPropertyVersionUUID { get; set; }

        public int? FlowID { get; set; }

        public int? FlowPropertyID { get; set; }

        public double? MeanValue { get; set; }

        public double? StDev { get; set; }

        [Column("FlowProperty-SQL")]
        [StringLength(36)]
        public string FlowProperty_SQL { get; set; }

        [Column("FlowReference-SQL")]
        [StringLength(36)]
        public string FlowReference_SQL { get; set; }

        [Column("Ind-SQL")]
        public int? Ind_SQL { get; set; }

        public virtual Flow Flow { get; set; }
    }
}
