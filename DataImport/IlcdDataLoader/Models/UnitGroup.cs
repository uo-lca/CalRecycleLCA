using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class UnitGroup
    {
        public UnitGroup()
        {
            this.FlowProperties = new List<FlowProperty>();
            this.UnitConversions = new List<UnitConversion>();
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
        public virtual ICollection<FlowProperty> FlowProperties { get; set; }
        public virtual ICollection<UnitConversion> UnitConversions { get; set; }
    }
}
