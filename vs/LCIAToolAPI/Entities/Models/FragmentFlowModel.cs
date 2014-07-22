using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities.Models {
    // JSON model for fragmentflows
    public class FragmentFlowModel
    {
        public int fragmentFlowID { get; set; }
        
        public int? fragmentID { get; set; }
        
        public string name { get; set; }

        public int? fragmentStageID { get; set; }

        public int? referenceFlowPropertyID { get; set; }

        public int? nodeTypeID { get; set; }

        public int? flowID { get; set; }

        public int? directionID { get; set; }

        public int? parentFragmentFlowID { get; set; }

        public int? nodeID { get; set; }    // ID of process or fragment related to node

        public double? nodeWeight { get; set; } // derived from related NodeCache

    }
}
