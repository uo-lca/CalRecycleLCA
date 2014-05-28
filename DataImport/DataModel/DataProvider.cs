namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataProvider")]
    public partial class DataProvider
    {
        public int DataProviderID { get; set; }

        [StringLength(36)]
        public string DataProviderUUID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}
