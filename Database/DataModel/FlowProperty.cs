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
            FlowFlowProperties = new HashSet<FlowFlowProperty>();
            FlowPropertyEmissions = new HashSet<FlowPropertyEmission>();
            FragmentFlows = new HashSet<FragmentFlow>();
            LCIAMethods = new HashSet<LCIAMethod>();
        }

        public int FlowPropertyID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? UnitGroupID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }

        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }

        public virtual ICollection<FlowPropertyEmission> FlowPropertyEmissions { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }
    }
}
