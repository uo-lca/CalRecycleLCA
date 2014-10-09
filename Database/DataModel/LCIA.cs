namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCIA")]
    public partial class LCIA : Entity
    {
        public LCIA()
        {
            CharacterizationParams = new HashSet<CharacterizationParam>();
        }

        public int LCIAID { get; set; }

        public int? LCIAMethodID { get; set; }

        public int? FlowID { get; set; }

        [StringLength(36)]
        public string FlowUUID { get; set; }

        [StringLength(255)]
        public string FlowName { get; set; }

        [StringLength(100)]
        public string Geography { get; set; }

        public int? DirectionID { get; set; }

        public double? Factor { get; set; }

        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual LCIAMethod LCIAMethod { get; set; }
    }
}
