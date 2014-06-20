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
    
    public partial class NodeDissipation
    {
        public NodeDissipation()
        {
            this.NodeDissipationParams = new HashSet<NodeDissipationParam>();
        }
    
        public int NodeDissipationID { get; set; }
        public Nullable<int> FragmentNodeID { get; set; }
        public Nullable<int> FlowPropertyID { get; set; }
        public Nullable<double> EmisionFactor { get; set; }
    
        public virtual FlowProperty FlowProperty { get; set; }
        public virtual FragmentNode FragmentNode { get; set; }
        public virtual ICollection<NodeDissipationParam> NodeDissipationParams { get; set; }
    }
}