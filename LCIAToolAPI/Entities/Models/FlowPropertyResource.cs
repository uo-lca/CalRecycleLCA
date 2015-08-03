using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    /// <summary>
    /// FlowPropertyResource - Used for producing flow property web service data.
    /// Simplifies FlowProperty by collapsing relationships and omitting unused properties
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FlowPropertyResource : Resource
    {

        public FlowPropertyResource() : base("FlowProperty") { }

        public override int ID { get { return FlowPropertyID; } }

        public int FlowPropertyID { get; set; }
        public string ReferenceUnit { get; set; }  // FlowProperty.UnitGroup.UnitConversion.Unit 
    }

    /// <summary>
    /// FlowPropertyMagnitude - associates Flow Property with Magnitude.
    /// Embedded in FragmentFlowResource model.
    /// </summary>
    public class FlowPropertyMagnitude
    {
        public int? FlowPropertyID { get; set; }
        public string Unit { get; set; }
        public FlowPropertyResource FlowProperty { get; set; }
        public double Magnitude { get; set; }  // NodeCache.FlowMagnitude * FlowFlowProperty.MeanValue
    }
}

