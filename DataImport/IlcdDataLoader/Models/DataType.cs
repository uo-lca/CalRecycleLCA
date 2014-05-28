using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class DataType
    {
        public DataType()
        {
            this.CategorySystems = new List<CategorySystem>();
        }

        public int DataTypeID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CategorySystem> CategorySystems { get; set; }
    }
}
