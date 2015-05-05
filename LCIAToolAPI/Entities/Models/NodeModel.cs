using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FlowTerminationModel
    {
        public int? RefID { get; set; } // referer-- either FragmentNodeProcessID, FragmentNodeFragmentID, FragmentFlowID, BackgroundID
        public int ScenarioID { get; set; }   // node behavior is scenario dependent
        public int NodeTypeID { get; set; }   // included here for background resolution
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment
        public int TermFlowID { get; set; }  //  FragmentNodeProcess.FlowID or FragmentNodeFragment.FlowID depending on whether NodeType is Process or Fragment 
        public int? BalanceFFID { get; set; } // the outbound FFID for balancing
    }

    /// <summary>
    /// Used to store local nodecaches prior to DB insert.
    /// This could be folded into FragmentNodeResource -- which should really be named FragmentNodeModel
    /// </summary>
    public class NodeCacheModel
    {
        public int FragmentID { get; set; }
        public int NodeTypeID { get; set; }
        public int? FlowID { get; set; }
        public int DirectionID { get; set; }
        public int FragmentFlowID { get; set; }
        public int ScenarioID { get; set; }
        public double NodeWeight { get; set; }
        public double FlowMagnitude { get; set; }
    }

}
