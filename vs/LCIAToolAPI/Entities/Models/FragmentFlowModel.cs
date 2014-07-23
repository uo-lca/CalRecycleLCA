using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities.Models {
    /// <summary>
    /// FragmentFlowModel - Used for producing fragmentflows web service data.
    /// Simplifies database model by omitting unused relationships and
    /// embedding related data. 
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FragmentFlowModel
    {
        //
        // Data Model Properties
        //
        public int FragmentFlowID { get; set; }

        public int? FragmentID { get; set; }

        public string Name { get; set; }

        public int? FragmentStageID { get; set; }

        public int? ReferenceFlowPropertyID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? ParentFragmentFlowID { get; set; }
        //
        // Properties from related objects
        //
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment
        public int? ScenarioID { get; set; }    // NodeCache.ScenarioID
        public double? NodeWeight { get; set; } // NodeCache.NodeWeight

    }
}
