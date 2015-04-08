namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowProperty")]
    public partial class FlowProperty : Entity
    {
        public FlowProperty()
        {
            CompositionDataSet = new HashSet<CompositionData>();
            Flows = new HashSet<Flow>();
            FlowFlowProperties = new HashSet<FlowFlowProperty>();
            FlowPropertyEmissions = new HashSet<FlowPropertyEmission>();
            LCIAMethods = new HashSet<LCIAMethod>();
            //ConservationParams = new HashSet<ConservationParam>();
        }

        public int FlowPropertyID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? UnitGroupID { get; set; }

        public int ILCDEntityID { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }

        public virtual ICollection<CompositionData> CompositionDataSet { get; set; }

        public virtual ICollection<FlowPropertyEmission> FlowPropertyEmissions { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    
        //public virtual ICollection<ConservationParam> ConservationParams { get; set; }

    }
}
