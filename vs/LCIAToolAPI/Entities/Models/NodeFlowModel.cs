using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class NodeFlowModel
    {
        public int? FlowID { get; set; }
        public int? DirectionID { get; set; }
        public double? Result { get; set; }
        public double? FlowMagnitude { get; set; }
        public int? RefFlowID { get; set; }
        [JsonIgnore]
        public int? FragmentID { get; set; }
        [JsonIgnore]
        public int? ScenarioID { get; set; }
        [JsonIgnore]
        public int? NodeTypeID { get; set; }
    }
}
