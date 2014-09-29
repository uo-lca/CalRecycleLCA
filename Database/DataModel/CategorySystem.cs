namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CategorySystem")]
    public partial class CategorySystem : Entity
    {
        public CategorySystem()
        {
            Categories = new HashSet<Category>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategorySystemID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? DataTypeID { get; set; }

        [StringLength(4)]
        public string Delimiter { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual DataType DataType { get; set; }
    }
}
