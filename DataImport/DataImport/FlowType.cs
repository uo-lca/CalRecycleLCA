//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataImport
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlowType
    {
        public FlowType()
        {
            this.Flows = new HashSet<Flow>();
        }
    
        public int FlowTypeID { get; set; }
        public string Type { get; set; }
    
        public virtual ICollection<Flow> Flows { get; set; }
    }
}
