using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Flow
    {
        public Flow()
        {
            this.FlowFlowProperties = new List<FlowFlowProperty>();
            this.Processes = new List<Process>();
            this.ProcessFlows = new List<ProcessFlow>();
        }

        public int FlowID { get; set; }
        public string FlowUUID { get; set; }
        public string FlowVersion { get; set; }
        public string Name { get; set; }
        public string CASNumber { get; set; }
        public Nullable<int> FlowPropertyID { get; set; }
        public Nullable<int> FlowTypeID { get; set; }
        public string FlowType_SQL { get; set; }
        public string ReferenceFlowProperty_SQL { get; set; }
        public virtual FlowProperty FlowProperty { get; set; }
        public virtual FlowType FlowType { get; set; }
        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }
        public virtual ICollection<Process> Processes { get; set; }
        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
