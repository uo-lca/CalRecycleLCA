using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FragmentModel - Used for producing fragment web service data.
    /// Simplifies database model by omitting relationships and ILCDEntityID
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FragmentModel {

        public int FragmentID { get; set; }

        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }

    }
}
