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
    
    public partial class UnitGroup
    {
        public UnitGroup()
        {
            this.UnitConversions = new HashSet<UnitConversion>();
            this.FlowProperties = new HashSet<FlowProperty>();
        }
    
        public int UnitGroupID { get; set; }
        public string UnitGroupUUID { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string ReferenceUnit { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool Voided { get; set; }
    
        public virtual ICollection<UnitConversion> UnitConversions { get; set; }
        public virtual ICollection<FlowProperty> FlowProperties { get; set; }
    }
}
