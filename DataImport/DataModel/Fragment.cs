namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fragment")]
    public partial class Fragment
    {
        public int FragmentID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public bool? Background { get; set; }
    }
}
