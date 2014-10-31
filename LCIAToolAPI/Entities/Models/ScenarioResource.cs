using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Web API resource for Scenario information.
    /// </summary>
    public class ScenarioResource {

        // from Scenario table
        public int ScenarioID { get; set; }
        public string Name { get; set; }
        public int TopLevelFragmentID { get; set; }
        public double ActivityLevel { get; set; }
        public int ReferenceFlowID { get; set; }
        public int ReferenceDirectionID { get; set; }

        // ScenarioGroup.Name
        public int ScenarioGroupID { get; set; }
        
    }

    public class ScenarioParamResource {
        
        // from Param table
        public int ParamID { get; set; }
        public int ParamTypeID { get; set; }
        public int ScenarioID { get; set; }
        public string Name { get; set; }
        // joined from DependencyParam-- nullable -- frontend should 
        public int? FragmentFlowID { get; set; }
        // joined from FlowPropertyParam
        public int? FlowFlowPropertyID { get; set; }
        // joined from ProcessDissipationParam
        public int? ProcessDissipationID { get; set; }
        // joined from ProcessEmissionParam
        public int? ProcessFlowID { get; set; }
        // joined from CharacterizationParam
        public int? LCIAID { get; set; }
        // joined from whatever target table was indicated above
        public float Value { get; set; }
    }

    public class NodeSubstitution {

        // from FragmentFlow
        public int FragmentFlowID { get; set; }
        public int NodeTypeID { get; set; }
        // node type 1
        // from FragmentNodeProcess
        public int DefaultProcessID { get; set; }
        // from ProcessSubstitution
        public int SubstituteProcessID { get; set; }
        // node type 2
        // from FragmentNodeFragment
        public int DefaultFragmentID { get; set; }
        // from ProcessSubstitution
        public int SubstituteFragmentID { get; set; }
    }

    /*************
    public class ScenarioSubstitutionResource {
        
        public int ScenarioID { get; set; }
        // ScenarioBackground
        public ICollection<BackgroundResource> ScenarioBackgrounds { get; set; }
        // ProcessSubstitution and FragmentSubstitution
        // or should these be separate classes?
        public ICollection<NodeSubstitution> ScenarioNodeSubstitutions { get; set; }
        // composition substitution later
    }
     * *************/
}

