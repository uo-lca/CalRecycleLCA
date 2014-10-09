namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataType")]
    public partial class DataType : Entity
    {
        public DataType()
        {
            CategorySystems = new HashSet<CategorySystem>();
            ILCDEntities = new HashSet<ILCDEntity>();
        }

        public int DataTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<CategorySystem> CategorySystems { get; set; }

        public virtual ICollection<ILCDEntity> ILCDEntities { get; set; }
    }
}
