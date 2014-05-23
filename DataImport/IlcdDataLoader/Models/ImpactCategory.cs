using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class ImpactCategory
    {
        public ImpactCategory()
        {
            this.LCIAMethods = new List<LCIAMethod>();
        }

        public int ImpactCategoryID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
