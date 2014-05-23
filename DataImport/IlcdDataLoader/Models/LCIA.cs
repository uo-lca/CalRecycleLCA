using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class LCIA
    {
        public int LCIAID { get; set; }
        public Nullable<int> LCIAMethodID { get; set; }
        public string LCIAUUID { get; set; }
        public Nullable<int> FlowID { get; set; }
        public string Location { get; set; }
        public Nullable<int> DirectionID { get; set; }
        public Nullable<double> Factor { get; set; }
        public string Flow_SQL { get; set; }
        public string Direction_SQL { get; set; }
        public virtual Direction Direction { get; set; }
        public virtual LCIAMethod LCIAMethod { get; set; }
    }
}
