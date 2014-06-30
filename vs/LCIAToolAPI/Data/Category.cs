namespace Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            Category1 = new HashSet<Category>();
            Classifications = new HashSet<Classification>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        [StringLength(60)]
        public string ExternalClassID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public int? CategorySystemID { get; set; }

        public int? ParentCategoryID { get; set; }

        public int? HierarchyLevel { get; set; }

        public virtual ICollection<Category> Category1 { get; set; }

        public virtual Category Category2 { get; set; }

        public virtual CategorySystem CategorySystem { get; set; }

        public virtual ICollection<Classification> Classifications { get; set; }
    }
}
