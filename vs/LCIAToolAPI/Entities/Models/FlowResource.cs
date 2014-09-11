using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FlowResource - Used for producing web API flow resources.
    /// Simplifies EF model by omitting navigation properties and ILCDEntityID
    ///
    /// Maintains Pascal case of properties in EF model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FlowResource {

        public int FlowID { get; set; }
        public string Name { get; set; }
        public string CASNumber { get; set; }
        public int ReferenceFlowPropertyID { get; set; }
        public int FlowTypeID { get; set; }
    }
}
