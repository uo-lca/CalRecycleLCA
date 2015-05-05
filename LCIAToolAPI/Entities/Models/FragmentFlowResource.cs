using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {

    /// <summary>
    /// FlowPropertyMagnitude - associates Flow Property with Magnitude.
    /// Embedded in FragmentFlowResource model.
    /// </summary>
    public class FlowPropertyMagnitude {
        public int FlowPropertyID { get; set; }
        public String Unit { get; set; }
        public double Magnitude { get; set; }  // NodeCache.FlowMagnitude * FlowFlowProperty.MeanValue
    }

    public class FragmentStageResource :IEquatable<FragmentStageResource>
    {
        public int FragmentStageID { get; set; }
        public int? FragmentID { get; set; }
        public string Name { get; set; }

        // http://blogs.msdn.com/b/csharpfaq/archive/2009/03/25/how-to-use-linq-methods-to-compare-objects-of-custom-types.aspx
        public bool Equals(FragmentStageResource other)
        {
            if (Object.ReferenceEquals(other, null)) return false;

            if (Object.ReferenceEquals(this, other)) return true;

            return FragmentStageID.Equals(other.FragmentStageID);
        }

        public override int GetHashCode()
        {
            return FragmentStageID.GetHashCode();
        }
    }

    /// <summary>
    /// A FragmentFlowResource is a FragmentFlow merged with related Node data
    /// Contains collection of FlowMagnitude objects - one for every property of the flow
    /// </summary>
    public class FragmentFlowResource {
        //
        // FragmentFlow Properties
        //
        public int FragmentID { get; set; }
        public int FragmentFlowID { get; set; }
        public int? FragmentStageID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }  
        public string NodeType { get; set; }
        public int? FlowID { get; set; }
        public string Direction { get; set; }
        public int? ParentFragmentFlowID { get; set; }
        //
        // derived from NodeCache
        //
        public double? NodeWeight { get; set; }
        //
        // derived from termination
        //
        public bool? IsBackground { get; set; } // ResolveBackground sets this flag; stomps on NodeTypeID
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment
        public bool? isConserved { get; set; }
        public bool? isBalanceFlow { get; set; }

        public ICollection<FlowPropertyMagnitude> FlowPropertyMagnitudes { get; set; }
    }
    
}
