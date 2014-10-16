namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataSource")]
    public partial class DataSource : Entity
    {
        public DataSource()
        {
            ILCDEntities = new HashSet<ILCDEntity>();
        }

        public int DataSourceID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string DirName { get; set; }

        public virtual ICollection<ILCDEntity> ILCDEntities { get; set; }
    }
}
