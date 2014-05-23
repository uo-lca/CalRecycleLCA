using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class FlowType
    {
        public FlowType()
        {
            this.Flows = new List<Flow>();
        }

        public int FlowTypeID { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Flow> Flows { get; set; }
    }
}
