namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Class")]
    public partial class Class
    {
        public Class()
        {
            Categories = new HashSet<Category>();
            Categories1 = new HashSet<Category>();
            Classifications = new HashSet<Classification>();
        }

        public int ClassID { get; set; }

        [StringLength(60)]
        public string ExternalClassID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [Column("CategorySystemID-SQL")]
        public int? CategorySystemID_SQL { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Category> Categories1 { get; set; }

        public virtual ICollection<Classification> Classifications { get; set; }
    }
}
