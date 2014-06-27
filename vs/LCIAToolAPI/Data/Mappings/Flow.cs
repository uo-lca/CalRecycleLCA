namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Flow")]
    public partial class Flow
    {
        public Flow()
        {
            Backgrounds = new HashSet<Background>();
            CompostionModels = new HashSet<CompostionModel>();
            FlowPropertyEmissions = new HashSet<FlowPropertyEmission>();
            FlowFlowProperties = new HashSet<FlowFlowProperty>();
            FragmentFlows = new HashSet<FragmentFlow>();
            Processes = new HashSet<Process>();
            ProcessFlows = new HashSet<ProcessFlow>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
        }

        public int FlowID { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(15)]
        public string CASNumber { get; set; }

        public int? ReferenceFlowProperty { get; set; }

        public int? FlowTypeID { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<CompostionModel> CompostionModels { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual FlowType FlowType { get; set; }

        public virtual ICollection<FlowPropertyEmission> FlowPropertyEmissions { get; set; }

        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<Process> Processes { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }
    }
}
