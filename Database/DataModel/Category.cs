namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        public int CategoryID { get; set; }

        public int? CategorySystemID { get; set; }

        public int? ClassID { get; set; }

        public int? ParentClassID { get; set; }

        [Column("DataTypeID-notneededremovelater")]
        public int? DataTypeID_notneededremovelater { get; set; }

        public int? HierarchyLevel { get; set; }

        [StringLength(250)]
        public string Hier { get; set; }

        [Column("ClassID-SQL")]
        [StringLength(60)]
        public string ClassID_SQL { get; set; }

        [Column("Parent-SQL")]
        [StringLength(60)]
        public string Parent_SQL { get; set; }

        [Column("ClassName-SQL")]
        [StringLength(100)]
        public string ClassName_SQL { get; set; }

        public virtual CategorySystem CategorySystem { get; set; }

        public virtual Class Class { get; set; }

        public virtual Class Class1 { get; set; }
    }
}
