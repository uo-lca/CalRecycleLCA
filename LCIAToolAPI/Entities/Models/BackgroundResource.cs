using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Web API resource for background information.
    /// </summary>
    public class BackgroundResource {
        
        public int FlowID { get; set; }
        public int DirectionID { get; set; }
        public int NodeTypeID { get; set; }
        // if NodeTypeID = 1
        public int? TargetProcessID { get; set; }
        // if NodeTypeID = 2
        public int? TargetFragmentID { get; set; }
    }
}
