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
    
    public partial class UnitConversion
    {
        public int UnitConversionID { get; set; }
        public string UnitConversionUUID { get; set; }
        public string Unit { get; set; }
        public Nullable<int> UnitGroupID { get; set; }
        public Nullable<double> Conversion { get; set; }
        public Nullable<int> Ind_sql { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool Voided { get; set; }
    }
}
