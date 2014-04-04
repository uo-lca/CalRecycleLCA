//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LCIATool.Models.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Process
    {
        public Process()
        {
            this.ProcessFlows = new HashSet<ProcessFlow>();
        }
    
        public int ProcessID { get; set; }
        public string ProcessUUID { get; set; }
        public string ProcessVersion { get; set; }
        public string Process1 { get; set; }
        public string Year { get; set; }
        public string Geography { get; set; }
        public string ReferenceFlow_SQL { get; set; }
        public string RefererenceType { get; set; }
        public string ProcessType { get; set; }
        public string Diagram { get; set; }
        public Nullable<int> ProcessFlowID { get; set; }
    
        public virtual Flow Flow { get; set; }
        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
