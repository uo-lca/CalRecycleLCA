using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Web API resource for Process LCIA result.
    /// </summary>
    public class ProcessLCIAResultResource {
        public int FlowID { get; set; }         // Process LCI flow
        public int DirectionID { get; set; }    
        public double Quantity { get; set; }    // Process LCI result
        public double Factor { get; set; }      // CharacterizationParam value or LCIA factor
        public double Result { get; set; }      // Quantity * Factor
    }

    public class LCIAResultResource {

        public int LCIAMethodID { get; set; }

	    public ICollection<ProcessLCIAResultResource> ProcessLCIAResults  { get; set; }
    }
}
