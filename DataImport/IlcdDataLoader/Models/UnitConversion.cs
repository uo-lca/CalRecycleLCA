using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
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
        public virtual UnitGroup UnitGroup { get; set; }
    }
}
