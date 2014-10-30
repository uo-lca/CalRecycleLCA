// PROPOSED new definition for LCIAResultResource

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Proposed {
    /// <summary>
    /// Web API resource for Process LCIA result.
    /// </summary>
    public class DetailedLCIAResource {  /// WAS ProcessLCIAResultResource
        public int FlowID { get; set; }         // Process LCI flow
        public int DirectionID { get; set; }    
        public double Quantity { get; set; }    // Process LCI result
        public double Factor { get; set; }      // CharacterizationParam value or LCIA factor
        public double Result { get; set; }      // Quantity * Factor
    }

    public class AggregateLCIAResource {
	// several of these may be nonzero at once; at least one must be nonzero
	public int? ProcessID;
	public int? FragmentFlowID;
	public int? FragmentStageID;

	// this is computed as the sum(LCIADetail.Result)
	public double CumulativeResult { get; set; }

	public ICollection<DetailedLCIAResource> LCIADetail { get; set; }
    }

    public class LCIAResultResource {

        public int ScenarioID;
        public int LCIAMethodID { get; set; }

        public ICollection<AggregateLCIAResource> LCIAResults  { get; set; }
    }
}
