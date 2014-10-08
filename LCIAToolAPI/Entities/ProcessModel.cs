using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// ProcessModel - Used for producing process web service data.
    /// Simplifies database model by omitting relationships and ILCDEntityID
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class ProcessModel {

        public int ProcessID { get; set; }

        public string Name { get; set; }

        public string ReferenceYear { get; set; }

        public string Geography { get; set; }

        public int? ReferenceTypeID { get; set; }

        public int ProcessTypeID { get; set; }

        public int? ReferenceFlowID { get; set; }
    }
}
