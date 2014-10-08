using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FlowTypeResource - Used for producing web API flow type resources.
    /// Simplifies EF model by omitting navigation properties
    ///
    /// Maintains Pascal case of properties in EF model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FlowTypeResource {
        public int FlowTypeID { get; set; }
        public string Name { get; set; }
    }
}
