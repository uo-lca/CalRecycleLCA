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
    
    public partial class ReferenceType
    {
        public ReferenceType()
        {
            this.Processes = new HashSet<Process>();
        }
    
        public int ReferenceTypeID { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Process> Processes { get; set; }
    }
}
