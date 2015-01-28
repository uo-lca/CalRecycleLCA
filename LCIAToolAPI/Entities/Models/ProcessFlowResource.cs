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
        public double Magnitude { get; set; }
        public double Result { get; set; }
        public double STDev { get; set; }
    }
}
