namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Classification")]
    public partial class Classification
    {
        public int ClassificationID { get; set; }

        [StringLength(36)]
        public string ClassificationUUID { get; set; }

        public int? CategorySystemID { get; set; }

        public int? ClassID { get; set; }

        [Column("ClassID-SQL")]
        [StringLength(100)]
        public string ClassID_SQL { get; set; }

        [Column("CategorySystem-SQL")]
        [StringLength(200)]
        public string CategorySystem_SQL { get; set; }

        public virtual CategorySystem CategorySystem { get; set; }

        public virtual Class Class { get; set; }
    }
}
