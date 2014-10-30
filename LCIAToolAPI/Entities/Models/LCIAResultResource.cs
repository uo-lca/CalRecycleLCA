using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// container for detailed LCIA results (such as Process LCIA output).
    /// </summary>
    public class DetailedLCIAResource {
        public int FlowID { get; set; }         // Process LCI flow
        public int DirectionID { get; set; }    
        public double Quantity { get; set; }    // Process LCI result
        public double Factor { get; set; }      // CharacterizationParam value or LCIA factor
        public double Result { get; set; }      // Quantity * Factor
    }

    /// <summary>
    /// Container for Aggregated LCIA results- such as fragmentflow or fragmentstage scores
    /// </summary>
    public class AggregateLCIAResource
    {
        // several of these may be nonzero at once; at least one must be nonzero
        public int? ProcessID;
        public int? FragmentFlowID;
        public int? FragmentStageID;

        // this is computed as the sum(LCIADetail.Result)
        public double CumulativeResult { get; set; }

        public ICollection<DetailedLCIAResource> LCIADetail { get; set; }
    }

    public class LCIAResultResource
    {
        public int LCIAMethodID { get; set; }
        public int ScenarioID { get; set; }

	    public ICollection<AggregateLCIAResource> LCIAScore  { get; set; }
    }
}
