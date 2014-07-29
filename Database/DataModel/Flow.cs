namespace LcaDataModel
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
            FlowFlowProperties = new HashSet<FlowFlowProperty>();
            FlowPropertyEmissions = new HashSet<FlowPropertyEmission>();
            FragmentFlows = new HashSet<FragmentFlow>();
            LCIAs = new HashSet<LCIA>();
            Processes = new HashSet<Process>();
            ProcessFlows = new HashSet<ProcessFlow>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
            FragmentNodeProcesses = new HashSet<FragmentNodeProcess>();
            FragmentNodeFragments = new HashSet<FragmentNodeFragment>();

        }

        public int FlowID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(15)]
        public string CASNumber { get; set; }

        public int? ReferenceFlowProperty { get; set; }

        public int? FlowTypeID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<CompostionModel> CompostionModels { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual FlowType FlowType { get; set; }

        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }

        public virtual ICollection<FlowPropertyEmission> FlowPropertyEmissions { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<FragmentNodeProcess> FragmentNodeProcesses { get; set; }

        public virtual ICollection<FragmentNodeFragment> FragmentNodeFragments { get; set; }

        public virtual ICollection<LCIA> LCIAs { get; set; }

        public virtual ICollection<Process> Processes { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }
    }
}
