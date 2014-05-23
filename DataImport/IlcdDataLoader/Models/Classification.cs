using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Classification
    {
        public int ClassificationID { get; set; }
        public string ClassificationUUID { get; set; }
        public Nullable<int> CategorySystemID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public string ClassID_SQL { get; set; }
        public string CategorySystem_SQL { get; set; }
        public virtual CategorySystem CategorySystem { get; set; }
        public virtual Class Class { get; set; }
    }
}
