using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FlowPropertyModel - Used for producing flow property web service data.
    /// Simplifies FlowProperty by collapsing relationships and omitting unused properties
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FlowPropertyModel {

        public int FlowPropertyID { get; set; }
        public string Name { get; set; }
        public string ReferenceUnitName { get; set; }  // FlowProperty.UnitGroup.UnitConversion.Unit 
    }
}
