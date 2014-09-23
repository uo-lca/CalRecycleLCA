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
        public double Magnitude { get; set; }  // NodeCache.FlowMagnitude * FlowFlowProperty.MeanValue

    }

    /// <summary>
    /// A FragmentFlowResource is a FragmentFlow merged with related Node data
    /// Contains collection of FlowMagnitude objects - one for every property of the flow
    /// </summary>
    public class FragmentFlowResource {
        //
        // FragmentFlow Properties
        //
        public int FragmentFlowID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }        
        public int NodeTypeID { get; set; }
        public int? FlowID { get; set; }
        public int DirectionID { get; set; }
        public int? ParentFragmentFlowID { get; set; }
        //
        // Properties from entities related to FragmentFlow
        //
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment

        public ICollection<FlowPropertyMagnitude> FlowPropertyMagnitudes { get; set; }
    }
    
    /// <summary>
    /// output from ComputeFragmentLCIA. just FragmentFlowID + Result, with some metadata
    /// in the future, report FragmentStageID instead
    /// </summary>
    public class FragmentLCIAResource {
        
        // Outputs from ComputeFragmentLCIA
        public int FragmentFlowID { get; set; }
        public double Result { get; set; }

        // metadata from FragmentFlow (or FragmentStage later)
        public string Name { get; set; }
    }
}
