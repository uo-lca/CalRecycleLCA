namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BackgroundFragment")]
    public partial class BackgroundFragment
    {
        public int BackgroundFragmentID { get; set; }

        public int? BackgroundID { get; set; }

        public int? FragmentID { get; set; }

        public virtual Background Background { get; set; }

        public virtual Fragment Fragment { get; set; }
    }
}
