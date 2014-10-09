using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Web API resource for LCIA methods.
    /// 
    /// Contains simple properties of LCIAMethod, 
    /// collapsed IndicatorType,
    /// and reference to flow property for ease of use
    /// 
    /// </summary>
    public class LCIAMethodResource {
        public int LCIAMethodID { get; set; }
        public string Name { get; set; }
        public string Methodology { get; set; }
        public int ImpactCategoryID { get; set; }
        public string ImpactIndicator { get; set; }
        public string ReferenceYear { get; set; }
        public string Duration { get; set; }
        public string ImpactLocation { get; set; }
        public string IndicatorType { get; set; }   // IndicatorType.Name
        public bool Normalization { get; set; }
        public bool Weighting { get; set; }
        public string UseAdvice { get; set; }
        public FlowPropertyResource ReferenceFlowProperty { get; set; }     
    }
}
