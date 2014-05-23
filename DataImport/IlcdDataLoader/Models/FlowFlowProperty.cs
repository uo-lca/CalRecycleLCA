using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class FlowFlowProperty
    {
        public int FlowPropertyVersionID { get; set; }
        public string FlowPropertyVersionUUID { get; set; }
        public Nullable<int> FlowID { get; set; }
        public Nullable<int> FlowPropertyID { get; set; }
        public Nullable<double> MeanValue { get; set; }
        public Nullable<double> StDev { get; set; }
        public string FlowProperty_SQL { get; set; }
        public string FlowReference_SQL { get; set; }
        public Nullable<int> Ind_SQL { get; set; }
        public virtual Flow Flow { get; set; }
    }
}
