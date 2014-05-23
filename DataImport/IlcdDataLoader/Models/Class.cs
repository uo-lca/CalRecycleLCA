using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Class
    {
        public Class()
        {
            this.Categories = new List<Category>();
            this.Categories1 = new List<Category>();
            this.Classifications = new List<Classification>();
        }

        public int ClassID { get; set; }
        public string ExternalClassID { get; set; }
        public string Name { get; set; }
        public Nullable<int> CategorySystemID_SQL { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Category> Categories1 { get; set; }
        public virtual ICollection<Classification> Classifications { get; set; }
    }
}
