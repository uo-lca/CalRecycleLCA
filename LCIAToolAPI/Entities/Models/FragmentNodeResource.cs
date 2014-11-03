using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FragmentNodeResource
    {
        public int? RefID { get; set; } // referer-- either FragmentNodeProcessID, FragmentNodeFragmentID, FragmentFlowID, BackgroundID
        public int ScenarioID { get; set; }   // node behavior is scenario dependent
        public int NodeTypeID { get; set; }   // included here for background resolution
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment
        public int TermFlowID { get; set; }  //  FragmentNodeProcess.FlowID or FragmentNodeFragment.FlowID depending on whether NodeType is Process or Fragment 
    }
}
