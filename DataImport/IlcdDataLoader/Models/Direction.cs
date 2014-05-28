using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Direction
    {
        public Direction()
        {
            this.LCIAs = new List<LCIA>();
            this.ProcessFlows = new List<ProcessFlow>();
        }

        public int DirectionID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<LCIA> LCIAs { get; set; }
        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
