using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// ProcessFlowResource - Used for producing web API processflow resources.
    /// Simplifies EF model by omitting navigation properties.
    /// Reference to Flow is changed to reference to FlowResource
    /// Fields that should never contain NULL values have non-nullable properties.
    /// Fields that are currently have no values have been omitted.
    ///
    /// Maintains Pascal case of properties in EF model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    /// 
    public class ProcessFlowResource {

        // public int ProcessFlowID { get; set; } // not necessary; may expose information
        public FlowResource Flow { get; set; }
        public string Direction { get; set; }
        public string VarName { get; set; }
        /// <summary>
        /// note: content = composition * FlowPropertyEmission.Scale to account for molecular
        /// weight conversion.  So e.g. a flow with 0.85 carbon CompositionData
        /// would show up as a content of 0.85 * 44 / 12 = 3.12 for Flow CO2
        /// </summary>
        public double? Content { get; set; }
        /// <summary>
        /// Dissipation = fraction of content that gets emitted.  (1 - Dissipation) = fraction retained.
        /// </summary>
        public double? Dissipation { get; set; }
        /// <summary>
        /// For dissipation flows: Content * Dissipation = Quantity.  
        /// For non-dissipation flows, Quantity is ProcessFlow.Result.
        /// Composition, Dissipation, ProcessFlow.Result can have Type 5, 6, 8 params respectively.
        /// </summary>
        public double Quantity { get; set; }
        public double STDev { get; set; }
    }
}
