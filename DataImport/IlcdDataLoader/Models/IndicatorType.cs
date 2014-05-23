using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class IndicatorType
    {
        public IndicatorType()
        {
            this.LCIAMethods = new List<LCIAMethod>();
        }

        public int IndicatorTypeID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
