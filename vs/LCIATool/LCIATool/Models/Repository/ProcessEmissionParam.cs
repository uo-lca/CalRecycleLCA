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
    
    public partial class ProcessEmissionParam
    {
        public int ProcessEmissionParamID { get; set; }
        public Nullable<int> ParamID { get; set; }
        public Nullable<int> ProcessFlowID { get; set; }
    
        public virtual Param Param { get; set; }
        public virtual ProcessFlow ProcessFlow { get; set; }
    }
}