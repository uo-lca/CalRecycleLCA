using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// LinkMagnitude - associates Flow Property with Magnitude (Sankey link width).
    /// Embedded in FragmentLink model.
    /// </summary>
    public class LinkMagnitude {
        public int FlowPropertyID { get; set; }
        public double Magnitude { get; set; }  // NodeCache.FlowMagnitude * FlowFlowProperty.MeanValue
                                                
    }
    
    /// <summary>
    /// FragmentLink - model for Sankey diagram link 
    /// Contains collection of LinkMagnitude objects.
    /// </summary>
    public class FragmentLink {
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

        public ICollection<LinkMagnitude> LinkMagnitudes { get; set; }
    }
}
