namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowProperty")]
    public partial class FlowProperty
    {
        public FlowProperty()
        {
            Flows = new HashSet<Flow>();
            LCIAMethods = new HashSet<LCIAMethod>();
        }

        public int FlowPropertyID { get; set; }

        [StringLength(36)]
        public string FlowPropertyUUID { get; set; }

        [StringLength(15)]
        public string FlowPropertyVersion { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? UnitGroupID { get; set; }

        [StringLength(36)]
        public string UnitGroup_SQL { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
