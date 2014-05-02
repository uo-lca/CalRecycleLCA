using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class LCIAMethodModel
    {
        public int LCIAMethodID { get; set; }
        public string LCIAMethodUUID { get; set; }
        public string LCIAMethodVersion { get; set; }
        public string Name { get; set; }
        public string Methodology { get; set; }
        [JsonIgnore]
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
    }
}