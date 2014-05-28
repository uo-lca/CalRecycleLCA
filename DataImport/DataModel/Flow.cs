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
            FlowFlowProperties = new HashSet<FlowFlowProperty>();
            Processes = new HashSet<Process>();
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public int FlowID { get; set; }

        [StringLength(36)]
        public string FlowUUID { get; set; }

        [StringLength(15)]
        public string FlowVersion { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(15)]
        public string CASNumber { get; set; }

        public int? FlowPropertyID { get; set; }

        public int? FlowTypeID { get; set; }

        [StringLength(200)]
        public string FlowType_SQL { get; set; }

        [StringLength(36)]
        public string ReferenceFlowProperty_SQL { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual FlowType FlowType { get; set; }

        public virtual ICollection<FlowFlowProperty> FlowFlowProperties { get; set; }

        public virtual ICollection<Process> Processes { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
