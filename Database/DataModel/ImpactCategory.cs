namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImpactCategory")]
    public partial class ImpactCategory : Entity
    {
        public ImpactCategory()
        {
            LCIAMethods = new HashSet<LCIAMethod>();
        }

        public int ImpactCategoryID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
