namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCIA")]
    public partial class LCIA
    {
        public LCIA()
        {
            CharacterizationParams = new HashSet<CharacterizationParam>();
        }

        public int LCIAID { get; set; }

        public int? LCIAMethodID { get; set; }

        public int? FlowID { get; set; }

        [StringLength(100)]
        public string Geography { get; set; }

        public int? DirectionID { get; set; }

        public double? Factor { get; set; }

        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }
    }
}
