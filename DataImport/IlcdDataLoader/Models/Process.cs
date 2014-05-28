using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Process
    {
        public Process()
        {
            this.ProcessFlows = new List<ProcessFlow>();
        }

        public int ProcessID { get; set; }
        public string ProcessUUID { get; set; }
        public string ProcessVersion { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Geography { get; set; }
        public string ReferenceFlow_SQL { get; set; }
        public string RefererenceType { get; set; }
        public string ProcessType { get; set; }
        public string Diagram { get; set; }
        public Nullable<int> FlowID { get; set; }
        public virtual Flow Flow { get; set; }
        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
