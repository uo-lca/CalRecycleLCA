using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class FlowProperty
    {
        public FlowProperty()
        {
            this.Flows = new List<Flow>();
            this.LCIAMethods = new List<LCIAMethod>();
        }

        public int FlowPropertyID { get; set; }
        public string FlowPropertyUUID { get; set; }
        public string FlowPropertyVersion { get; set; }
        public string Name { get; set; }
        public Nullable<int> UnitGroupID { get; set; }
        public string UnitGroup_SQL { get; set; }
        public virtual ICollection<Flow> Flows { get; set; }
        public virtual UnitGroup UnitGroup { get; set; }
        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
