namespace Data.Mappings
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
            FlowPropertyEmissions = new HashSet<FlowPropertyEmission>();
            LCIAMethods = new HashSet<LCIAMethod>();
        }

        public int FlowPropertyID { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? UnitGroupID { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }

        public virtual ICollection<FlowPropertyEmission> FlowPropertyEmissions { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
