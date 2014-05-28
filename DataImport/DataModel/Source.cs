namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Source")]
    public partial class Source
    {
        public int SourceID { get; set; }

        [StringLength(36)]
        public string SourceUUID { get; set; }

        [StringLength(15)]
        public string SourceVersion { get; set; }

        [Column("Source")]
        [StringLength(255)]
        public string Source1 { get; set; }

        [StringLength(60)]
        public string Citation { get; set; }

        [StringLength(60)]
        public string PubType { get; set; }

        [StringLength(255)]
        public string URI { get; set; }
    }
}
