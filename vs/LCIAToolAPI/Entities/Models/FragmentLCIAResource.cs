using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {

    /// <summary>
    /// FragmentFlowLCIAResource - derived from FragmentLCIAModel
    /// </summary>
    public class FragmentFlowLCIAResource {
        public int FragmentFlowID { get; set; }
        public double Result { get; set; }  // ImpactScore
    }

    // TODO: 
    //public class FragmentStageLCIAResource {
    //    public int FragmentStageID { get; set; }
    //    public double Result { get; set; }  // ImpactScore
    //}
    
    /// <summary>
    /// Web API resource for Fragment LCIA results
    /// </summary>
    public class FragmentLCIAResource {        
        public int ScenarioID;
        public ICollection<FragmentFlowLCIAResource> FragmentFlowLCIAResults { get; set; }
        // TODO: 
        //public ICollection<FragmentStageLCIAResource> FragmentStageLCIAResults { get; set; }
        
    }
}
