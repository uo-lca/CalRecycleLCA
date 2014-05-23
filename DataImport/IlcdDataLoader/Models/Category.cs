using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Category
    {
        public int CategoryID { get; set; }
        public Nullable<int> CategorySystemID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> ParentClassID { get; set; }
        public Nullable<int> DataTypeID_notneededremovelater { get; set; }
        public Nullable<int> HierarchyLevel { get; set; }
        public string Hier { get; set; }
        public string ClassID_SQL { get; set; }
        public string Parent_SQL { get; set; }
        public string ClassName_SQL { get; set; }
        public virtual CategorySystem CategorySystem { get; set; }
        public virtual Class Class { get; set; }
        public virtual Class Class1 { get; set; }
    }
}
