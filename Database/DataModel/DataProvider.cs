namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataProvider")]
    public partial class DataProvider : Entity
    {
        public DataProvider()
        {
            ILCDEntities = new HashSet<ILCDEntity>();
        }

        public int DataProviderID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string DirName { get; set; }

        public virtual ICollection<ILCDEntity> ILCDEntities { get; set; }
    }
}
