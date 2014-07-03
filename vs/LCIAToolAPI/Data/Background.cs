namespace Data
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
            BackgroundCaches = new HashSet<BackgroundCache>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BackgroundID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? TargetID { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<BackgroundCache> BackgroundCaches { get; set; }
    }
}