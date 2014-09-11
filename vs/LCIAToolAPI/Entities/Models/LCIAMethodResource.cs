using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Web API resource for LCIA methods.
    /// Contains simple properties of LCIAMethod (no object references or collections).
    /// Fixes some modeling problems in LCIAMethod:
    ///     No property should be null, 
    ///     and foreign key name should match referenced primary key name.
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
        public int IndicatorTypeID { get; set; }
        public bool Normalization { get; set; }
        public bool Weighting { get; set; }
        public string UseAdvice { get; set; }
        public int FlowPropertyID { get; set; }
    }
}
