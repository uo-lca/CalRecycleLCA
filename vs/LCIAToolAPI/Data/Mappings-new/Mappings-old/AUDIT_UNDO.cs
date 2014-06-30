namespace Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AUDIT_UNDO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AUDIT_LOG_TRANSACTION_ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(261)]
        public string TABLE_NAME { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(261)]
        public string TABLE_SCHEMA { get; set; }

        [StringLength(6)]
        public string ACTION_NAME { get; set; }

        [Key]
        [Column(Order = 3)]
        public string HOST_NAME { get; set; }

        [Key]
        [Column(Order = 4)]
        public string APP_NAME { get; set; }

        [Key]
        [Column(Order = 5)]
        public string MODIFIED_BY { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime MODIFIED_DATE { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AFFECTED_ROWS { get; set; }

        public int? AUDIT_LOG_DATA_ID { get; set; }

        [StringLength(1500)]
        public string PRIMARY_KEY { get; set; }

        [StringLength(128)]
        public string COL_NAME { get; set; }

        [StringLength(4000)]
        public string OLD_VALUE { get; set; }

        [StringLength(4000)]
        public string NEW_VALUE { get; set; }

        [StringLength(1)]
        public string DATA_TYPE { get; set; }
    }
}
