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
            VisibilityID = Convert.ToInt32(VisibilityEnum.Public);  // Default value needed for seeding the table
            ILCDEntities = new HashSet<ILCDEntity>();
        }

        public int DataSourceID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string DirName { get; set; }

        public int VisibilityID { get; set; }

        public virtual ICollection<ILCDEntity> ILCDEntities { get; set; }
    }
}
