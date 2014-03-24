using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class LCIAMethod
    {
        public int LCIAMethodID { get; set; }
        public int LCIAMethodUUID { get; set; }
        public int LCIAMethodVersion { get; set; }
        public string LCIAMethodName { get; set; }
        public string Methodology { get; set; }
        public string ImpactCategory { get; set; }
        public string ReferenceYear { get; set; }
        public string Duration { get; set; }
        public string ImpactLocation { get; set; }
        public string IndicatorType { get; set; }
        public string Normalization { get; set; }
        public string Weighting { get; set; }
        public string UseAdvice { get; set; }
        public int LCIAMethodSourceID { get; set; }
        public int LCIAMethodFlowPropertyID { get; set; }
    }
}