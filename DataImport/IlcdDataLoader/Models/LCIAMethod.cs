using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class LCIAMethod
    {
        public LCIAMethod()
        {
            this.LCIAs = new List<LCIA>();
        }

        public int LCIAMethodID { get; set; }
        public string LCIAMethodUUID { get; set; }
        public string LCIAMethodVersion { get; set; }
        public string Name { get; set; }
        public string Methodology { get; set; }
        public Nullable<int> ImpactCategoryID { get; set; }
        public string ImpactIndicator { get; set; }
        public string ReferenceYear { get; set; }
        public string Duration { get; set; }
        public string ImpactLocation { get; set; }
        public Nullable<int> IndicatorTypeID { get; set; }
        public Nullable<bool> Normalization { get; set; }
        public Nullable<bool> Weighting { get; set; }
        public string UseAdvice { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<int> FlowPropertyID { get; set; }
        public string Source { get; set; }
        public string ReferenceQuantity { get; set; }
        public virtual FlowProperty FlowProperty { get; set; }
        public virtual ImpactCategory ImpactCategory { get; set; }
        public virtual IndicatorType IndicatorType { get; set; }
        public virtual ICollection<LCIA> LCIAs { get; set; }
    }
}
