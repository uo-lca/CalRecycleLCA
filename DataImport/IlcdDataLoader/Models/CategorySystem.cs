using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class CategorySystem
    {
        public CategorySystem()
        {
            this.Categories = new List<Category>();
            this.Classifications = new List<Classification>();
        }

        public int CategorySystemID { get; set; }
        public string CategorySystem1 { get; set; }
        public string URI { get; set; }
        public Nullable<int> DataTypeID { get; set; }
        public string Delimeter { get; set; }
        public string DataType_SQL { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual DataType DataType { get; set; }
        public virtual ICollection<Classification> Classifications { get; set; }
    }
}
