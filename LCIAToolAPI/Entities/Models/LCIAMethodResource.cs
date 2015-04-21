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
    public class LCIAMethodResource : Resource {
        public LCIAMethodResource() : base("LCIAMethod") { }

        public override int ID { get { return LCIAMethodID; } }
        
        public int LCIAMethodID { get; set; }
        public string Methodology { get; set; }
        public int ImpactCategoryID { get; set; }
        public string ImpactIndicator { get; set; }
        public string ReferenceYear { get; set; }
        public string Duration { get; set; }
        public string ImpactLocation { get; set; }
        public string IndicatorType { get; set; }   // IndicatorType.Name
        public bool? Normalization { get; set; }
        public bool? Weighting { get; set; }
        public string UseAdvice { get; set; }
        public int ReferenceFlowPropertyID { get; set; }
        public FlowPropertyResource ReferenceFlowProperty { get; set; }     
    }

    public class LCIAFactorResource
    {
        public int LCIAMethodID { get; set; }
        public string Geography { get; set; }
        public int FlowID { get; set; }         // Process LCI flow
        public string Direction { get; set; }
        public double Factor { get; set; }      // CharacterizationParam value or LCIA factor
    }
}
