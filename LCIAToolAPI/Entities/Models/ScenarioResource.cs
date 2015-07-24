using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {


    public class ScenarioGroupResource
    {
        public int ScenarioGroupID { get; set; }
        public string Name { get; set; }
        public string Secret { get; set; } // for post only
        public string Visibility { get; set; }
    }


    /// <summary>
    /// Web API resource for Scenario information.
    /// </summary>
    public class ScenarioResource {

        // from Scenario table
        public int ScenarioID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TopLevelFragmentID { get; set; }
        public double ActivityLevel { get; set; }
        public int ReferenceFlowID { get; set; }
        public string ReferenceDirection { get; set; }

        // ScenarioGroup.Name
        public int ScenarioGroupID { get; set; }
        
    }

    /// <summary>
    /// ParamResource describes a parameter.  
    /// Parameters are uniquely determined by a ScenarioID and either one or two internal 
    /// reference IDs, depending on the param type.  
    ///    ParamType:      1   2   3   4   5   6   7   8   9   10
    ///    FragmentFlowID  X   X
    /// CompositionDataID                 (x)
    ///            FlowID              X       X  (x)  X  (x)  X
    ///    FlowPropertyID              X
    ///         ProcessID                      X  (x)  X  (x) 
    ///      LCIAMethodID                                      X
    /// 
    /// Notes: ParamTypeID = 3 is not used / obsolete.
    /// ParamTypeID = 5 is not yet implemented
    /// ParamTypeID = 7, 9 are not going to be implemented
    /// </summary>
    public class ParamResource {
        
        // from Param table
        public int ParamID { get; set; }
        public int ParamTypeID { get; set; }
        public int ScenarioID { get; set; }
        public string Name { get; set; }
        // For ParamTypeID == 1, 2 [ 3 is irrelevant]
        public int? FragmentFlowID { get; set; }
        // For ParamTypeID == 5 // eliminated: CompositionData params manifest as FlowPropertyParams in the outside world
        // public int? CompositionDataID { get; set; }
        // For ParamTypeID == 4, 6, 8, 10
        public int? FlowID { get; set; }
        // For ParamTypeID == 4
        public int? FlowPropertyID { get; set; }
        // For ParamTypeID == 6, 8 [ 7, 9 will not be implemented]
        public int? ProcessID { get; set; }
        // For ParamTypeID == 10
        public int? LCIAMethodID { get; set; }
        // common to all
        public double? Value { get; set; }
        public double? DefaultValue { get; set; }
    }


    public class NodeSubstitutionResource {
        public int ScenarioID { get; set; }
        public int FragmentFlowID { get; set; }
        public int SubstituteILCDEntityID { get; set; }
    }

    public class BackgroundSubstutitionResource {
        public int ScenarioID { get; set ; }
        public int FlowID { get; set; }
        public string Direction { get; set; }
        public int SubstituteILCDEntityID { get; set; }
    }

    /* **************
     * This will wait until composition stuff is fixed in the schema
    public class CompositionSubstitutionResource {
        public int ScenarioID { get; set; }

    }
     * ************ */

    /// <summary>
    /// The set of params and substitutions associated with a given scenario.
    /// </summary>
    public class ScenarioDetailResource {

        public int ScenarioID { get; set; }
        public IEnumerable<ParamResource> ScenarioParams { get; set; }

        public IEnumerable<NodeSubstitutionResource> NodeSubstitutions { get; set; }
        public IEnumerable<BackgroundSubstutitionResource> BackgroundSubstitutions { get; set; }
        // public IEnumerable<CompositionSubstitutionResource> CompositionSubstitutions { get; set; }
    }

}

