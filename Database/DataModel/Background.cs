namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Background")]
    public partial class Background
    {
        public Background()
        {
            BackgroundFragments = new HashSet<BackgroundFragment>();
        }

        public int BackgroundID { get; set; }

        public int? FlowID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? TargetID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<BackgroundFragment> BackgroundFragments { get; set; }
    }
}
