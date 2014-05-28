namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CategorySystem")]
    public partial class CategorySystem
    {
        public CategorySystem()
        {
            Categories = new HashSet<Category>();
            Classifications = new HashSet<Classification>();
        }

        public int CategorySystemID { get; set; }

        [Column("CategorySystem")]
        [StringLength(100)]
        public string CategorySystem1 { get; set; }

        [StringLength(255)]
        public string URI { get; set; }

        public int? DataTypeID { get; set; }

        [StringLength(4)]
        public string Delimeter { get; set; }

        [Column("DataType-SQL")]
        [StringLength(100)]
        public string DataType_SQL { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual ICollection<Classification> Classifications { get; set; }
    }
}
