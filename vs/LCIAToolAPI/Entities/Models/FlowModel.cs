using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FlowModel - Used for producing flows web service data.
    /// Simplifies database model by omitting relationships and ILCDEntityID
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FlowModel {

        public int FlowID { get; set; }
        public string Name { get; set; }
        public string CASNumber { get; set; }
        public int? ReferenceFlowPropertyID { get; set; }
        public int? FlowTypeID { get; set; }
    }
}
