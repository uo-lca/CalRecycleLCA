namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BackgroundProcess")]
    public partial class BackgroundProcess
    {
        public int BackgroundProcessID { get; set; }

        public int? BackgroundID { get; set; }

        public int? ProcessID { get; set; }

        public virtual Process Process { get; set; }
    }
}
