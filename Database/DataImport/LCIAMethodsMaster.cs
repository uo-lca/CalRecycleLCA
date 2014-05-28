using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataImport
{
    public class LCIAMethodsMaster
    {
        public string LCIAMethodUUID { get; set; }
        public string LCIAMethodName { get; set; }
        public string Methodology { get; set; }
        public string ImpactCategory { get; set; }
        public string ImpactIndicator { get; set; }
        public string ReferenceYear { get; set; }
        public string Duration { get; set; }
        public string ImpactLocation { get; set; }
        public string IndicatorType { get; set; }
        public bool Normalization { get; set; }
        public bool Weighting { get; set; }
        public string UseAdvice { get; set; }
        public string Source { get; set; }
        public string ReferenceQuantity { get; set; }

        public int Ind { get; set; }
        public string LCIAUUID { get; set; }
        public string Flow { get; set; }
        public string Direction { get; set; }
        public float Factor { get; set; }
    }
}