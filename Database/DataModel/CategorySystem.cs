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
        public int CategorySystemID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? DataTypeID { get; set; }

        [StringLength(4)]
        public string Delimiter { get; set; }

        public virtual DataType DataType { get; set; }
    }
}
