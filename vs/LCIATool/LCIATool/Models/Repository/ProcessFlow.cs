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
    
    public partial class ProcessFlow
    {
        public int ProcessFlowID { get; set; }
        public string ProcessUUID { get; set; }
        public Nullable<int> ProcessID { get; set; }
        public Nullable<int> FlowID { get; set; }
        public Nullable<int> DirectionID { get; set; }
        public string Type { get; set; }
        public string VarName { get; set; }
        public Nullable<double> Magnitude { get; set; }
        public Nullable<double> Result { get; set; }
        public Nullable<double> STDev { get; set; }
        public string Flow_SQL { get; set; }
        public string Direction_SQL { get; set; }
        public Nullable<int> Ind_SQL { get; set; }
        public string Geography { get; set; }
    
        public virtual Direction Direction { get; set; }
        public virtual Flow Flow { get; set; }
        public virtual Process Process { get; set; }
    }
}
